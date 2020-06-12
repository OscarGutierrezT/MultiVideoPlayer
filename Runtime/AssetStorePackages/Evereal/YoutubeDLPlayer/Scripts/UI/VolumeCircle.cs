/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class VolumeCircle : MonoBehaviour
  {
    public VideoPlayerCtrl videoPlayerCtrl;

    private string LOG_FORMAT = "[VolumeCircle] {0}";

    void OnMouseDown()
    {
      if (videoPlayerCtrl != null)
      {
        videoPlayerCtrl.OnVolumePressDown();
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
        videoPlayerCtrl.OnVolumeRelease();
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
        videoPlayerCtrl.OnVolumeDrag();
      }
      else
      {
        Debug.LogWarningFormat(LOG_FORMAT, "VideoPlayerCtrl not attached!");
      }
    }
  }
}