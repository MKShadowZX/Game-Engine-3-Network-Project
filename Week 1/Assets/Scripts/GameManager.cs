using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Drag and drop the empty game objects that denote the player start points.
    public List<Transform> playerStartPoints;

    //Drag & drop the car prefabs fromt he 'Resources' folder
    public List<GameObject> playerPrefebs;

    [HideInInspector]
    public GameObject myCarInstance;


    private void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("<color=red> Not connected to photon network. Can't proceed </color>");
            return;
        }

        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        //Actor is 1-based. So we subtract 1 from it to get the index of the list.
        Vector3 startPos = playerStartPoints[actorNum - 1].position;

        //Note that we use PhotonNetwork.Instantiate instead of the regular Instantiate.
        myCarInstance = PhotonNetwork.Instantiate(playerPrefebs[0].name, startPos, Quaternion.identity);




    }

}
