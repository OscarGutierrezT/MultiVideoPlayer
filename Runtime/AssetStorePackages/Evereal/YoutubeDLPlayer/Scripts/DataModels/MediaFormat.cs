/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System;

namespace Evereal.YoutubeDLPlayer
{
  [Serializable]
  public class MediaFormat
  {
    public int asr;
    public int tbr;
    public int abr;
    public String protocol;
    public String format;
    public String format_id;
    public String format_note;
    public String ext;
    public int preference;
    public String vcodec;
    public String acodec;
    public int width;
    public int height;
    public long filesize;
    public int fps;
    public String url;
    public String manifest_url;
  }
}