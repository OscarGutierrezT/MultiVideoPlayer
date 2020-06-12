/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class VideoTime : MonoBehaviour
  {

    private TextMesh textMesh;
    private string LOG_FORMAT = "[VideoTime] {0}";

    private void Awake()
    {
      textMesh = GetComponent<TextMesh>();
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

    public string GetText()
    {
      if (textMesh == null)
      {
        Debug.LogWarningFormat(LOG_FORMAT, "TextMesh not attached!");
        return "";

      }
      return textMesh.text;
    }
  }
}
