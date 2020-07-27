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
    /// e.g. --format [protocol=https][ext=mp4]/[protocol=http][ext=mp4]
    /// </summary>
    public const string DEFAULT_YTDL_VIDEO_PARSE_OPTIONS = "--no-cache-dir --no-warnings";
    /// <summary>
    /// Default youtube-dl options settings can be played with Unity <c>AudioSource</c>.
    /// e.g. --format [protocol=https][ext=mp3]/[protocol=http][ext=mp3]
    /// </summary>
    public const string DEFAULT_YTDL_AUDIO_PARSE_OPTIONS = "--no-cache-dir --no-warnings";
    /// <summary>
    /// Video title length limit in UI.
    /// </summary>
    public const int VIDEO_TITLE_LENGTH_LIMIT = 30;
  }
}