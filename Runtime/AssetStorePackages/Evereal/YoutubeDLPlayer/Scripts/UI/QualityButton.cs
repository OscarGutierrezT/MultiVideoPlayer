/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class QualityButton : MonoBehaviour
  {
    public VideoPlayerCtrl videoPlayerCtrl;

    private TextMesh textMesh;
    private string LOG_FORMAT = "[QualityButton] {0}";

    private void Awake()
    {
      textMesh = GetComponent<TextMesh>();
    }

    void OnMouseUpAsButton()
    {
      if (videoPlayerCtrl != null)
      {
        videoPlayerCtrl.ToggleQuality();
      }
      else
      {
        Debug.LogWarningFormat(LOG_FORMAT, "VideoPlayerCtrl not attached!");
      }
    }

    public void SetText(string text)
    {
      if (textMesh != null)
      {
        textMesh.text = text;
      }
      else
      {
        Debug.LogWarningFormat(LOG_FORMAT, "TextMesh not attached!");
      }
    }
  }
}
