using RenderHeads.Media.AVProVideo;
using Evereal.YoutubeDLPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ezphera.MultiVideoPlayer
{
    [RequireComponent(typeof(YTDLParser))]
    public class EZLiveVideoStreaming : MonoBehaviour
    {
        [Header("The url live streaming Youtube, Vimeo or Twitch")]
        public string url = "https://www.twitch.tv/tfue";
        [Tooltip("The custom options for youtube-dl")]
        public string options = "--format best[protocol=m3u8]";

        public MediaPlayer mediaPlayer;
        YTDLParser ytdlParser;
        // Parsed media info
        private MediaInfo mediaInfo;
        public bool _autoLoad;
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
            if (mediaPlayer) {
                mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, mediaInfo.url);
            }
            //foreach (var format in info.formats)
            //{
            //    Debug.LogFormat(LOG_FORMAT, string.Format("{0}: {1}", format.format_id, format.url));
            //}
        }

        private void ErrorReceived(YTDLParser.ErrorEvent error)
        {
            //Debug.LogErrorFormat(LOG_FORMAT, "Receive error code: " + error.code);
            //errorMsg = error.message;
        }

    }
}