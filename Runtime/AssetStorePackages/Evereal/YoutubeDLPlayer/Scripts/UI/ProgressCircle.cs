/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class ProgressCircle : MonoBehaviour
  {
    public VideoPlayerCtrl videoPlayerCtrl;

    private string LOG_FORMAT = "[ProgressCircle] {0}";

    void OnMouseDown()
    {
      if (videoPlayerCtrl != null)
      {
        videoPlayerCtrl.OnProgressPressDown();
      }
      else
      {
        Debug.LogWarningFormat(LOG_FORMAT, "VideoPlayerCtrl not attached!");
      }
    }

    void OnMouseUp()
    {
      if (videoPlayerCtrl != null)
      {
        videoPlayerCtrl.OnProgressRelease();
      }
      else
      {
        Debug.LogWarningFormat(LOG_FORMAT, "VideoPlayerCtrl not attached!");
      }
    }

    void OnMouseDrag()
    {
      if (videoPlayerCtrl != null)
      {
        videoPlayerCtrl.OnProgressDrag();
      }
      else
      {
        Debug.LogWarningFormat(LOG_FORMAT, "VideoPlayerCtrl not attached!");
      }
    }
  }
}