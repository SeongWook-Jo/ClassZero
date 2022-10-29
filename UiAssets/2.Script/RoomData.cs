using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    [HideInInspector]
    // 방 이름
    public string roomName = "";

    [HideInInspector]
    // 현재 접속 유저 수
    public int connectPlayer = 0;

    //룸의 최대 접속자 수
    [HideInInspector]
    public int maxPlayers = 0;

    public Text textRoomName;
    public Text textConnectInfo;

   public void DisplayRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/" + maxPlayers.ToString() + ")";
    }
}
