//using Photon.Pun;
//using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

//[RequireComponent(typeof(PhotonView))]
public class VideoPlayerProgressBar : /*MonoBehaviourPun*/MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public bool Sync;
    [SerializeField]
    private VideoPlayer videoPlayer;

    private Image progress;

    private void Awake()
    {
        progress = GetComponent<Image>();
    }

    private void Update()
    {
        if (videoPlayer.frameCount > 0)
            progress.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if(photonView.IsMine) photonView.RPC("RPC_TrySkip", RpcTarget.All, eventData);
        TrySkip(eventData, true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //if (photonView.IsMine) photonView.RPC("RPC_TrySkip", RpcTarget.All, eventData);
        TrySkip(eventData, true);
    }

    //[PunRPC]
    //void RPC_TrySkip(PointerEventData eventData)
    //{
    //    TrySkip(eventData);
    //}

    private void TrySkip(PointerEventData eventData, bool sendRPC = false)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            progress.rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
            if (sendRPC)
            {
                if (Sync)
                { //photonView.RPC("RPC_SkipToPercent", RpcTarget.All, pct); 
                }
                else
                {
                    SkipToPercent(pct);

                }
            }
            else
            {
                SkipToPercent(pct);
            }
        }
    }

    private void SkipToPercent(float pct)
    {
        var frame = videoPlayer.frameCount * pct;
        videoPlayer.frame = (long)frame;
    }
    //[PunRPC]
    void RPC_SkipToPercent(float pct)
    {
        SkipToPercent(pct);
    }
}