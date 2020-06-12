/* Copyright (c) 2020-present Evereal. All rights reserved. */

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// The error code of <c>YTDLParser</c> module.
  /// </summary>
  public enum ErrorCode
  {
    FETCH_RELEASE_API_FAILED,
    UPDATE_LIB_FAILED,
    GRANT_LIB_PERMISSION_FAILED,
    PARSE_FAILED,
    PARSE_EXCEPTION,
    PLAY_VIDEO_FAILED,
    ANDROID_PLUGIN_ERROR,
  }

  /// <summary>
  /// The processing status of <c>YTDLParser</c> module.
  /// </summary>
  public enum ProcessingStatus
  {
    READY,
    UPDATING,
    PARSING,
    ERROR,
  }

  /// <summary>
  /// The render type of <c>YTDLVideoPlayer</c> module.
  /// </summary>
  public enum RenderType
  {
    MATERIAL,
    SCREEN,
  }

  // public enum QualityType
  // {
  //   Auto, // Decide video quality automatically.
  //   Highest, // Select the highest video quality.
  //   Lowest, // Select the lowest video quality.
  // }
}