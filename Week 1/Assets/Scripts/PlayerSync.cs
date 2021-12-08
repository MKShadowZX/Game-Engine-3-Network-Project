using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerSync : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        //Enable the carmovementcontroller only if the current car is controllers by the current instance.
        //We check that by using 'photonView.IsMine' property.
        GetComponent<CarMovementController>().enabled = photonView.IsMine;
        //Turn off car view if the current instance is not controlling this car.
        GetComponentInChildren<Camera>().gameObject.SetActive(photonView.IsMine);
        //turn off lap controller if it doesn't 
        //belong to our view
        GetComponent<LapController>().enabled = photonView.IsMine;

        GetComponentInChildren<Text>().text = photonView.Owner.NickName;
        if (photonView.Owner.NickName == PhotonNetwork.NickName)
        {
            GetComponentInChildren<Text>().gameObject.SetActive(false);
        }

        GO_ID_Duo duo = new GO_ID_Duo(gameObject, photonView.ViewID);

        if (!GameManager.instance.playerRanks.Exists(x => x.viewID == photonView.ViewID))
            GameManager.instance.playerRanks.Add(duo);
    }

    
}
