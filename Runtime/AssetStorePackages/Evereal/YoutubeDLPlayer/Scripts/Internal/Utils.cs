/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System;

namespace Evereal.YoutubeDLPlayer
{
  public class Utils
  {
    public static string GetFormatTimeStringFromSeconds(double seconds)
    {
      TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
      string formatTime = "00:00";
      if (timeSpan.Hours > 0)
      {
        formatTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
        timeSpan.Hours,
        timeSpan.Minutes,
        timeSpan.Seconds);
      }
      else
      {
        formatTime = string.Format("{0:D2}:{1:D2}",
        timeSpan.Minutes,
        timeSpan.Seconds);
      }
      return formatTime;
    }
  }
}