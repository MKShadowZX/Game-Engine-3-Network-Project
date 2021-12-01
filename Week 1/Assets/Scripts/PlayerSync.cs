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
        GetComponentInChildren<Text>().text = photonView.Owner.NickName;
    }

    
}
