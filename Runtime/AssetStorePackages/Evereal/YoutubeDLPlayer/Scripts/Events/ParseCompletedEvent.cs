/* Copyright (c) 2020-present Evereal. All rights reserved. */

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// Invoked when <c>YTDLParser</c> parse completed.
  /// </summary>
  /// <param name="player">The <c>YTDLPlayerBase</c> source that is emitting the event.</param>
  /// <param name="mediaInfo">The youtube-dl parsed media info.</param>
  public delegate void ParseCompletedEvent(YTDLPlayerBase player, MediaInfo mediaInfo);
}
