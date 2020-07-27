/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

using Debug = UnityEngine.Debug;

namespace Evereal.YoutubeDLPlayer
{
  public class YTDLParser : MonoBehaviour
  {
    #region Properties

    /// <summary>
    /// Get or set the current status.
    /// </summary>
    /// <value>The current status.</value>
    public ProcessingStatus status { get; protected set; }
    /// <summary>
    /// Custom parse options for youtube-dl.
    /// See: https://github.com/ytdl-org/youtube-dl#options
    /// </summary>
    private string options = null;
    /// <summary>
    /// Whether the youtube-dl is prepared for parsing.
    /// </summary>
    public bool isPrepared { get; private set; }
    // The youtube-dl parsing thread
    private Thread parsingThread;
    // The youtube-dl update parsing thread
    private Thread updateParsingThread;
    private Queue<MediaInfo> parseCompletedQueue;
    private Queue<ErrorEvent> errorQueue;
    private Queue<string> versionQueue;

    /// <summary>
    /// Android native plugin resources.
    /// </summary>
    private AndroidJavaClass androidPluginClass;
    private AndroidJavaObject androidPluginInstance;
    private AndroidJavaClass androidActivityClass;
    private AndroidJavaObject androidActivityContext;

    // Log message format template
    private string LOG_FORMAT = "[YTDLParser] {0}";

    #endregion

    #region Events

    public struct ErrorEvent
    {
      public ErrorEvent(ErrorCode c, String m)
      {
        code = c;
        message = m;
      }
      public ErrorCode code;
      public String message;
    }
    /// <summary>
    /// Invoked when <c>YTDLParser</c> parse completed.
    /// </summary>
    /// <param name="mediaInfo">The youtube-dl parsed media info.</param>
    public delegate void ParseCompletedEvent(MediaInfo mediaInfo);
    /// <summary>
    /// Invoked when error occurred, such as HTTP connection problems are reported through this callback.
    /// </summary>
    /// <param name="error">Error event contains code and message.</param>
    public delegate void ErrorReceivedEvent(ErrorEvent error);

    // Invoked when parse completed
    public event ParseCompletedEvent parseCompleted = delegate { };
    // Invoked when error occurred
    public event ErrorReceivedEvent errorReceived = delegate { };

    #endregion

    #region YTDL Parser

    public void SetOptions(string options)
    {
      this.options = options;
    }

    public IEnumerator PrepareAndParse(string url)
    {
      // reset error status
      if (status == ProcessingStatus.ERROR)
        status = ProcessingStatus.READY;
      if (status != ProcessingStatus.READY)
      {
        Debug.LogWarningFormat(LOG_FORMAT, "Previous process not finished yet!");
        yield break;
      }
      if (isPrepared)
      {
        // start parse
        ParseAsync(url);
        yield break;
      }
      // fetch ytdl latest version.
      string latestVersion = "";
      string latestDownloadUrl = "";
      status = ProcessingStatus.UPDATING;
      using (UnityWebRequest www = UnityWebRequest.Get(Constants.YTDL_LATEST_RELEASE_API))
      {
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
          errorReceived(new ErrorEvent(ErrorCode.FETCH_RELEASE_API_FAILED, www.error));
          status = ProcessingStatus.ERROR;
          yield break;
        }
        else
        {
          ReleaseInfo releaseInfo = JsonUtility.FromJson<ReleaseInfo>(www.downloadHandler.text);
          latestVersion = releaseInfo.tag_name;
          foreach (ReleaseInfo.AssetInfo asset in releaseInfo.assets)
          {
            if (asset.name == YTDLUtils.executable)
            {
              latestDownloadUrl = asset.browser_download_url;
            }
          }
        }
      }
      // get current ytdl version
      string version = PlayerPrefs.GetString(Constants.PREF_YTDL_VERSION_KEY);

