using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class NetworkMgr : MonoBehaviourPunCallbacks
{
    private string _playerName;
    private string _roomName;
    private string _gameMode = "rm";

    public string playerName
    {
        set { _playerName = value; }
        get { return _playerName; }
    }

    public string roomName 
    {
        set { _roomName = value; }
        get { return _roomName; }
    }

    public GameObject waitingToConnectPanel;
    public GameObject gameLobbyOptionsPanel;
    public GameObject loginPanel;
    public GameObject joiningRoomPanel;

    public GameObject createRoomPanel;
    public GameObject creatingRoomPanel;
    public GameObject roomLobbyPanel;

    public Button strtGameButton;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemsList = new List<RoomItem>();
    public Transform contentObj;

    //References from RoomUserPanel
    public Text roomInfoText;
    public Text gameTypeText;

    //Drag & drop 'PlayerList' under 'RoomUserPanel' into the field.
    public Transform playerListHolder;

    public GameObject playerListItemPrefab;

    private Dictionary<int, GameObject> playerGODict;

    private void Awake()
    {
        playerGODict = new Dictionary<int, GameObject>(); //initialize this before we use it later...
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //MasterClient -> Is the one who creates the room generally, this of this as the host
    //Client -> All other clients who joins an existing room created by the master client

    /// <summary>
    /// Own custom function that will connect to photon server
    /// </summary>
    void Connect() 
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// This function should be called when login button is pressed on the U.I
    /// </summary>
    public void OnLoginButtonPressed()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("<color=red> Player name not entered. Can't connect to server without one.</color>");
        }
        else
        {
            Connect();
            waitingToConnectPanel.SetActive(true);
            loginPanel.SetActive(false);
        }


    }

    public override void OnConnected()
    {
        Debug.Log("<color=green> Connection established with Photon! </color>");

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "got connected.");

        waitingToConnectPanel.SetActive(false);
        gameLobbyOptionsPanel.SetActive(true);

    }

    void CreateRoom()
    {

        Photon.Realtime.RoomOptions ro = new Photon.Realtime.RoomOptions();

        ro.MaxPlayers = 4;

        //"m" = gameMode
        ro.CustomRoomPropertiesForLobby = new string[] { "m" };
        //"gameMode" How many bytes? - 8 bytes
        // "m" How many bytes? - 1 byte
        //"gameTypeName" - 12 bytes - Each letter is 1 byte

        //1 player
        //4 clients - 12 * 4 = 48 bytes. //per occurance.
        //10 times in a second.
        //48 * 10 = 480 bytes in a second.
        //40 bytes in a second if you go with 1 letter.

        //"grp" = game room properties.
        ExitGames.Client.Photon.Hashtable grp = new ExitGames.Client.Photon.Hashtable()
        {
            {"m", _gameMode }//,
            // {"e", },
            // {"l",  }
        };


        ro.CustomRoomProperties = grp;

        PhotonNetwork.CreateRoom(roomName, ro);

    }
    /// <summary>
    /// Called when the room is created successfully.
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("<color= green> Room created successfully! </color>");
        roomLobbyPanel.SetActive(true);
        creatingRoomPanel.SetActive(false);
    }

    /// <summary>
    /// Called when the room creation failed.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("<color= red> Room created failed: error code:" 
            + returnCode+". err msg: " 
            + message + "</color>");
    }

    public void OnCreateRoomButtonPressed()
    {
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("<color=red> Room name not entered. Can't create a room without one.</color>");
        }
        else
        {
            CreateRoom();
            creatingRoomPanel.SetActive(true);
            createRoomPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Get a 2 letter code fromt he ui input, suggeting either rm for race mode or dm for death match mode.
    /// </summary>
    /// <param name="code"></param>
    public void SetGameMode(string code)
    {
        _gameMode = code;
    }


    public override void OnJoinedRoom()
    {
        joiningRoomPanel.SetActive(false);
        roomLobbyPanel.SetActive(true);

        Debug.Log("<color=cyan> User: " + PhotonNetwork.LocalPlayer.NickName + " joined " 
            + PhotonNetwork.CurrentRoom.Name+ " ||| Players: "
            + PhotonNetwork.CurrentRoom.PlayerCount + "/ " 
            + PhotonNetwork.CurrentRoom.MaxPlayers+"</color>");

        //Print the above information in "roomInfoText" textfield as well
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " "
            + PhotonNetwork.CurrentRoom.PlayerCount + "/" 
            + PhotonNetwork.CurrentRoom.MaxPlayers;

        //PhotonNetwork.CurrentRoom.Players

        //Fetch the list of players in the room and create an instance of playerlistitem for each player.
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            CreatePlayerListItem(p);
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("<color=cyan> Remote User: " + newPlayer.NickName + " joined "
            + PhotonNetwork.CurrentRoom.Name + " ||| Players: "
            + PhotonNetwork.CurrentRoom.PlayerCount + " / "
            + PhotonNetwork.CurrentRoom.MaxPlayers + "</color>");

        //Print the above information in "roomInfoText" textfield as well
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + "  "
            + PhotonNetwork.CurrentRoom.PlayerCount + " / "
            + PhotonNetwork.CurrentRoom.MaxPlayers;

        CreatePlayerListItem(newPlayer);
    }

    void CreatePlayerListItem(Player newPlayer)
    {
        GameObject item = Instantiate(playerListItemPrefab, playerListHolder);
        item.GetComponent<PlayerItemInfoUI>().Init(newPlayer.ActorNumber, newPlayer.NickName);
        playerGODict.Add(newPlayer.ActorNumber, item);

        object _isRemotePlayerReady;

        if(newPlayer.CustomProperties.TryGetValue("pReady", out _isRemotePlayerReady))
        {
            item.GetComponent<PlayerItemInfoUI>().SetReadyState((bool)_isRemotePlayerReady);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GameObject item = playerGODict[otherPlayer.ActorNumber].gameObject;
        Destroy(item);
        playerGODict.Remove(otherPlayer.ActorNumber);

        Debug.Log("<color=cyan> User: " + otherPlayer.NickName + " has left the room."
            + PhotonNetwork.CurrentRoom.Name + " ||| Players: "
            + PhotonNetwork.CurrentRoom.PlayerCount + "/ "
            + PhotonNetwork.CurrentRoom.MaxPlayers + "</color>");
    }

    public override void OnLeftRoom()
    {
        roomLobbyPanel.SetActive(false);
        gameLobbyOptionsPanel.SetActive(true);

        foreach (GameObject player in playerGODict.Values)
        {
            Destroy(player.gameObject);
        }
        
        playerGODict.Clear();
        //playerGODict = null;
    }

    public void OnPlayerLeave()
    {
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable()
        {
            {"pReady", false }

        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObj);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    #region JOIN_RANDOM_ROOM

    /// <summary>
    /// Call this function on each of the 2 buttons
    /// RacingGameMode & DeathRaceGameMode under JounRandomGamePanel
    /// race game is rm
    /// death race is dm
    /// </summary>
    /// <param name="gameModeCode"></param>
    public void OnJoinRandomRoomModeButtonClicked(string gameModeCode)
    {
        Debug.Log("<color=orange> Trying to find a random room of gameMode type="
            + gameModeCode + "</color>");

        ExitGames.Client.Photon.Hashtable expectedProperties = 
            new ExitGames.Client.Photon.Hashtable
        {
            {"m", gameModeCode }
        };

        PhotonNetwork.JoinRandomRoom(expectedProperties, 0);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("<color=red> Join random room failed with message = " + message + "</color>");
    }

    #endregion


    #region UPDATE_PLAYER_PROPERTIES

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        object _isRemotePlayerReady;

        if(changedProps.TryGetValue("pReady", out _isRemotePlayerReady)) 
        {
            playerGODict[targetPlayer.ActorNumber].GetComponent<PlayerItemInfoUI>().SetReadyState((bool)_isRemotePlayerReady);    
        }

        strtGameButton.interactable = IsGameReadyToStart();
    }

    #endregion

    #region GAME_READ_FUNCTION

    private bool IsGameReadyToStart()
    {
        if (!PhotonNetwork.IsMasterClient) return false;

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            object _isRemotePlayerReady;
            if(p.CustomProperties.TryGetValue("pReady", out _isRemotePlayerReady))
            {
                if (!(bool)_isRemotePlayerReady)
                    return false;
            }
            else
            {
                Debug.LogError("Can't find pReady propert. Did you mis-spell in this function or in PlayerItemUIInfo?");
                return false;
            }
        }
        return true;
    }

    #endregion

    #region START_GAME

    public void OnStartButtonClicked()
    {
        object gameModeCode;

        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("m", out gameModeCode))
        {
            if ((string)gameModeCode == "rm") PhotonNetwork.LoadLevel("RaceModeLevel");
            else if ((string)gameModeCode == "dm")
                PhotonNetwork.LoadLevel("DeathModeLevel");
            else
            {
                Debug.Log("Didn't recognize the game mode code: " + gameModeCode);
            }
        }
        else
        {
            Debug.Log("Can't find property 'm' in the room");
        }
    }

    #endregion
}