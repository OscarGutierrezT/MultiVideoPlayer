/* Copyright (c) 2020-present Evereal. All rights reserved. */

namespace Evereal.YoutubeDLPlayer
{
  public static class Constants
  {
    /// <summary>
    /// PlayerPrefs key for current youtube-dl version.
    /// </summary>
    public const string PREF_YTDL_VERSION_KEY = "PREF_YTDL_VERSION_KEY";
    /// <summary>
    /// API request for latest youtube-dl.
    /// </summary>
#if UNITY_ANDROID && !UNITY_EDITOR
    public const string YTDL_LATEST_RELEASE_API = "https://api.github.com/repos/yausername/youtubedl-lazy/releases/latest";
#else
    public const string YTDL_LATEST_RELEASE_API = "https://api.github.com/repos/ytdl-org/youtube-dl/releases/latest";
#endif
    /// <summary>
    /// Android native plugin class.
    /// </summary>
    public const string ANDROID_PLUGIN_CLASS = "com.evereal.youtubedl_android.YoutubeDLPlugin";
    /// <summary>
    /// Default youtube-dl options settings can be played with Unity <c>VideoPlayer</c>.
    /// </summary>
    public const string DEFAULT_YTDL_VIDEO_PARSE_OPTIONS = "--format [protocol=https][ext=mp4]/[protocol=http][ext=mp4] --no-cache-dir";
    /// <summary>
    /// Default low video format
    /// </summary>
    public const string WORST_YTDL_VIDEO_PARSE_OPTIONS = "--format best[height<=480][protocol=https][ext=mp4]/[protocol=http][ext=mp4] --no-cache-dir";
    /// <summary>
    /// Default youtube-dl options settings can be played with Unity <c>AudioSource</c>.
    /// </summary>
    public const string DEFAULT_YTDL_AUDIO_PARSE_OPTIONS = "--format [protocol=https][ext=mp3]/[protocol=http][ext=mp3] --no-cache-dir";
    /// <summary>
    /// Video title length limit in UI.
    /// </summary>
    public const int VIDEO_TITLE_LENGTH_LIMIT = 30;
  }
}