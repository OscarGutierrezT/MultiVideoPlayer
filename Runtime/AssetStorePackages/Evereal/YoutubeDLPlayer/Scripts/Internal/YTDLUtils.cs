/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System.IO;

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// Youtube DL executable settings.
  /// </summary>
  public static class YTDLUtils
  {
    // The youtube-dl folder
    public static string folder
    {
      get
      {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        return Directory.GetCurrentDirectory() + "/YoutubeDL/";
#else
        return "";
#endif
      }
    }

    // Get youtube-dl executable
    public static string executable
    {
      get
      {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return "youtube-dl.exe";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        return "youtube-dl";
#elif UNITY_ANDROID && !UNITY_EDITOR
        return "youtube_dl.zip";
#else
        return "";
#endif
      }
    }

    // Get youtube-dl path
    public static string path
    {
      get
      {
        return folder + executable;
      }
    }

		public static bool IsExist()
    {
      return File.Exists(path);
    }

		public static void CheckFolder()
		{
			if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }
		}
  }
}