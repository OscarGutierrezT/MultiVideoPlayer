/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class ReplayButton : MonoBehaviour
  {
    public VideoPlayerCtrl videoPlayerCtrl;

    private string LOG_FORMAT = "[ReplayButton] {0}";

    // Use this for initialization
    void OnMouseUpAsButton()
    {
      if (videoPlayerCtrl != null)
      {
        videoPlayerCtrl.ReplayVideo();
      }
      else
      {
        Debug.LogWarningFormat(LOG_FORMAT, "VideoPlayerCtrl not attached!");
      }
    }
  }
}