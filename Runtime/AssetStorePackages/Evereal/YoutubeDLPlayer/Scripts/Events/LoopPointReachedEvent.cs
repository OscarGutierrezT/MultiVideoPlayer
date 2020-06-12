/* Copyright (c) 2020-present Evereal. All rights reserved. */

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// Invoked when the <c>VideoPlayer</c> reaches the end of the content to play.
  /// </summary>
  /// <param name="player">The <c>YTDLPlayerBase</c> source that is emitting the event.</param>
  public delegate void LoopPointReachedEvent(YTDLPlayerBase player);
}
