/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System;
using UnityEngine;
using UnityEngine.Video;

namespace Evereal.YoutubeDLPlayer
{
  public class YTDLVideoPlayer : YTDLPlayerBase
  {
    #region Properties

    // Initial video quality settings
    public QualityType initialQuality = QualityType._720p;
    // Video render type
    public RenderType renderType = RenderType.MATERIAL;
    // Video player renderer
    public Renderer videoRenderer;
    // Video render target camera
    public Camera targetCamera;

    // Unity video player instance
    private VideoPlayer videoPlayer;

    // Just another video player instance
    // TODO, replace with a better solution such as cache audio file
    private VideoPlayer audioPlayer;
    private bool useAudioPlayer = false;

    // Video player audio source instance
    private AudioSource audioSource;

    public int formatIndex { get; private set; }

    // Whether content is being played. (Read Only)
    public bool isPlaying
    {
      get
      {
        return videoPlayer.isPlaying;
      }
    }

    // Whether the VideoPlayer has successfully prepared the content to be played. (Read Only)
    public bool isPrepared
    {
      get
      {
        return videoPlayer.isPrepared;
      }
    }

    // Whether playback is paused. (Read Only)
    public bool isPaused
    {
      get; private set;
    }

    #endregion

    #region Events

    // Invoked when <c>YTDLParser</c> parse started.
    public event ParseStartedEvent parseStarted = delegate { };

    // Invoked when <c>YTDLParser</c> parse completed.
    public event ParseCompletedEvent parseCompleted = delegate { };
    private void OnParseCompleted(MediaInfo info)
    {
      // parse completed
      isParsed = true;

      mediaInfo = info;

      // extract supported video format
      ExtractSupportedMediaFormats(mediaInfo);

      if (supportedVideoFormats.Count > 0)
      {
        // set initial quality format
        formatIndex = TryGetInitialFormatIndex();

        MediaFormat format = supportedVideoFormats[formatIndex];

        // use audio player if no audio stream found
        if (format.acodec == "none")
        {
          useAudioPlayer = true;
          MediaFormat audioFormat = TryGetAudioStream();
          InitAudioPlayer(audioFormat.url);
        }
        else
        {
          if (audioPlayer != null)
          {
            audioPlayer.Stop();
          }
          useAudioPlayer = false;
        }

        // set video player url
        videoPlayer.url = ValidParsedVideoUrl(mediaInfo, format.url);

        // prepare video for play
        Prepare();
      }
      else
      {
        Debug.LogWarningFormat(LOG_FORMAT, "No supported video stream format found!");
      }

      parseCompleted(this, mediaInfo);
    }

    // Invoked when the <c>VideoPlayer</c> preparation is complete.
    public event PrepareCompletedEvent prepareCompleted = delegate { };
    private void OnPrepareCompleted(VideoPlayer player)
    {
      if ((useAudioPlayer && audioPlayer.isPrepared) || !useAudioPlayer)
      {
        Play();

        prepareCompleted(this);
      }
    }

    // Invoked when the <c>VideoPlayer</c> clock is synced back to its <c>VideoTimeReference</c>.
    public event ClockResyncOccurredEvent clockResyncOccurred = delegate { };
    private void OnClockResyncOccurred(VideoPlayer source, double seconds)
    {
      clockResyncOccurred(this, seconds);
    }

    // Invoke after a seek operation completes.
    public event SeekCompletedEvent seekCompleted = delegate { };
    private void OnSeekCompleted(VideoPlayer source)
    {
      seekCompleted(this);
    }

    // Invoked immediately after Play is called.
    public event StartedEvent started = delegate { };
    private void OnStarted(VideoPlayer source)
    {
      started(this);
    }

    // Invoked when the <c>VideoPlayer</c> reaches the end of the content to play.
    public event LoopPointReachedEvent loopPointReached = delegate { };
    private void OnLoopPointReached(VideoPlayer source)
    {
      loopPointReached(this);
    }

    // Invoked when a new frame is ready.
    public event FrameReadyEvent frameReady = delegate { };
    private void OnFrameReady(VideoPlayer source, long frameIdx)
    {
      frameReady(this, frameIdx);
    }

