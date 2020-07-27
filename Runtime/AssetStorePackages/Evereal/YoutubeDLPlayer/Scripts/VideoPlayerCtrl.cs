/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  [RequireComponent(typeof(YTDLVideoPlayer))]
  public class VideoPlayerCtrl : MonoBehaviour
  {
    #region Properties

    public VideoTitle videoTitle;
    public VideoTime videoTime;
    public PlayButton playButton;
    public GameObject loadingCircle;
    public GameObject progressCircle;
    public GameObject progressBar;
    public GameObject progressBarBG;
    public GameObject volumeCircle;
    public GameObject volumeBar;
    public GameObject volumeBarBG;
    public QualityButton qualityButton;

    private YTDLVideoPlayer ytdlVideoPlayer;
    private Camera mainCamera;

    private float maxProgressValue;
    private float newProgressX;
    private float maxProgressX;
    private float minProgressX;
    private float progressPosY;
    private float simpleProgressValue;
    private float progressValue;
    private float progressBarWidth;
    private bool isProgressDragging;

    private float maxVolumeValue;
    private float newVolumeX;
    private float maxVolumeX;
    private float minVolumeX;
    private float volumePosY;
    private float simpleVolumeValue;
    private float volumeValue;
    private float volumeBarWidth;

    public bool isVideoJumping { get; set; }

    public bool isVideoPlaying
    {
      get
      {
        return ytdlVideoPlayer.isPlaying;
      }
    }

    public bool isVideoPaused
    {
      get
      {
        return ytdlVideoPlayer.isPaused;
      }
    }

    // Log message format template
    private const string LOG_FORMAT = "[VideoPlayerCtrl] {0}";

    #endregion

    #region Events

    private void OnParseStarted(YTDLPlayerBase player)
    {
      loadingCircle.SetActive(true);
    }

    private void OnPlayerPrepareCompleted(YTDLPlayerBase player)
    {
      // jump video to position
      if (isVideoJumping)
      {
        CalcProgressSimpleValue();
        JumpVideo();
      }
    }

    private void OnPlayerStarted(YTDLPlayerBase player)
    {
      loadingCircle.SetActive(false);
      isVideoJumping = false;
      MediaInfo videoInfo = ytdlVideoPlayer.GetVideoInfo();
      // update video title
      videoTitle.SetText(videoInfo.title);
      // update quality button
      UpdateQualityButton();
      // toggle play button
      playButton.Toggle();
    }

    #endregion

    #region Video Player Ctrl

    public void OnProgressPressDown()
    {
      if (ytdlVideoPlayer.isParsed)
      {
        PauseVideo();
        minProgressX = progressBar.transform.localPosition.x;
        maxProgressX = minProgressX + progressBarWidth;
      }
    }

    public void OnProgressRelease()
    {
      if (ytdlVideoPlayer.isParsed)
      {
        isProgressDragging = false;
        loadingCircle.SetActive(true);
        CalcProgressSimpleValue();
        PlayVideo();
        JumpVideo();
      }
    }

    public void OnProgressDrag()
    {
      if (ytdlVideoPlayer.isParsed)
      {
        isProgressDragging = true;
        isVideoJumping = true;
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 curPosition = mainCamera.ScreenToWorldPoint(curScreenPoint);
        progressCircle.transform.position = new Vector2(curPosition.x, curPosition.y);
        newProgressX = progressCircle.transform.localPosition.x;
        if (newProgressX > maxProgressX) { newProgressX = maxProgressX; }
        if (newProgressX < minProgressX) { newProgressX = minProgressX; }
        progressCircle.transform.localPosition = new Vector2(newProgressX, progressPosY);
        CalcProgressSimpleValue();
        progressBar.transform.localScale = new Vector3(simpleProgressValue * progressBarWidth, progressBar.transform.localScale.y, 0);
      }
    }

    private void CalcProgressSimpleValue()
    {
      maxProgressValue = maxProgressX - minProgressX;
      progressValue = progressCircle.transform.localPosition.x - minProgressX;
      simpleProgressValue = progressValue / maxProgressValue;
    }

    private void JumpVideo()
    {
      int duration = ytdlVideoPlayer.GetVideoInfo().duration;
      double time = (double)duration * simpleProgressValue;
      ytdlVideoPlayer.time = time;
    }

    public void OnVolumePressDown()
    {
      minVolumeX = volumeBar.transform.localPosition.x;
      maxVolumeX = minVolumeX + volumeBarWidth;
    }

    public void OnVolumeRelease()
    {
      CalcVolumeSimpleValue();
    }

    public void OnVolumeDrag()
    {
      float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
      Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
      Vector3 curPosition = mainCamera.ScreenToWorldPoint(curScreenPoint);
      volumeCircle.transform.position = new Vector2(curPosition.x, curPosition.y);
      newVolumeX = volumeCircle.transform.localPosition.x;
      if (newVolumeX > maxVolumeX) { newVolumeX = maxVolumeX; }
      if (newVolumeX < minVolumeX) { newVolumeX = minVolumeX; }
      volumeCircle.transform.localPosition = new Vector2(newVolumeX, volumePosY);
      CalcVolumeSimpleValue();
      volumeBar.transform.localScale = new Vector3(simpleVolumeValue * volumeBarWidth, volumeBar.transform.localScale.y, 0);
    }

    private void CalcVolumeSimpleValue()
    {
      maxVolumeValue = maxVolumeX - minVolumeX;
      volumeValue = volumeCircle.transform.localPosition.x - minVolumeX;
      simpleVolumeValue = volumeValue / maxVolumeValue;
      // set volume value
      ytdlVideoPlayer.SetAudioVolume(0, simpleVolumeValue);
    }

    public void SetVideoParsedUrl(string url)
    {
      ytdlVideoPlayer.SetVideoParsedUrl(url);
    }

    public void PrepareVideo()
    {
      ytdlVideoPlayer.Prepare();
    }

    public void ToggleVideoPlay()
    {
      if (isVideoPlaying)
      {
        PauseVideo();
      }
      else
      {
        PlayVideo();
      }
    }

    public void PauseVideo()
    {
      ytdlVideoPlayer.Pause();
      playButton.Toggle();
    }

    public void PlayVideo()
    {
      ytdlVideoPlayer.Play();
      playButton.Toggle();
    }

    public void StopVideo()
    {
      ytdlVideoPlayer.Stop();
      playButton.Toggle();
    }

    public void ReplayVideo()
    {
      ytdlVideoPlayer.Stop();
      loadingCircle.SetActive(true);
      ytdlVideoPlayer.Play();
    }

    public void VolumeDown()
    {
      float volume = ytdlVideoPlayer.GetAudioVolume(0);
      volume = Mathf.Max(volume - 0.1f, 0f);
      ytdlVideoPlayer.SetAudioVolume(0, volume);
    }

    public void VolumeUp()
    {
      float volume = ytdlVideoPlayer.GetAudioVolume(0);
      volume = Mathf.Min(volume + 0.1f, 1f);
      ytdlVideoPlayer.SetAudioVolume(0, volume);
    }

    public void ToggleQuality()
    {
      if (ytdlVideoPlayer.SupportedVideoFormats.Count <= 0)
        return;
      if (loadingCircle.activeSelf)
        return;
      loadingCircle.SetActive(true);
      isVideoJumping = true;

      int index = ytdlVideoPlayer.formatIndex;
      index = (++index) % ytdlVideoPlayer.SupportedVideoFormats.Count;
      ytdlVideoPlayer.SwitchVideoFormat(index);

      UpdateQualityButton();
    }

    private void UpdateQualityButton()
    {
      int index = ytdlVideoPlayer.formatIndex;
      MediaFormat format = ytdlVideoPlayer.SupportedVideoFormats[index];
      string formatText = format.format_note;
      if (string.IsNullOrEmpty(formatText))
      {
        formatText = string.Format("{0}p", format.height);
      }
      qualityButton.SetText(formatText);
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
      ytdlVideoPlayer = GetComponent<YTDLVideoPlayer>();

      if (videoTitle == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "VideoTitle not attached!");
      }
      if (videoTime == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "VideoTime not attached!");
      }
      if (playButton == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "PlayButton not attached!");
      }
      if (loadingCircle == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "LoadingCircle not attached!");
      }
      if (progressCircle == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "ProgressCircle not attached!");
      }
      if (progressBar == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "ProgressBar not attached!");
      }
      if (progressBarBG == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "ProgressBarBG not attached!");
      }
      if (volumeCircle == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "VolumeCircle not attached!");
      }
      if (volumeBar == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "VolumeBar not attached!");
      }
      if (volumeBarBG == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "VolumeBarBG not attached!");
      }
      if (qualityButton == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "QualityButton not attached!");
      }

      // cache main camera
      mainCamera = Camera.main;

      progressPosY = progressCircle.transform.localPosition.y;
      progressBarWidth = progressBarBG.GetComponent<SpriteRenderer>().bounds.size.x;

      volumePosY = volumeCircle.transform.localPosition.y;
      volumeBarWidth = volumeBarBG.GetComponent<SpriteRenderer>().bounds.size.x;

      minProgressX = progressBar.transform.localPosition.x;
      maxProgressX = minProgressX + progressBarWidth;
    }

    private void Update()
    {
      if (!isProgressDragging && !isVideoJumping && isVideoPlaying)
      {
        int duration = ytdlVideoPlayer.GetVideoInfo().duration;

        // Unity video player bug, time may exceed duration
        // Force stop or loop
        if (ytdlVideoPlayer.time >= duration)
        {
          if (ytdlVideoPlayer.loop)
          {
            ReplayVideo();
          }
          else
          {
            StopVideo();
          }
          return;
        }

        float progress = (float)ytdlVideoPlayer.time / duration;
        progressBar.transform.localScale = new Vector3(progressBarWidth * progress, progressBar.transform.localScale.y, 0);
        progressCircle.transform.localPosition = new Vector2(progressBar.transform.localPosition.x + (progressBarWidth * progress), progressCircle.transform.localPosition.y);

        string timeString = string.Format("{0} / {1}",
          Utils.GetFormatTimeStringFromSeconds(ytdlVideoPlayer.time),
          Utils.GetFormatTimeStringFromSeconds(duration));
        if (!timeString.Equals(videoTime.GetText()))
        {
          videoTime.SetText(timeString);
        }
      }
    }

    private void OnEnable()
    {
      ytdlVideoPlayer.prepareCompleted += OnPlayerPrepareCompleted;
      ytdlVideoPlayer.parseStarted += OnParseStarted;
      ytdlVideoPlayer.started += OnPlayerStarted;
    }

    private void OnDisable()
    {
      ytdlVideoPlayer.prepareCompleted -= OnPlayerPrepareCompleted;
      ytdlVideoPlayer.parseStarted -= OnParseStarted;
      ytdlVideoPlayer.started -= OnPlayerStarted;
    }

    #endregion
  }
}