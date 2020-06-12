/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System;

namespace Evereal.YoutubeDLPlayer
{
  [Serializable]
  public class ReleaseInfo
  {
    public string tag_name;

    [Serializable]
    public class AssetInfo
    {
      public string name;
      public string browser_download_url;
    }
    public AssetInfo[] assets;
  }
}