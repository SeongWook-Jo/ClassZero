using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyManager : MonoBehaviour
{
    private PhotonView pv;
    private bool[] playerReady = new bool[3];
    

    private void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true;
        
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        Debug.Log(PhotonNetwork.player.ID + "번째");
    }
    
    // Lobby로 되돌아감
    public void ReturnLobby()
    {
        // "안의 캔버스 컴포넌트 비활성화"
        //GameObject.Find("").GetComponent<Canvas>().enabled = false;

        SceneManager.LoadScene("scNetLobby");

        PhotonNetwork.LeaveRoom();
    }

    public void lobbySet_Open()
    {
        SoundManager soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundmanager.SoundUiOpen();

    }

    public void OnPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerConnected()" + other.NickName);

        if(PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient" + PhotonNetwork.isMasterClient);
            
        }
    }

}