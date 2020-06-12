/* Copyright (c) 2020-present Evereal. All rights reserved. */

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// Invoked when a new frame is ready.
  /// </summary>
  /// <param name="player">The <c>YTDLPlayerBase</c> source that is emitting the event.</param>
  /// <param name="frameIdx">The frame the <c>VideoPlayer</c> is now at.</param>
  public delegate void FrameReadyEvent(YTDLPlayerBase player, double frameIdx);
}