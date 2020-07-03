using RenderHeads.Media.AVProVideo;
using Evereal.YoutubeDLPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ezphera.MultiVideoPlayer
{
    [RequireComponent(typeof(YTDLParser))]
    public class EZLiveVideoStreaming : MonoBehaviour
    {
        [Header("The url live streaming Youtube, Vimeo or Twitch")]
        public string url = "https://www.twitch.tv/tfue";
        [Tooltip("The custom options for youtube-dl")]
        public string options = "--format worst[protocol=m3u8]";

        public MediaPlayer mediaPlayer;
        YTDLParser ytdlParser;
        // Parsed media info
        private MediaInfo mediaInfo;
        public bool _autoLoad;
        public Action<string, bool> OnParceUrl;
        private void Awake()
        {
            ytdlParser = GetComponent<YTDLParser>();
            ytdlParser.SetOptions(options);
        }
        private void Start()
        {
            if (_autoLoad)
            {
                StartCoroutine(ytdlParser.PrepareAndParse(url));
            }
        }
        private void OnEnable()
        {
            ytdlParser.parseCompleted += ParseCompleted;
            ytdlParser.errorReceived += ErrorReceived;
        }
        private void OnDisable()
        {
            ytdlParser.parseCompleted -= ParseCompleted;
            ytdlParser.errorReceived -= ErrorReceived;
        }

        private void ParseCompleted(MediaInfo info)
        {
            mediaInfo = info;
            if (mediaPlayer)
            {
                mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, mediaInfo.url);
                if (!string.IsNullOrEmpty(mediaInfo.url))
                {
                    OnParceUrl(mediaInfo.url, true);
                }
                else
                {
                    OnParceUrl(mediaInfo.url, false);
                }
            }
            else 
            {
                OnParceUrl(mediaInfo.url, false);
            }
            //foreach (var format in info.formats)
            //{
            //    Debug.LogFormat(LOG_FORMAT, string.Format("{0}: {1}", format.format_id, format.url));
            //}
        }
        private void ErrorReceived(YTDLParser.ErrorEvent error)
        {
            //errorMsg = error.message;
        }

    }
}