      if (Application.platform == RuntimePlatform.Android)
      {
        #region ANDROID PREPARE SECTION

        // check ytdl version
        if (string.IsNullOrEmpty(version) || version != latestVersion)
        {
          // update and start parse
          UpdateAndParseAsync(latestVersion, latestDownloadUrl, url);
        }
        else
        {
          isPrepared = true;
          // start parse
          ParseAsync(url);
        }

        #endregion
      }
      else
      {
        #region STANDALONE & EDITOR PREPARE SECTION

        // check ytdl version
        if (string.IsNullOrEmpty(version) || version != latestVersion || !File.Exists(YTDLUtils.path))
        {
          // download ytdl latest version
          using (UnityWebRequest www = UnityWebRequest.Get(latestDownloadUrl))
          {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
              errorReceived(new ErrorEvent(ErrorCode.FETCH_RELEASE_API_FAILED, www.error));
              status = ProcessingStatus.ERROR;
              yield break;
            }
            else
            {
              YTDLUtils.CheckFolder();
              System.IO.File.WriteAllBytes(YTDLUtils.path, www.downloadHandler.data);
              Debug.LogFormat(LOG_FORMAT, "Update youtube-dl package success!");
              if (Application.platform == RuntimePlatform.OSXEditor ||
                  Application.platform == RuntimePlatform.OSXPlayer)
              {
                if (Command.Run("chmod", "a+rx " + "\"" + YTDLUtils.path + "\""))
                {
                  Debug.LogFormat(LOG_FORMAT, "Grant youtube-dl permission success!");
                }
                else
                {
                  errorReceived(new ErrorEvent(ErrorCode.GRANT_LIB_PERMISSION_FAILED, "Grant youtube-dl permission failed!"));
                  yield break;
                }
              }
            }
          }
          Debug.LogFormat(LOG_FORMAT, "Update youtube-dl to latest version " + latestVersion);
          // update pref version
          PlayerPrefs.SetString(Constants.PREF_YTDL_VERSION_KEY, latestVersion);
        }
        isPrepared = true;
        // start parse
        ParseAsync(url);

