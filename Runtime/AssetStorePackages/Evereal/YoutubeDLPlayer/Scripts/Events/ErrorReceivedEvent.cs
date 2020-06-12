/* Copyright (c) 2020-present Evereal. All rights reserved. */

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// Invoked when error occurred, such as HTTP connection problems are reported through this callback.
  /// </summary>
  /// <param name="player"><c>YTDLPlayerBase</c> instance.</param>
  /// <param name="error">Error code.</param>
  public delegate void ErrorReceivedEvent(YTDLPlayerBase player, ErrorCode error);
}