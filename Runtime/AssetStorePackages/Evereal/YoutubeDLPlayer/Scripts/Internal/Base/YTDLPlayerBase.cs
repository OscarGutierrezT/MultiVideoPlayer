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
    protected List<MediaFormat> supportedVideoFormats = new List<MediaFormat>();
    public List<MediaFormat> SupportedVideoFormats
    {
      get
      {
        return supportedVideoFormats;
      }
    }
    // Available audio format can be play
    public List<MediaFormat> supportedAudioFormats = new List<MediaFormat>();
    public List<MediaFormat> SupportedAudioFormats
    {
      get
      {
        return supportedAudioFormats;
      }
    }
    // If the youtube-dl already parse the video url
    public bool isParsed { get; protected set; }

    protected string LOG_FORMAT = "[YTDLPlayer] {0}";

    protected void ExtractSupportedMediaFormats(MediaInfo mediaInfo)
    {
      supportedVideoFormats.Clear();
      supportedAudioFormats.Clear();
      foreach (MediaFormat format in mediaInfo.formats)
      {
        // Unity Video Player
        // TODO, currently only support h.264 with mp4, can better support by target platform
        // https://docs.unity3d.com/ScriptReference/Video.VideoPlayer.html
        if (format.vcodec != "none" &&
          format.ext == "mp4" &&
          (format.protocol == "https" || format.protocol == "http"))
        {
          bool inserted = false;
          if (format.acodec != "none")
          {
            // video/audio together
            for (int i = 0; i < supportedVideoFormats.Count; i++)
            {
              // insert format based on video size
              if (format.height == supportedVideoFormats[i].height)
              {
                // replace
                supportedVideoFormats[i] = format;
                inserted = true;
                break;
              }
              else if (format.height < supportedVideoFormats[i].height)
              {
                // insert
                supportedVideoFormats.Insert(i, format);
                inserted = true;
                break;
              }
            }
          }
          else
          {
            // if (Application.platform == RuntimePlatform.Android)
            // {
            //   continue;
            // }
            // video only
            for (int i = 0; i < supportedVideoFormats.Count; i++)
            {
              // check duplicated
              if (format.height == supportedVideoFormats[i].height)
              {
                if (format.vcodec != null && format.vcodec.StartsWith("avc"))
                {
                  // replace
                  supportedVideoFormats[i] = format;
                }
                inserted = true;
                break;
              }
              else if (format.height < supportedVideoFormats[i].height)
              {
                // insert
                supportedVideoFormats.Insert(i, format);
                inserted = true;
                break;
              }
            }
          }
          if (!inserted)
          {
            // add to last
            supportedVideoFormats.Add(format);
          }
        }
        else if (format.acodec != "none" &&
          (format.ext == "mp3" || format.ext == "wav") && // mp3 only supported in android
          (format.protocol == "https" || format.protocol == "http"))
        {
          // audio only
          supportedAudioFormats.Add(format);
        }
      }
    }
  }
}