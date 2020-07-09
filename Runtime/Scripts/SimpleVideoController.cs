using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
//using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using JetBrains.Annotations;

namespace Ezphera.MultiVideoPlayer
{
    //[RequireComponent(typeof(PhotonView))]
    public class SimpleVideoController : MonoBehaviour/*PunCallbacks*/
    {
        public VideoPlayer videoPlayer;
        //public CanvasGroup canvasGroup;
        //public GameObject controledByMsn;
        public Canvas canvasConsole;
        //public InputField urlVideo = null;
        bool cameraNull = false;
        [SerializeField] VideoPlayerProgressBar videoPlayerProgressBar;
        public Slider volumenSlider;
        public Slider speedSlider;
        bool activateSliderVolumen = false;
        bool activateSliderSpeed = false;
        public Text currenTime;
        public Text totalTime;
        [Tooltip("Require Component Photon View")]
        public bool Sync = false;
        public bool playListVideos = false;
        public List<string> videosUrls;
        public InputField videoUrlInputField;
        int index = 0;

        public AudioSource audioSource;

        private void Awake()
        {
            //photonView = GetComponent<PhotonView>();
            //controledByMsn.SetActive(false);

            if (Sync)
            {
                videoPlayerProgressBar.Sync = true;
            }
            else
            {
                videoPlayerProgressBar.Sync = false;
            }
            cameraNull = false;
            activateSliderSpeed = false;
            activateSliderVolumen = false;
            if (videoUrlInputField) videoUrlInputField.text = "";
        }
        private void LateUpdate()
        {
            if (!cameraNull)
            {
                ChangeCamera();
                cameraNull = true;
            }
            long playerFrameCount = Convert.ToInt64(videoPlayer.frameCount);
            if (videoPlayer.frame == playerFrameCount)
            {
                if (playListVideos)
                {
                    if (videosUrls[index] != null) videoPlayer.url = videosUrls[index];
                    index++;
                }
            }
            if (videoPlayer.isPlaying)
            {
                float _totalTime = (videoPlayer.frameCount / videoPlayer.frameRate);
                totalTime.text = GetTimeText(_totalTime);
                currenTime.text = GetTimeText(_totalTime - (float)videoPlayer.clockTime);
            }
            
        }
        //public override void OnConnectedToMaster()
        //{
        //    base.OnConnectedToMaster();
        //}
        //public override void OnJoinedRoom()
        //{
        //    base.OnJoinedRoom();
        //}

        public void ChangeCamera()
        {
            canvasConsole.worldCamera = null;
        }

        //public void UpdateVideo()
        //{
        //    string videoActual = videoPlayer.url;
        //    Stop();
        //    if (urlVideo.text == "")
        //    {
        //        videoPlayer.url = videoActual;
        //    }
        //    else
        //    {
        //        videoPlayer.url = urlVideo.text;
        //    }

        //    Play();
        //}

        //public void addUrlVideo()
        //{
        //    videosUrls.Add(videoUrlInputField.text);
        //    videoUrlInputField.text = "";
        //}

        public void NextTime()
        {

        }
        public void PreviousTime()
        {

        }

        public void Stop()
        {
            if (Sync)
            { //photonView.RPC("RPC_Stop", RpcTarget.All);
            }
            else { videoPlayer.Stop(); }

        }

        public void Play()
        {
            if (Sync)
            { //photonView.RPC("RPC_Play", RpcTarget.All);
            }
            else { videoPlayer.Play(); }

        }
        public void Play(int videoIndex)
        {
            videoPlayer.url = videosUrls[videoIndex];
            Play();
        }
        public void Pause()
        {
            if (Sync)
            { //photonView.RPC("RPC_Pause", RpcTarget.All);
            }
            else { videoPlayer.Pause(); }

            //}
        }

        public void OnVolumenSlider()
        {

            if (!activateSliderVolumen)
            {
                volumenSlider.gameObject.SetActive(true);
                activateSliderVolumen = true;
            }
            else
            {
                volumenSlider.gameObject.SetActive(false);
                activateSliderVolumen = false;
            }
        }

        public void OnSpeedSlider()
        {
            if (!activateSliderVolumen)
            {
                speedSlider.gameObject.SetActive(true);
                activateSliderSpeed = true;
            }
            else
            {
                speedSlider.gameObject.SetActive(false);
                activateSliderSpeed = false;
            }
        }

        public void SetVolumenSlider()
        {
            if (Sync)
            { //photonView.RPC("RPC_SetVolumenSlider", RpcTarget.All, volumenSlider.value);
            }
            else { audioSource.volume = volumenSlider.value; }

        }

        public void SetSpeedSlider()
        {
            if (Sync)
            { //photonView.RPC("RPC_SetSpeedSlider", RpcTarget.All, speedSlider.value);
            }
            else { videoPlayer.playbackSpeed = speedSlider.value; }
        }
        string GetTimeText(float timeSeconds)
        {
            //var t = Mathf.Clamp(time - timeElapsed, 0.0f, time);
            //if (timeAsScore)
            //{
            //    return Mathf.FloorToInt(t * scoreBased).ToString();
            //}
            //else
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(timeSeconds);
                var min = /*time >= 60 ?*/ timeSpan.Minutes.ToString("D2") + ":";
                return min + timeSpan.Seconds.ToString("D2") + ":" + (timeSpan.Milliseconds / 10).ToString("D2");
            }
        }
        //[PunRPC]
        void RPC_Play()
        {
            videoPlayer.Play();
        }
        //[PunRPC]
        void RPC_Pause()
        {
            videoPlayer.Pause();
        }
        //[PunRPC]
        void RPC_Stop()
        {
            videoPlayer.Stop();
        }
        //[PunRPC]
        void RPC_SetVolumenSlider(float value)
        {
            audioSource.volume = value;
        }
        //[PunRPC]
        void RPC_SetSpeedSlider(float value)
        {
            videoPlayer.playbackSpeed = value;
        }
    }
}