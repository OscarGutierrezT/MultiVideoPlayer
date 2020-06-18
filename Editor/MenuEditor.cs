/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;
using UnityEditor;

namespace Evereal.YoutubeDLPlayer
{
  public class MenuEditor : MonoBehaviour
  {
    [MenuItem("Tools/Evereal/YoutubeDLPlayer/GameObject/YTDLVideoPlayer")]
    private static void CreateYTDLVideoPlayerObject(MenuCommand menuCommand)
    {
      GameObject ytdlVideoPlayerPrefab = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/YTDLVideoPlayer")) as GameObject;
      ytdlVideoPlayerPrefab.name = "YTDLVideoPlayer";
      GameObjectUtility.SetParentAndAlign(ytdlVideoPlayerPrefab, menuCommand.context as GameObject);
      Undo.RegisterCreatedObjectUndo(ytdlVideoPlayerPrefab, "Create " + ytdlVideoPlayerPrefab.name);
      Selection.activeObject = ytdlVideoPlayerPrefab;
    }

		[MenuItem("Tools/Evereal/YoutubeDLPlayer/GameObject/YTDLVideoPlayer (360)")]
    private static void CreateYTDLVideoPlayer360Object(MenuCommand menuCommand)
    {
      GameObject ytdlVideoPlayer360Prefab = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/YTDLVideoPlayer_360")) as GameObject;
      ytdlVideoPlayer360Prefab.name = "YTDLVideoPlayer_360";
      GameObjectUtility.SetParentAndAlign(ytdlVideoPlayer360Prefab, menuCommand.context as GameObject);
      Undo.RegisterCreatedObjectUndo(ytdlVideoPlayer360Prefab, "Create " + ytdlVideoPlayer360Prefab.name);
      Selection.activeObject = ytdlVideoPlayer360Prefab;
    }

		[MenuItem("Tools/Evereal/YoutubeDLPlayer/GameObject/YTDLVideoPlayer (Full Screen)")]
    private static void CreateYTDLVideoPlayerFullScreenObject(MenuCommand menuCommand)
    {
      GameObject ytdlVideoPlayerFullScreenPrefab = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/YTDLVideoPlayer_FullScreen")) as GameObject;
      ytdlVideoPlayerFullScreenPrefab.name = "YTDLVideoPlayer_FullScreen";
      GameObjectUtility.SetParentAndAlign(ytdlVideoPlayerFullScreenPrefab, menuCommand.context as GameObject);
      Undo.RegisterCreatedObjectUndo(ytdlVideoPlayerFullScreenPrefab, "Create " + ytdlVideoPlayerFullScreenPrefab.name);
      Selection.activeObject = ytdlVideoPlayerFullScreenPrefab;
    }

		[MenuItem("Tools/Evereal/YoutubeDLPlayer/GameObject/YTDLVideoPlayer (UI)")]
    private static void CreateYTDLVideoPlayerUIObject(MenuCommand menuCommand)
    {
      GameObject ytdlVideoPlayerUIPrefab = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/YTDLVideoPlayer_UI")) as GameObject;
      ytdlVideoPlayerUIPrefab.name = "YTDLVideoPlayer_UI";
      GameObjectUtility.SetParentAndAlign(ytdlVideoPlayerUIPrefab, menuCommand.context as GameObject);
      Undo.RegisterCreatedObjectUndo(ytdlVideoPlayerUIPrefab, "Create " + ytdlVideoPlayerUIPrefab.name);
      Selection.activeObject = ytdlVideoPlayerUIPrefab;
    }

		[MenuItem("Tools/Evereal/YoutubeDLPlayer/GameObject/YTDLAudioPlayer")]
    private static void CreateYTDLAudioPlayerObject(MenuCommand menuCommand)
    {
      GameObject ytdlAudioPlayerPrefab = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/YTDLAudioPlayer")) as GameObject;
      ytdlAudioPlayerPrefab.name = "YTDLAudioPlayer";
      GameObjectUtility.SetParentAndAlign(ytdlAudioPlayerPrefab, menuCommand.context as GameObject);
      Undo.RegisterCreatedObjectUndo(ytdlAudioPlayerPrefab, "Create " + ytdlAudioPlayerPrefab.name);
      Selection.activeObject = ytdlAudioPlayerPrefab;
    }

    [MenuItem("Tools/Evereal/YoutubeDLPlayer/GameObject/YTDLParser")]
    private static void CreateYTDLParserObject(MenuCommand menuCommand)
    {
      GameObject ytdlParserPrefab = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/YTDLParser")) as GameObject;
      ytdlParserPrefab.name = "YTDLParser";
      GameObjectUtility.SetParentAndAlign(ytdlParserPrefab, menuCommand.context as GameObject);
      Undo.RegisterCreatedObjectUndo(ytdlParserPrefab, "Create " + ytdlParserPrefab.name);
      Selection.activeObject = ytdlParserPrefab;
    }
  }
}