        #endregion
      }
    }

    #endregion

    #region Internal

    private void UpdateAndParseAsync(string version, string downloadUrl, string url)
    {
      if (updateParsingThread != null)
      {
        updateParsingThread.Abort();
      }
      updateParsingThread = new Thread(() => UpdateAndParse(version, downloadUrl, url));
      updateParsingThread.Priority = System.Threading.ThreadPriority.Lowest;
      updateParsingThread.IsBackground = true;
      // update and start parsing thread
      updateParsingThread.Start();
    }

    private void UpdateAndParse(string version, string downloadUrl, string url)
    {
      if (Application.platform == RuntimePlatform.Android)
      {
        #region ANDROID UPDATE & PARSE SECTION

        if (AndroidJNI.AttachCurrentThread() != 0)
        {
          EnqueueError(new ErrorEvent(ErrorCode.ANDROID_PLUGIN_ERROR, "Android attach current thread failed!"));
          return;
        }

        if (!androidPluginInstance.Call<bool>("update", androidActivityContext, downloadUrl))
        {
          EnqueueError(new ErrorEvent(ErrorCode.UPDATE_LIB_FAILED, "Android update youtube-dl lib failed!"));
          return;
        }
        // enqueue version and process in main thread
        versionQueue.Enqueue(version);

        isPrepared = true;

        status = ProcessingStatus.PARSING;

        options = ValidParseOptions(options);

        string output = androidPluginInstance.Call<string>("getVideoInfo", url, options);
        MediaInfo info = JsonUtility.FromJson<MediaInfo>(output);
        if (!string.IsNullOrEmpty(info.error))
        {
          EnqueueError(new ErrorEvent(ErrorCode.PARSE_FAILED, info.error));
          return;
        }
        lock (parseCompletedQueue) parseCompletedQueue.Enqueue(info);

        // done
        status = ProcessingStatus.READY;

        AndroidJNI.DetachCurrentThread();

        #endregion

      }
    }

    private void ParseAsync(string url)
    {
      if (parsingThread != null)
      {
        parsingThread.Abort();
      }
      parsingThread = new Thread(() => Parse(url));
      parsingThread.Priority = System.Threading.ThreadPriority.Lowest;
      parsingThread.IsBackground = true;
      // start parsing thread
      parsingThread.Start();
    }

    private void Parse(string url)
    {
      if (!isPrepared)
      {
        Debug.LogWarningFormat(LOG_FORMAT, "Youtube-dl is not ready for parsing yet!");
        return;
      }
      status = ProcessingStatus.PARSING;

      options = ValidParseOptions(options);

      if (Application.platform == RuntimePlatform.Android)
      {
        #region ANDROID PARSE SECTION

        if (AndroidJNI.AttachCurrentThread() != 0)
        {
          EnqueueError(new ErrorEvent(ErrorCode.ANDROID_PLUGIN_ERROR, "Android attach current thread failed!"));
          return;
        }

        string output = androidPluginInstance.Call<string>("getVideoInfo", url, options);
        MediaInfo info = JsonUtility.FromJson<MediaInfo>(output);
        if (!string.IsNullOrEmpty(info.error))
        {
          EnqueueError(new ErrorEvent(ErrorCode.PARSE_FAILED, info.error));
          return;
        }
        lock (parseCompletedQueue) parseCompletedQueue.Enqueue(info);

        // done
        status = ProcessingStatus.READY;

        AndroidJNI.DetachCurrentThread();

        #endregion
      }
      else
      {
        #region STANDALONE & EDITOR PARSE SECTION

        // parse video url
        try
        {
          string arguments = string.Format(" {0} --dump-json {1}", options, url);

          Process process = new Process();
          process.StartInfo.FileName = YTDLUtils.path;
          process.StartInfo.Arguments = arguments;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.RedirectStandardError = true;
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.UseShellExecute = false;

          process.Start();

          string output = process.StandardOutput.ReadToEnd().Trim();
          string error = process.StandardError.ReadToEnd().Trim();

          process.WaitForExit();
          process.Close();

          if (!string.IsNullOrEmpty(error))
          {
            EnqueueError(new ErrorEvent(ErrorCode.PARSE_FAILED, error));
            return;
          }

          Debug.LogFormat(LOG_FORMAT, "Success getting media resources!");
          MediaInfo info = JsonUtility.FromJson<MediaInfo>(output);

          lock (parseCompletedQueue) parseCompletedQueue.Enqueue(info);
        }
        catch (Exception e)
        {
          EnqueueError(new ErrorEvent(ErrorCode.PARSE_EXCEPTION, e.Message));
          return;
        }

        // done
        status = ProcessingStatus.READY;

        #endregion
      }
    }

    private string ValidParseOptions(string options)
    {
      if (options == null)
      {
        // default to unity video player option
        return Constants.DEFAULT_YTDL_VIDEO_PARSE_OPTIONS;
      }
      return options;
    }

    private void EnqueueError(ErrorEvent error)
    {
      lock (errorQueue) errorQueue.Enqueue(error);
      status = ProcessingStatus.ERROR;
    }

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
      status = ProcessingStatus.READY;

      isPrepared = false;

      parseCompletedQueue = new Queue<MediaInfo>();
      errorQueue = new Queue<ErrorEvent>();
      versionQueue = new Queue<string>();

      if (Application.platform == RuntimePlatform.Android)
      {
        if (androidPluginClass == null)
        {
          androidPluginClass = new AndroidJavaClass(Constants.ANDROID_PLUGIN_CLASS);
        }
        if (androidPluginInstance == null)
        {
          androidPluginInstance = androidPluginClass.CallStatic<AndroidJavaObject>("getInstance");
        }
        if (androidActivityClass == null)
        {
          androidActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        }
        if (androidActivityContext == null)
        {
          androidActivityContext = androidActivityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        androidPluginInstance.Call("init", androidActivityContext);
      }
    }

    private void Update()
    {
      if (parseCompletedQueue.Count > 0)
      {
        MediaInfo videoInfo;
        lock (parseCompletedQueue) videoInfo = parseCompletedQueue.Dequeue();
        parseCompleted(videoInfo);
      }
      if (errorQueue.Count > 0)
      {
        ErrorEvent error;
        lock (errorQueue) error = errorQueue.Dequeue();
        errorReceived(error);
      }
      if (versionQueue.Count > 0)
      {
        string version;
        lock (versionQueue) version = versionQueue.Dequeue();
        // update pref version
        PlayerPrefs.SetString(Constants.PREF_YTDL_VERSION_KEY, version);
      }
    }

    #endregion
  }
}