/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System;
using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class Readme : ScriptableObject
  {
    public Texture2D icon;
    public float iconMaxWidth = 128f;
    public string title;
    public Section[] sections;
    public bool loadedLayout;

    [Serializable]
    public class Section
    {
        public string heading;
        [TextArea]
        public string text;
        public string linkText, url;
    }
  }
}