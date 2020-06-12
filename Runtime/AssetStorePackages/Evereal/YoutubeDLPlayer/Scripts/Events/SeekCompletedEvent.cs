/* Copyright (c) 2020-present Evereal. All rights reserved. */

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// Invoke after a seek operation completes.
  /// </summary>
  /// <param name="player">The <c>YTDLPlayerBase</c> source that is emitting the event.</param>
  public delegate void SeekCompletedEvent(YTDLPlayerBase player);
}
