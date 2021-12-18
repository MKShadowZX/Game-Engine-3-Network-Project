using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviour
{

    public List<PlayerStandingUIItem> standingsUIList;
    public List<PlayerStandingUIItem> playerRankUIList;

    public enum raiseEventCodes
    {
        raceFinishCode = 0,
        raceFinishUpdateRank = 1,
        raceCheckLapRank = 2,
        raceLapRankUpdate = 3
    };

    private int lastAssignedRank = 0;

    public Text countdownTimerText;

    static int minuteCount;
    static int secondCount;
    static float milliSecondCount;
    string totalTime;

    bool isRaceStarting = false;

    CarIndex carIndex;

    //Drag and drop the empty game objects that denote the player start points.
    public List<Transform> playerStartPoints;

    //Drag & drop the car prefabs fromt he 'Resources' folder
    public List<GameObject> playerPrefebs;

    [HideInInspector]
    public GameObject myCarInstance;

    public List<GO_ID_Duo> playerRanks;

    public GameObject gameOverPanel;
    public GameObject standingUIPanel;
    int allDoneCount;

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

        playerRanks = new List<GO_ID_Duo>();
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

        int indx = playerRanks.FindIndex(x => x.viewID == photonViewID);

        playerRanks[indx].checkpoint++;

        object[] data = new object[] { playerRanks[indx].viewID, playerRanks[indx].rank, playerRanks[indx].checkpoint };
        // Raise an event via PhotonNetwork ...
        PhotonNetwork.RaiseEvent(
            (byte)raiseEventCodes.raceFinishCode,
           data,
           new Photon.Realtime.RaiseEventOptions() { Receivers = Photon.Realtime.ReceiverGroup.All },
           SendOptions.SendReliable

            );
    }

    void UpdateLapRank(int photonViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int indx = playerRanks.FindIndex(x => x.viewID == photonViewID);

        playerRanks[indx].checkpoint++;

        if (playerRanks[indx].rank == 0)
        {
            lastAssignedRank++;
            playerRanks[indx].rank = lastAssignedRank;
        }

        object[] data = new object[] { playerRanks[indx].viewID, playerRanks[indx].rank, playerRanks[indx].checkpoint };
        // Raise an event via PhotonNetwork ...
        PhotonNetwork.RaiseEvent(
            (byte)raiseEventCodes.raceLapRankUpdate,
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
            int checkpoint = (int)incomingData[2];

            int indx = playerRanks.FindIndex(x => x.viewID == viewID);
            //This will have no effect on the masterclient as we already have the rank
            //updated.
            //but on every other non-master client, this is important as those clients
            //won't have the rank data updated until we do the following
            playerRanks[indx].rank = rank;
            playerRanks[indx].checkpoint = checkpoint;

            //Next step: Display which player just completed the race....
            Debug.Log(playerRanks[indx].pv.Owner.NickName + " FINISHED  AT POSITION: " +
                rank);

            bool swap = true;

            while (swap)
            {
                swap = false;

                for (int i = playerRanks.Count - 1; i > 0; i--)
                {
                    if (playerRanks[i].checkpoint > playerRanks[i - 1].checkpoint)
                    {
                        swap = true;
                        var tempRank = playerRanks[i - 1];
                        playerRanks[i - 1] = playerRanks[i];
                        playerRanks[i] = tempRank;
                    }
                }
            }

            //TO-DO (part of beta sprint 2 assignment)
            //Step 1:
            //Turn on the respective standing/rank's UI game Object
            //standingsUIList[_______].gameObject.SetActive(true);
            
            //Step 2:
            //Call  the 'UpdateInfo' function on that ui item and pass the 
            //photon player/car nickname, rank, and whether the car belongs to you
            //standingsUIList[_______].UpdateInfo( ________, ________ , __________);
            CalculateTime();
            playerRanks[indx].totalTime = totalTime;
            playerRanks[indx].isFinished = true;
            allDoneCount++;

            for (int i = 0; i < playerRanks.Count; i++)
            {
                playerRanks[i].rank = i + 1;
                standingsUIList[i].UpdateInfo(playerRanks[i].pv.Owner.NickName, playerRanks[i].rank, playerRanks[i].totalTime, PhotonNetwork.LocalPlayer.IsLocal);
                playerRankUIList[i].UpdateInfo(playerRanks[i].pv.Owner.NickName, playerRanks[i].rank, playerRanks[i].totalTime, PhotonNetwork.LocalPlayer.IsLocal);
                standingsUIList[i].gameObject.SetActive(true);
                playerRankUIList[i].gameObject.SetActive(true);

                if (allDoneCount == playerRanks.Count)
                {
                    standingUIPanel.SetActive(false);
                    gameOverPanel.SetActive(true);
                }
            }
        }
        else if (photonEventData.Code == (byte)raiseEventCodes.raceCheckLapRank)//raceFinishUpdateRank
        {
            object[] incomingData = (object[])photonEventData.CustomData;
            UpdateLapRank((int)incomingData[1]);

        }
        else if (photonEventData.Code == (byte)raiseEventCodes.raceLapRankUpdate)
        {
            object[] incomingData = (object[])photonEventData.CustomData;
            int viewID = (int)incomingData[0];
            int rank = (int)incomingData[1];
            int checkpoint = (int)incomingData[2];

            int indx = playerRanks.FindIndex(x => x.viewID == viewID);

            playerRanks[indx].rank = rank;
            playerRanks[indx].checkpoint = checkpoint;

            bool swap = true;

            while (swap)
            {
                swap = false;

                for (int i = playerRanks.Count - 1; i > 0; i--)
                {
                    if (playerRanks[i].checkpoint > playerRanks[i - 1].checkpoint)
                    {
                        swap = true;
                        var tempRank = playerRanks[i - 1];
                        playerRanks[i - 1] = playerRanks[i];
                        playerRanks[i] = tempRank;
                    }
                }
            }

            /*for (int i = 0; i < playerRanks.Count; i++)
            {
                if (indx != i)
                {
                    if (playerRanks[indx].checkpoint > playerRanks[i].checkpoint)
                    {
                        if (playerRanks[indx].rank > playerRanks[i].rank && playerRanks[i].rank != 0)
                        {
                            int tempRank = playerRanks[indx].rank;
                            playerRanks[indx].rank = playerRanks[i].rank;
                            playerRanks[i].rank = tempRank;

                            for (int j = 0; j < playerRanks.Count; j++)
                            {
                                if (i != j)
                                {
                                    if (playerRanks[i].checkpoint > playerRanks[j].checkpoint)
                                    {
                                        if (playerRanks[i].rank > playerRanks[j].rank && playerRanks[i].rank != 0)
                                        {
                                            tempRank = playerRanks[i].rank;
                                            playerRanks[i].rank = playerRanks[j].rank;
                                            playerRanks[i].rank = tempRank;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }*/

            CalculateTime();

            for (int i = 0; i < playerRanks.Count; i++)
            {
                if (!playerRanks[i].isFinished)
                {
                    playerRanks[i].rank = i + 1;
                    playerRanks[i].totalTime = totalTime;
                    standingsUIList[i].UpdateInfo(playerRanks[i].pv.Owner.NickName, playerRanks[i].rank, playerRanks[i].totalTime, PhotonNetwork.LocalPlayer.IsLocal);
                    standingsUIList[i].gameObject.SetActive(true);
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (isRaceStarting)
        {
            milliSecondCount += Time.deltaTime * 10;

            if (milliSecondCount >= 10)
            {
                milliSecondCount = 0;
                secondCount += 1;
            }

            if (secondCount >= 60)
            {
                secondCount = 0;
                minuteCount += 1;
            }
        }
    }

    public void EnableRaceTimer()
    {
        isRaceStarting = true;
    }

    public void CalculateTime()
    {
        string minute;
        string second;

        if (minuteCount <= 9)
        {
            minute = "0" + minuteCount.ToString();
        }
        else
        {
            minute = minuteCount.ToString();
        }

        if (secondCount <= 9)
        {
            second = "0" + secondCount.ToString();
        }
        else
        {
            second = secondCount.ToString();
        }

        totalTime = minute + ":" + second + "." + milliSecondCount.ToString("F0");
    }
}
