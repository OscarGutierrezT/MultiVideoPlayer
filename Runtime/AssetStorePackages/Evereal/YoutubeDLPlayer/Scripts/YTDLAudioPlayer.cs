/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Evereal.YoutubeDLPlayer
{
  public class YTDLAudioPlayer : YTDLPlayerBase
  {
    #region Properties

    // Unity audio source instance
    private AudioSource audioSource;

    // Whether content is being played.
    public bool isPlaying
    {
      get
      {
        return audioSource.isPlaying;
      }
    }

    #endregion

    #region Events

    // Invoked when <c>YTDLParser</c> parse started.
    public event ParseStartedEvent parseStarted = delegate { };

    // Invoked when <c>YTDLParser</c> parse completed.
    public event ParseCompletedEvent parseCompleted = delegate { };
    private void OnParseCompleted(MediaInfo info)
    {
      mediaInfo = info;

      // extract available audio format
      // ExtractAvailableAudioFormat();

      // download & prepare audio
      StartCoroutine(GetAudioClip(ValidParsedAudioUrl(mediaInfo, mediaInfo.url), autoPlay));

      // parse completed
      isParsed = true;

      parseCompleted(this, mediaInfo);
    }

    // Invoked when error occurred, such as HTTP connection problems are reported through this callback.
    public event ErrorReceivedEvent errorReceived = delegate { };
    private void OnParserErrorReceived(YTDLParser.ErrorEvent error)
    {
      errorReceived(this, error.code);
    }

    #endregion

    #region YTDL Audio Player

    public AudioSource GetAudioPlayer()
    {
      return audioSource;
    }

    public MediaInfo GetAudioInfo()
    {
      return mediaInfo;
    }

    public void SetAudioUrl(string url)
    {
      this.url = url;
      isParsed = false;
    }

    public void Parse(bool auto = true)
    {
      if (string.IsNullOrEmpty(url))
      {
        Debug.LogWarningFormat(LOG_FORMAT, "Please provide the video url!");
        return;
      }
      autoPlay = auto;
      ytdlParser.SetOptions(GetAudioUrlParseOptions(url));
      StartCoroutine(ytdlParser.PrepareAndParse(ValidAudioUrl(url)));
      parseStarted(this);
    }

    public bool Play()
    {
      if (!isParsed)
      {
        Parse(true);
        return false;
      }
      else
      {
        audioSource.Play();
        return true;
      }
    }

    #endregion

    #region Internal

    private IEnumerator GetAudioClip(string url, bool auto)
    {
      using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
      {
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
          Debug.LogErrorFormat(LOG_FORMAT, www.error);
        }
        else
        {
          AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
          if (clip != null)
          {
            audioSource.clip = clip;
            if (auto)
            {
              audioSource.Play();
            }
          }
        }
      }
    }

    IEnumerator LoadAudioClip(string url, bool auto)
    {
      // string path = "file://" + url;
      using (WWW www = new WWW(url))
      {
        yield return www;

        AudioClip clip = www.GetAudioClip(false, true, AudioType.MPEG);
        if (clip != null)
        {
          audioSource.clip = clip;
          if (auto)
          {
            audioSource.Play();
          }
        }
      }
    }

    // private void ExtractAvailableAudioFormat()
    // {
    //   availableMediaFormat.Clear();
    //   // parse valid format for audio source
    //   foreach (MediaFormat format in mediaInfo.formats)
    //   {
    //     // TODO, support other container and protocol
    //     if (
    //       format.ext == "mp3" &&
    //       format.acodec != "none" &&
    //       (format.protocol == "https" || format.protocol == "http")
    //     )
    //     {
    //       availableMediaFormat.Add(format);
    //     }
    //   }
    // }

    private string GetAudioUrlParseOptions(string url)
    {
      return Constants.DEFAULT_YTDL_AUDIO_PARSE_OPTIONS;
    }

    private string ValidAudioUrl(string url)
    {
      return url;
    }

    private string ValidParsedAudioUrl(MediaInfo mediaInfo, string url)
    {
      return url;
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
      // initial youtube-dl parser
      ytdlParser = GetComponent<YTDLParser>();

      // initial audio source
      audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.playOnAwake = false;
      audioSource.loop = loop;
    }

    private void Start()
    {
      if (string.IsNullOrEmpty(url))
      {
        Debug.LogWarningFormat(LOG_FORMAT, "Please provide the audio url!");
        return;
      }
      if (autoPlay)
      {
        Play();
      }
    }

    private void OnEnable()
    {
      // bind ytdl core events
      ytdlParser.parseCompleted += OnParseCompleted;
      ytdlParser.errorReceived += OnParserErrorReceived;
    }

    private void OnDisable()
    {
      ytdlParser.parseCompleted -= OnParseCompleted;
      ytdlParser.errorReceived -= OnParserErrorReceived;
    }

    #endregion
  }
}