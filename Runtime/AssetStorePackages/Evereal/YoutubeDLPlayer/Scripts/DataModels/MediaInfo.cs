/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System;

namespace Evereal.YoutubeDLPlayer
{
  [Serializable]
  public class MediaInfo
  {
    public String id;
    public String extractor;
    public String fulltitle;
    public String title;
    public String upload_date;
    public String display_id;
    public int duration;
    public String description;
    public String thumbnail;
    public String license;
    public String view_count;
    public String like_count;
    public String dislike_count;
    public String repost_count;
    public String average_rating;

    public String uploader_id;
    public String uploader;

    public String url;
    public String player_url;
    public String webpage_url;
    public String webpage_url_basename;
    public String resolution;
    public int width;
    public int height;
    public String protocol;
    public String format;
    public String format_note;
    public String ext;
    public long start_time;

    public String[] categories;
    public MediaFormat[] formats;
    public MediaThumbnail[] thumbnails;

    public String manifest_url;

    public String error;
  }
}