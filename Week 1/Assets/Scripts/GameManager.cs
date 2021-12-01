using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviour
{

    public List<PlayerStandingUIItem> standingsUIList;

    public enum raiseEventCodes
    {
        raceFinishCode = 0,
        raceFinishUpdateRank = 1
    };

    private int lastAssignedRank = 0;

    public Text countdownTimerText;

    //Drag and drop the empty game objects that denote the player start points.
    public List<Transform> playerStartPoints;

    //Drag & drop the car prefabs fromt he 'Resources' folder
    public List<GameObject> playerPrefebs;

    [HideInInspector]
    public GameObject myCarInstance;

    CarIndex carIndex;

    public List<GO_ID_Duo> playerRanks;

    //Lazy Singleton
    #region LAZY_SINGLETON_AND_AWAKE
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("<color=red> Not connected to photon network. Can't proceed </color>");
            return;
        }

        carIndex = FindObjectOfType<CarIndex>();

        SpawnCar();
    }

    void SpawnCar()
    {
        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;

        //Actor is 1-based. So we subtract 1 from it to get the index of the list.
        Vector3 startPos = playerStartPoints[actorNum - 1].position;

        //Note that we use PhotonNetwork.Instantiate instead of the regular Instantiate.
        myCarInstance = PhotonNetwork.Instantiate(playerPrefebs[carIndex.index].name, startPos, Quaternion.identity);
    }

    //To be called each time a car passes through the trigger point
    //This code should only be executed on masterclient
    void CheckAndUpdateRank(int photonViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        lastAssignedRank++;

        int indx = playerRanks.FindIndex(x => x.viewID == photonViewID);

        playerRanks[indx].rank = lastAssignedRank;

        object[] data = new object[] { playerRanks[indx].viewID, playerRanks[indx].rank };
        // Raise an event via PhotonNetwork ...
        PhotonNetwork.RaiseEvent(
            (byte)raiseEventCodes.raceFinishCode,
           data,
           new Photon.Realtime.RaiseEventOptions() { Receivers = Photon.Realtime.ReceiverGroup.All },
           SendOptions.SendReliable

            );
    }


    /// <summary>
    /// This will be called by photon when a particular game event code is triggered
    /// </summary>
    /// <param name="photonEventData"></param>
    public void OnEventCallback(EventData photonEventData)
    {

        if (photonEventData.Code == (byte)raiseEventCodes.raceFinishUpdateRank)//raceFinishUpdateRank
        {
            object[] incomingData = (object[])photonEventData.CustomData;
            CheckAndUpdateRank((int)incomingData[1]);

        }
        //the following event can be called on any car... even the ones that are still 
        //in the race and not yet crossed the finish line
        //The events could be sent by a car that just completed the race...
        else if (photonEventData.Code == (byte)raiseEventCodes.raceFinishCode)//raceFinishCode
        {
            object[] incomingData = (object[])photonEventData.CustomData;
            int viewID = (int)incomingData[0];
            int rank = (int)incomingData[1];


            int indx = playerRanks.FindIndex(x => x.viewID == viewID);
            //This will have nno effect on the masterclient as we already have the rank
            //updated.
            //but on every other non-master client, this is important as those clients
            //won't have the rank data updated until we do the following
            playerRanks[indx].rank = rank;

            //Next step: Display which player just completed the race....
            Debug.Log("<color=red>" + playerRanks[indx].pv.Owner.NickName + " FINISHED  AT POSITION: " +
                rank + "</color>");

            //TO-DO (part of beta sprint 2 assignment)
            //Step 1:
            //Turn on the respective standing/rank's UI game Object
            //standingsUIList[_______].gameObject.SetActive(true);
            //Step 2:
            //Call  the 'UpdateInfo' function on that ui item and pass the 
            //photon player/car nickname, rank, and whether the car belongs to you
            //standingsUIList[_______].UpdateInfo( ________, ________ , __________);
        }

    }
}