    // Invoked when error occurred, such as HTTP connection problems are reported through this callback.
    public event ErrorReceivedEvent errorReceived = delegate { };
    private void OnParserErrorReceived(YTDLParser.ErrorEvent error)
    {
      Debug.LogErrorFormat(LOG_FORMAT, error.message);
      errorReceived(this, error.code);
    }
    private void OnErrorReceived(VideoPlayer source, string message)
    {
      Debug.LogErrorFormat(LOG_FORMAT, message);
      errorReceived(this, ErrorCode.PLAY_VIDEO_FAILED);
    }

    // Invoked when the audio player preparation is complete.
    private void OnAudioPrepareCompleted(VideoPlayer source)
    {
      if (isPrepared)
      {
        // if video already prepared
        Play();

        prepareCompleted(this);
      }
    }

    #endregion

    #region YTDL Video Player

    public VideoPlayer GetVideoPlayer()
    {
      return videoPlayer;
    }

    public YTDLParser GetYTDLParser()
    {
      return ytdlParser;
    }

    public MediaInfo GetVideoInfo()
    {
      return mediaInfo;
    }

    public void SwitchVideoFormat(int index)
    {
      if (index >= supportedVideoFormats.Count || index < 0)
        return;
      Stop();
      formatIndex = index;
      MediaFormat format = supportedVideoFormats[formatIndex];
      // use audio player if no audio stream found
      if (format.acodec == "none")
      {
        useAudioPlayer = true;
        MediaFormat audioFormat = TryGetAudioStream();
        InitAudioPlayer(audioFormat.url);
      }
      else
      {
        if (audioPlayer != null)
        {
          audioPlayer.Stop();
        }
        useAudioPlayer = false;
      }
      SetVideoParsedUrl(format.url);
      Prepare();
    }

    public void SetVideoUrl(string url)
    {
      this.url = url;
      isParsed = false;
    }

    public void SetVideoParsedUrl(string url)
    {
      videoPlayer.url = ValidParsedVideoUrl(mediaInfo, url);
    }

    public void SetRenderType(RenderType type)
    {
      renderType = type;
      SwitchRenderType();
    }

    public void SetVideoRenderer(Renderer renderer)
    {
      videoRenderer = renderer;
    }

    public void SetTargetCamera(Camera cam)
    {
      targetCamera = cam;
    }

    public void Parse()
    {
      if (string.IsNullOrEmpty(url))
      {
        Debug.LogWarningFormat(LOG_FORMAT, "Please provide the video url!");
        return;
      }
      ytdlParser.SetOptions(Constants.DEFAULT_YTDL_VIDEO_PARSE_OPTIONS);
      StartCoroutine(ytdlParser.PrepareAndParse(ValidVideoUrl(url)));
      parseStarted(this);
    }

    public void Prepare()
    {
      if (!isParsed)
      {
        Parse();
        return;
      }
      if (!isParsed)
      {
        return;
      }
      if (!videoPlayer.isPrepared)
      {
        videoPlayer.Prepare();
      }
      if (useAudioPlayer && !audioPlayer.isPrepared)
      {
        audioPlayer.Prepare();
      }
    }

    public void Play()
    {
      if (!isParsed)
      {
        Parse();
        return;
      }
      videoPlayer.Play();
      if (useAudioPlayer)
      {
        audioPlayer.Play();
      }
      isPaused = false;
    }

    public void Pause()
    {
      videoPlayer.Pause();
      if (useAudioPlayer)
      {
        audioPlayer.Pause();
      }
      isPaused = true;
    }

    public void Stop()
    {
      videoPlayer.Stop();
      if (useAudioPlayer)
      {
        audioPlayer.Stop();
      }
    }

    public void StepForward()
    {
      videoPlayer.StepForward();
      if (useAudioPlayer)
      {
        audioPlayer.StepForward();
      }
    }

