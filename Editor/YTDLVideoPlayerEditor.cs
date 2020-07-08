/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Evereal.YoutubeDLPlayer
{
  /// <summary>
  /// Inspector editor script for <c>YTDLVideoPlayer</c> component.
  /// </summary>
  [CustomEditor(typeof(YTDLVideoPlayer))]
  public class YTDLVideoPlayerEditor : Editor
  {
    YTDLVideoPlayer ytdlVideoPlayer;
    SerializedProperty videoRenderer;
    SerializedProperty targetCamera;
    SerializedProperty videoQuality;

    public void OnEnable()
    {
      ytdlVideoPlayer = (YTDLVideoPlayer)target;

      videoRenderer = serializedObject.FindProperty("videoRenderer");
      targetCamera = serializedObject.FindProperty("targetCamera");
      videoQuality = serializedObject.FindProperty("videoQuality");
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      ytdlVideoPlayer.url = EditorGUILayout.TextField("URL", ytdlVideoPlayer.url);
      ytdlVideoPlayer.autoPlay = EditorGUILayout.Toggle("Auto Play", ytdlVideoPlayer.autoPlay);
      ytdlVideoPlayer.loop = EditorGUILayout.Toggle("Loop", ytdlVideoPlayer.loop);
      ytdlVideoPlayer.renderType = (RenderType)EditorGUILayout.EnumPopup("Render Type", ytdlVideoPlayer.renderType);
      ytdlVideoPlayer.videoQuality = (YTDLVideoPlayer.EZVideoQuality)EditorGUILayout.EnumPopup("Max Quality", ytdlVideoPlayer.videoQuality);
      if (ytdlVideoPlayer.renderType == RenderType.MATERIAL)
      {
        EditorGUILayout.PropertyField(videoRenderer, new GUIContent("Video Renderer"), true);
      }
      else if (ytdlVideoPlayer.renderType == RenderType.SCREEN)
      {
        EditorGUILayout.PropertyField(targetCamera, new GUIContent("Target Camera"), true);
      }

      if (GUI.changed)
      {
        EditorUtility.SetDirty(target);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
      }

            if (ytdlVideoPlayer.videoQuality == YTDLVideoPlayer.EZVideoQuality.Best) 
            {
                EditorGUILayout.HelpBox("Se verá muy bonito y todo pero quizás a los clientes ni les corra, asegurate de escoger la mejor opción", MessageType.Warning);
            }

      // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
      serializedObject.ApplyModifiedProperties();
    }
  }
}