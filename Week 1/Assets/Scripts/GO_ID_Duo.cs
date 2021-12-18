using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GO_ID_Duo
{
    public int rank;
    public int checkpoint;
    public GameObject go;
    public int viewID;
    public string totalTime;
    public bool isFinished;

    public PhotonView pv { get => go.GetComponent<PhotonView>(); }

    public GO_ID_Duo(GameObject _go, int vi)
    {
        viewID = vi;
        go = _go;
        rank = 0;
        totalTime = "00:00.0";
        checkpoint = 0;
        isFinished = false;
    }
}
