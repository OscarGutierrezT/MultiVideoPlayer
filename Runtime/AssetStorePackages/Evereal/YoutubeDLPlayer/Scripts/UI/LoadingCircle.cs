/* Copyright (c) 2020-present Evereal. All rights reserved. */

using UnityEngine;

namespace Evereal.YoutubeDLPlayer
{
  public class LoadingCircle : MonoBehaviour
  {
    private float rotateSpeed = 200f;

    private void Update()
    {
      transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
      // rectComponent.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
  }
}