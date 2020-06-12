/* Copyright (c) 2020-present Evereal. All rights reserved. */

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// Invoked when the <c>VideoPlayer</c> clock is synced back to its VideoTimeReference.
  /// </summary>
  /// <param name="player">The <c>YTDLPlayerBase</c> source that is emitting the event.</param>
  /// <param name="seconds">The xc.</param>
  public delegate void ClockResyncOccurredEvent(YTDLPlayerBase player, double seconds);
}
