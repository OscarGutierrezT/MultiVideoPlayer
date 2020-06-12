/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class PlayButton : MonoBehaviour
  {
    public VideoPlayerCtrl videoPlayerCtrl;

    public GameObject playIcon;
    public GameObject pauseIcon;

    private const string LOG_FORMAT = "[PlayButton] {0}";

    void OnMouseUpAsButton()
    {
      if (videoPlayerCtrl == null)
      {
        Debug.LogWarningFormat(LOG_FORMAT, "VideoPlayerCtrl not attached!");
      }

      videoPlayerCtrl.ToggleVideoPlay();
    }

    public void Toggle()
    {
      if (playIcon == null)
      {
        Debug.LogWarningFormat(LOG_FORMAT, "PlayIcon not attached!");
      }
      if (pauseIcon == null)
      {
        Debug.LogWarningFormat(LOG_FORMAT, "PauseIcon not attached!");
      }

      playIcon.SetActive(!videoPlayerCtrl.isVideoPlaying);
      pauseIcon.SetActive(videoPlayerCtrl.isVideoPlaying);
    }
  }
}
