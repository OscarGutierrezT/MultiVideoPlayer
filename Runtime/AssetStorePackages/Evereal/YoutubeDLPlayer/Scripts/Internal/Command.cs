/* Copyright (c) 2020-present Evereal. All rights reserved. */

using System;
using System.Diagnostics;

namespace Evereal.YoutubeDLPlayer
{
  public class Command
  {
    public static bool Run(string procName, string arguments)
    {
      try
      {
        Process process = new Process();
        process.StartInfo.FileName = procName;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.RedirectStandardError = false;
        process.StartInfo.RedirectStandardOutput = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        process.WaitForExit();
        process.Close();
      }
      catch (Exception e)
      {
        UnityEngine.Debug.LogError(e.Message);
        return false;
      }
      return true;
    }

    public static bool RunDebug(string procName, string arguments)
    {
      try
      {
        Process process = new Process();
        process.StartInfo.FileName = procName;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());
        UnityEngine.Debug.Log(process.StandardError.ReadToEnd());
        process.WaitForExit();
        process.Close();
      }
      catch (Exception e)
      {
        UnityEngine.Debug.LogError(e.Message);
        return false;
      }
      return true;
    }
  }
}