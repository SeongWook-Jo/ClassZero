using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyManager : MonoBehaviour
{
    private PhotonView pv;
    public bool?[] playerReady = new bool?[3] { null, null, null };
    private int[] playerNumber = new int[100];

    private void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true;
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        Debug.Log(playerReady.Length);
        {
            playerNumber[PhotonNetwork.player.ID] = 0;
            playerReady[0] = false;
        }
    }
    private void Update()
    {
        if((bool)playerReady[0] && (bool)playerReady[1] && (bool)playerReady[2])
        {
            Debug.Log("11");
        }
    }
    // Lobby로 되돌아감
    public void ReturnLobby()
    {
        //PhotonNetwork.player.ID
        // "안의 캔버스 컴포넌트 비활성화"
        //GameObject.Find("").GetComponent<Canvas>().enabled = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("scNetLobby");
    }

    public void OnPhotonPlayerConnected(PhotonPlayer tempPlayer)
    {
        for (int i = 0; i < playerReady.Length; i++)
        {
            if (playerReady[i] == null)
            {
                playerNumber[tempPlayer.ID] = i;
                playerReady[i] = false;
                break;
            }
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer tempPlayer)
    {
        playerReady[playerNumber[tempPlayer.ID]] = null;
    }

    public void OnClickReady()
    {
        pv.RPC("Ready", PhotonTargets.All, PhotonNetwork.player.ID);
    }
    [PunRPC]
    public void Ready(int id)
    {
        if (playerReady[playerNumber[id]] == false)
            playerReady[playerNumber[id]] = true;
        else
            playerReady[playerNumber[id]] = false;
    }
    public void OnClickReturn()
    {
        pv.RPC("PlayerIdSend", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
    }
    [PunRPC]
    public void PlayerIdSend(int temp)
    {
        Debug.Log("플레이어 ID " + temp);
        Debug.Log(playerReady[playerNumber[temp]]);
        Debug.Log(playerReady[0] + " "+ playerReady[1] + " " + playerReady[2]);
    }


}
