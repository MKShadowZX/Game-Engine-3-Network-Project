using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GO_ID_Duo
{
    public int rank;
    public GameObject go;
    public int viewID;

    public PhotonView pv { get => go.GetComponent<PhotonView>(); }

    public GO_ID_Duo(GameObject _go, int vi)
    {
        viewID = vi;
        go = _go;
        rank = 0;
    }
}
