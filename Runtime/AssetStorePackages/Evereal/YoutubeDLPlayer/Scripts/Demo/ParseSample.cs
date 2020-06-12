/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System.Collections.Generic;
using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class ParseSample : MonoBehaviour
  {
    [Tooltip("The online video URL, e.g. https://www.twitch.tv/tfue. If this URL is offline or not available, you can choose a new one from https://www.twitch.tv/.")]
    public string url = "https://www.twitch.tv/tfue";
    [Tooltip("The custom options for youtube-dl")]
    public string options = "--format best[protocol=m3u8]";
    // Youtube-DL instance
    public YTDLParser ytdlParser;
    // Parsed media info
    private MediaInfo mediaInfo;
    // Parse error message
    private string errorMsg = null;
    // Available video format can be play
    public List<MediaFormat> availableMediaFormat { get; private set; }

    // Log message format template
    private string LOG_FORMAT = "[ParseSample] {0}";

    private void Awake()
    {
      if (ytdlParser == null)
      {
        Debug.LogErrorFormat(LOG_FORMAT, "YTDLParser component is not found!");
      }
      ytdlParser.parseCompleted += ParseCompleted;
      ytdlParser.errorReceived += ErrorReceived;

      // Set your custom parameters
      ytdlParser.SetOptions(options);
    }

    private void ParseCompleted(MediaInfo info)
    {
      mediaInfo = info;

      foreach (var format in info.formats)
      {
        Debug.LogFormat(LOG_FORMAT, string.Format("{0}: {1}", format.format_id, format.url));
      }
    }

    private void ErrorReceived(YTDLParser.ErrorEvent error)
    {
      //Debug.LogErrorFormat(LOG_FORMAT, "Receive error code: " + error.code);
      errorMsg = error.message;
    }

    private void OnGUI()
    {
      GUI.skin.label.fontSize = 24;
      GUI.Label(new Rect(10, 10, Screen.width - 10, 40), "Original URL:");
      GUI.Label(new Rect(10, 50, Screen.width - 10, 40), url);
      if (ytdlParser.status == ProcessingStatus.READY)
      {
        if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 25, 150, 50), "Start Parse"))
        {
          // Start parse
          StartCoroutine(ytdlParser.PrepareAndParse(url));
        }
      }
      else
      {
        if (GUI.Button(new Rect(Screen.width / 2 - 75, Screen.height / 2 - 25, 150, 50), "Status: " + ytdlParser.status)) { }
      }
      if (ytdlParser.status == ProcessingStatus.PARSING)
      {
        GUI.Label(new Rect(10, 90, Screen.width - 10, 40), "Parsing...");
      }
      else
      {
        if (mediaInfo != null)
        {
          string parsedUrl = mediaInfo.url;
          if (parsedUrl.Length > 100)
          {
            parsedUrl = parsedUrl.Substring(0, 100) + "...";
          }
          GUI.Label(new Rect(10, 90, Screen.width - 10, 40), "Parse Result:");
          GUI.Label(new Rect(10, 130, Screen.width - 10, 40), parsedUrl);
        }
        else if (errorMsg != null)
        {
          GUI.Label(new Rect(10, 90, Screen.width - 10, 40), "Parse Error:");
          GUI.Label(new Rect(10, 130, Screen.width - 10, 80), errorMsg);
        }
      }
    }
  }
}