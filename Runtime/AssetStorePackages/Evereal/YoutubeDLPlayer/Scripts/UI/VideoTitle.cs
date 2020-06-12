/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class VideoTitle : MonoBehaviour
  {
    private TextMesh textMesh;
    private string LOG_FORMAT = "[VideoTitle] {0}";

    private void Awake()
    {
      textMesh = GetComponent<TextMesh>();
    }

    public void SetText(string text)
    {
      if (text.Length > Constants.VIDEO_TITLE_LENGTH_LIMIT)
      {
        text = string.Format("{0}...", text.Substring(0, Constants.VIDEO_TITLE_LENGTH_LIMIT));
      }
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
