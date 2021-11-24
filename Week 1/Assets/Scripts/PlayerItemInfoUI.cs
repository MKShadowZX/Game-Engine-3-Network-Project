using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerItemInfoUI : MonoBehaviour
{
    public Text playerName;
    public Button readyBtn;
    public Image readyImg;

    public void Init(int pNum, string pName)
    {
        playerName.text = pNum + ". " + pName;

        if (PhotonNetwork.LocalPlayer.ActorNumber != pNum)
        {
            readyBtn.gameObject.SetActive(false);
        }
        else
        {
            readyBtn.gameObject.SetActive(true);
        }

    }

    #region READY_BUTTON

    public void OnReadyButtonClicked()
    {
        //Transmit to photon network that our local player has pressed the ready button.

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable()
        {
            {"pReady", true }

        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

        SetReadyState(true);
        readyBtn.gameObject.SetActive(false);

    }

    public void SetReadyState(bool isReady)
    {
        if (isReady)
        {
            readyImg.enabled = true;
        }
        else
        {
            readyImg.enabled = false;
        }
    }


    #endregion


}