    public bool GetAudioMute(ushort trackIndex)
    {
      if (useAudioPlayer)
      {
        if (audioPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
          return audioPlayer.GetDirectAudioMute(trackIndex);
        }
        else if (audioPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
          return audioPlayer.GetTargetAudioSource(trackIndex).mute;
        }
      }
      else
      {
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
          return videoPlayer.GetDirectAudioMute(trackIndex);
        }
        else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
          return videoPlayer.GetTargetAudioSource(trackIndex).mute;
        }
      }
      return false;
    }

    public void SetAudioMute(ushort trackIndex, bool mute)
    {
      if (useAudioPlayer)
      {
        if (audioPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
          audioPlayer.SetDirectAudioMute(trackIndex, mute);
        }
        else if (audioPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
          audioPlayer.GetTargetAudioSource(trackIndex).mute = mute;
        }
      }
      else
      {
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
          videoPlayer.SetDirectAudioMute(trackIndex, mute);
        }
        else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
          videoPlayer.GetTargetAudioSource(trackIndex).mute = mute;
        }
      }
    }

    public float GetAudioVolume(ushort trackIndex)
    {
      if (useAudioPlayer)
      {
        if (audioPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
          return audioPlayer.GetDirectAudioVolume(trackIndex);
        }
        else if (audioPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
          return audioPlayer.GetTargetAudioSource(trackIndex).volume;
        }
      }
      else
      {
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
          return videoPlayer.GetDirectAudioVolume(trackIndex);
        }
        else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
          return videoPlayer.GetTargetAudioSource(trackIndex).volume;
        }
      }
      return 0f;
    }

    public void SetAudioVolume(ushort trackIndex, float volume)
    {
      if (useAudioPlayer)
      {
        if (audioPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
          audioPlayer.SetDirectAudioVolume(trackIndex, volume);
        }
        else if (audioPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
          audioPlayer.GetTargetAudioSource(trackIndex).volume = volume;
        }
      }
      else
      {
        if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
        {
          videoPlayer.SetDirectAudioVolume(trackIndex, volume);
        }
        else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
          videoPlayer.GetTargetAudioSource(trackIndex).volume = volume;
        }
      }
    }

    public double time
    {
      get
      {
        return videoPlayer.time;
      }
      set
      {
        videoPlayer.time = value;
        if (useAudioPlayer)
        {
          audioPlayer.time = value;
        }
      }
    }

    public long frame
    {
      get
      {
        return videoPlayer.frame;
      }
      set
      {
        videoPlayer.frame = value;
        if (useAudioPlayer)
        {
          audioPlayer.frame = value;
        }
      }
    }

    public ulong frameCount
    {
      get
      {
        return videoPlayer.frameCount;
      }
    }

    #endregion

    #region Internal

    private void SwitchRenderType()
    {
      switch (renderType)
      {
        case RenderType.MATERIAL:
          videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
          if (videoRenderer == null)
          {
            Debug.LogErrorFormat(LOG_FORMAT, "Video renderer not correctly set!");
          }
          videoPlayer.targetMaterialRenderer = videoRenderer;
          break;
        case RenderType.SCREEN:
          videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
          if (targetCamera == null)
          {
            Debug.LogErrorFormat(LOG_FORMAT, "Target camera not correctly set!");
          }
          videoPlayer.targetCamera = targetCamera;
          break;
      }
    }

    private string ValidVideoUrl(string url)
    {
      if (url.Contains("www.youtube.com"))
      {
        string[] parts = url.Split('&');
        return parts[0];
      }
      return url;
    }

    private string ValidParsedVideoUrl(MediaInfo mediaInfo, string url)
    {
      if (string.IsNullOrEmpty(url))
        return url;
      if (mediaInfo.extractor == "vimeo")
      {
        url = url.Replace("source=1", "");
      }
      return url;
    }

    private void InitAudioPlayer(string audioUrl)
    {
      if (audioPlayer == null)
      {
        // initial video player
        audioPlayer = gameObject.AddComponent<VideoPlayer>();
        audioPlayer.renderMode = VideoRenderMode.APIOnly;
        audioPlayer.source = VideoSource.Url;
        audioPlayer.playOnAwake = false;
        audioPlayer.waitForFirstFrame = true;
        audioPlayer.skipOnDrop = false;
        audioPlayer.isLooping = loop;

        // set audio output mode to AudioSource
        audioPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        audioPlayer.controlledAudioTrackCount = 1;
        audioPlayer.EnableAudioTrack(0, true);
        audioPlayer.SetTargetAudioSource(0, audioSource);

        audioPlayer.prepareCompleted += OnAudioPrepareCompleted;
      }

      string validUrl = ValidParsedVideoUrl(mediaInfo, audioUrl);
      if (audioPlayer.url != validUrl)
      {
        audioPlayer.url = validUrl;
      }
    }

    private MediaFormat TryGetAudioStream()
    {
      // get first available audio stream
      for (int i = 0; i < supportedVideoFormats.Count; i++)
      {
        MediaFormat format = supportedVideoFormats[i];
        if (format.acodec != "none" && format.acodec != null)
        {
          return format;
        }
      }
      return null;
    }

    private int TryGetInitialFormatIndex()
    {
      // get target height
      int targetHeight = 360;
      switch (initialQuality)
      {
        case QualityType._360p:
          targetHeight = 360;
          break;
        case QualityType._720p:
          targetHeight = 720;
          break;
        case QualityType._1080p:
          targetHeight = 1080;
          break;
        case QualityType._1440p:
          targetHeight = 1440;
          break;
        case QualityType._2160p:
          targetHeight = 2160;
          break;
      }
      // search best fit video stream index
      int index = 0, minDiff = int.MaxValue;
      for (int i = 0; i < supportedVideoFormats.Count; i++)
      {
        MediaFormat format = supportedVideoFormats[i];
        int diff = Math.Abs(format.height - targetHeight);
        if (diff < minDiff)
        {
          index = i;
          minDiff = diff;
        }
      }
      return index;
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
      // initial youtube-dl parser
      ytdlParser = GetComponent<YTDLParser>();

      // initial audio source
      audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.playOnAwake = false;

      // initial video player
      videoPlayer = gameObject.AddComponent<VideoPlayer>();
      videoPlayer.source = VideoSource.Url;
      videoPlayer.playOnAwake = false;
      videoPlayer.waitForFirstFrame = true;
      videoPlayer.skipOnDrop = false;
      videoPlayer.isLooping = loop;

      // set audio output mode to AudioSource
      videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
      videoPlayer.controlledAudioTrackCount = 1;
      videoPlayer.EnableAudioTrack(0, true);
      videoPlayer.SetTargetAudioSource(0, audioSource);

      // set video render type
      SwitchRenderType();
    }

    private void Start()
    {
      if (autoPlay)
      {
        Prepare();
      }
    }

    private void Update()
    {
      if (useAudioPlayer)
      {
        // sync video/audio player
        if (Math.Abs((videoPlayer.time - audioPlayer.time)) >= 1)
        {
          audioPlayer.time = videoPlayer.time;
        }
      }
    }

    private void OnEnable()
    {
      // bind ytdl parser events
      ytdlParser.parseCompleted += OnParseCompleted;
      ytdlParser.errorReceived += OnParserErrorReceived;
      // override video player event
      videoPlayer.started += OnStarted;
      videoPlayer.prepareCompleted += OnPrepareCompleted;
      videoPlayer.frameReady += OnFrameReady;
      videoPlayer.seekCompleted += OnSeekCompleted;
      videoPlayer.clockResyncOccurred += OnClockResyncOccurred;
      videoPlayer.loopPointReached += OnLoopPointReached;
      videoPlayer.errorReceived += OnErrorReceived;
    }

    private void OnDisable()
    {
      ytdlParser.parseCompleted -= OnParseCompleted;
      ytdlParser.errorReceived -= OnParserErrorReceived;

      videoPlayer.started -= OnStarted;
      videoPlayer.prepareCompleted -= OnPrepareCompleted;
      videoPlayer.frameReady -= OnFrameReady;
      videoPlayer.seekCompleted -= OnSeekCompleted;
      videoPlayer.clockResyncOccurred -= OnClockResyncOccurred;
      videoPlayer.loopPointReached -= OnLoopPointReached;
      videoPlayer.errorReceived -= OnErrorReceived;

      if (audioPlayer != null)
      {
        audioPlayer.prepareCompleted -= OnAudioPrepareCompleted;
        Destroy(audioPlayer);
        audioPlayer = null;
      }
    }

    #endregion
  }
}