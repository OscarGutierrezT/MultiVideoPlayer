/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System.Collections.Generic;
using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  [RequireComponent(typeof(YTDLParser))]
  public class YTDLPlayerBase : MonoBehaviour
  {
    [Tooltip("The online media url, e.g. https://www.youtube.com/watch?v=DDsRfbfnC_A")]
    [SerializeField]
    public string url;
    [Tooltip("Start playback as soon as the game started or new video url loaded.")]
    public bool autoPlay = true;
    [Tooltip("Start video playback at the begining when end is reached.")]
    public bool loop = false;

    // Youtube-DL instance
    protected YTDLParser ytdlParser;
    // Parsed media info
    protected MediaInfo mediaInfo;
    // Available video format can be play
    public List<MediaFormat> availableMediaFormat { get; protected set; }
    // If the youtube-dl already parse the video url
    public bool isParsed { get; protected set; }

    protected string LOG_FORMAT = "[YTDLPlayer] {0}";
  }
}