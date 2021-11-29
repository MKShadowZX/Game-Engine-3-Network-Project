using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Text roomName;
    NetworkMgr networkMgr;

    private void Start()
    {
        networkMgr = FindObjectOfType<NetworkMgr>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void OnRoomItemPressed()
    {
        networkMgr.JoinRoom(roomName.text);
    }
}
