/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Evereal.YoutubeDLPlayer
{
  public class YTDLVideoPlayer : YTDLPlayerBase
  {
    #region Properties

    // Video render type
    public RenderType renderType = RenderType.MATERIAL;
    // Video player renderer
    public Renderer videoRenderer;
    // Video render target camera
    public Camera targetCamera;

    // Unity video player instance
    private VideoPlayer videoPlayer;

    // Video player audio source instance
    private AudioSource audioSource;

    // Whether content is being played. (Read Only)
    public bool isPlaying
    {
      get
      {
        return videoPlayer.isPlaying;
      }
    }

    // Whether playback is paused. (Read Only)
    public bool isPaused
    {
      get; private set;
    }
        /// <summary>
        /// La calidad Maxima que traeria el video, 
        /// ¡Hey, esto lo agregó Oscar Gutierrez!
        /// </summary>
        public enum EZVideoQuality 
        {
            Best,
            Best720,
            Best480,
            Best360
        }
        /// <summary>
        /// Dejamos por default la calidad máxima a 480p
        /// </summary>
        public EZVideoQuality videoQuality = EZVideoQuality.Best480;

    #endregion

    #region Events

    // Invoked when <c>YTDLParser</c> parse started.
    public event ParseStartedEvent parseStarted = delegate { };

    // Invoked when <c>YTDLParser</c> parse completed.
    public event ParseCompletedEvent parseCompleted = delegate { };
    private void OnParseCompleted(MediaInfo info)
    {
      mediaInfo = info;

      // extract available video format
      ExtractAvailableVideoFormat();

      // set video player url
      videoPlayer.url = ValidParsedVideoUrl(mediaInfo, mediaInfo.url);

      // set video to play then prepare audio to prevent buffering
      videoPlayer.Prepare();

      if (autoPlay)
      {
        // play video
        videoPlayer.Play();
      }

      // parse completed
      isParsed = true;

      parseCompleted(this, mediaInfo);
    }

    // Invoked when the <c>VideoPlayer</c> preparation is complete.
    public event PrepareCompletedEvent prepareCompleted = delegate { };
    private void OnPrepareCompleted(VideoPlayer player)
    {
      prepareCompleted(this);
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
      Debug.LogWarningFormat(LOG_FORMAT, message);
      errorReceived(this, ErrorCode.PLAY_VIDEO_FAILED);
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

    public void Parse(bool auto = true)
    {
      if (string.IsNullOrEmpty(url))
      {
        Debug.LogWarningFormat(LOG_FORMAT, "Please provide the video url!");
        return;
      }
      autoPlay = auto;
      ytdlParser.SetOptions(GetVideoUrlParseOptions(url));
      StartCoroutine(ytdlParser.PrepareAndParse(ValidVideoUrl(url)));
      parseStarted(this);
    }

    public void Prepare()
    {
      if (!isParsed)
      {
        return;
      }
      videoPlayer.Prepare();
    }

    public void Play()
    {
      if (!isParsed)
      {
        Parse();
        return;
      }
      videoPlayer.Play();
      isPaused = false;
    }

    public void Pause()
    {
      videoPlayer.Pause();
      isPaused = true;
    }

    public void Stop()
    {
      videoPlayer.Stop();
    }

    public void StepForward()
    {
      videoPlayer.StepForward();
    }

    public bool GetAudioMute(ushort trackIndex)
    {
      if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
      {
        return videoPlayer.GetDirectAudioMute(trackIndex);
      }
      else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
      {
        return videoPlayer.GetTargetAudioSource(trackIndex).mute;
      }
      return false;
    }

    public void SetAudioMute(ushort trackIndex, bool mute)
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

    public float GetAudioVolume(ushort trackIndex)
    {
      if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
      {
        return videoPlayer.GetDirectAudioVolume(trackIndex);
      }
      else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
      {
        return videoPlayer.GetTargetAudioSource(trackIndex).volume;
      }
      return 0f;
    }

    public void SetAudioVolume(ushort trackIndex, float volume)
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

    public double time
    {
      get
      {
        return videoPlayer.time;
      }
      set
      {
        videoPlayer.time = value;
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

    private void ExtractAvailableVideoFormat()
    {
      availableMediaFormat.Clear();
      // parse valid format for unity video player
      foreach (MediaFormat format in mediaInfo.formats)
      {
        // TODO, support other container and protocol
        if (
          format.ext == "mp4" &&
          format.acodec != "none" &&
          (format.protocol == "https" || format.protocol == "http")
        )
        {
          availableMediaFormat.Add(format);
        }
      }
    }
    /// <summary>
    /// Este método fue editado por Oscar Gutierrez para que los usuarios que tengan pc con pocos recursos 
    /// no se mamen con videos pesados
    /// </summary>
    /// <param name="url">Esta variable ni se para que la usaban, al parecer no hace nada...</param>
    /// <returns></returns>
    private string GetVideoUrlParseOptions(string url)
    {
            var bestFormat = "";
            if (videoQuality == EZVideoQuality.Best720) bestFormat = "best[height<=720]";
            else if (videoQuality == EZVideoQuality.Best480) bestFormat = "best[height<=480]";
            else if (videoQuality == EZVideoQuality.Best360) bestFormat = "best[height<=360]";
            return string.Format("--format {0}[protocol=https][ext=mp4]/[protocol=http][ext=mp4] --no-cache-dir", bestFormat);
            //return Constants.WORST_YTDL_VIDEO_PARSE_OPTIONS;
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
      // initial media format list
      availableMediaFormat = new List<MediaFormat>();
    }

    private void Start()
    {
      if (string.IsNullOrEmpty(url))
      {
        Debug.LogWarningFormat(LOG_FORMAT, "Please provide the video url!");
        return;
      }
      if (autoPlay)
      {
        Play();
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
    }

    #endregion
  }
}