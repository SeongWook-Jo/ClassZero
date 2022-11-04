using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyManager : MonoBehaviour
{
    private PhotonView pv;
    
    public bool?[] playerReady = new bool?[3] { null,null,null};  // Network room에 들어온 Player.ID 를 받을 배열(maxplayer가 3명까지라 size 3)
    private int[] playerNumber = new int[100]; // Player가 room에 들어올 때 마다 1씩 찍히기 때문에(찍어본 결과 나갔다 들어오면 추가로 쌓이면서 찍힘) 여유있게 100으로 설정

    public GameObject StartBtn;
    public GameObject ReadyBtn;


    void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true; // 보내는걸 멈춰놨던 메세지 다시 true 해줌
        
        pv = GetComponent<PhotonView>(); // PhotonView 사용을 위해 pv를 가져온다,,
        PhotonNetwork.automaticallySyncScene=true;
    }
    
    void Start()
    {
        Debug.Log(playerReady.Length);
        {
            playerNumber[PhotonNetwork.player.ID] = 0;
            playerReady[0] = false;
        }

    }

    private void Update() // MasterClient는 Update에 종속
    {
        
        Debug.Log(playerReady[0] +" " +playerReady[1] + " "+ playerReady[2]);

        if ((bool)playerReady[0] && (bool)playerReady[1] && (bool)playerReady[2])  // 여기가... 아예 못받아옴
        {
            
            StartBtn.SetActive(true);
        }
        else
        {
            StartBtn.SetActive(false);
        }

    }

    // Lobby로 되돌아감
    public void OnClickReturnLobby()
    {
        // "안의 캔버스 컴포넌트 비활성화"
        //GameObject.Find("").GetComponent<Canvas>().enabled = false;

        SceneManager.LoadScene("scNetLobby");

        PhotonNetwork.LeaveRoom();
    }

   
    public void OnClickSet()  // set(sound)..
    {
        SoundManager soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundmanager.OnClickSoundUi_Close();
        
    }
    
    
    public void OnPhotonPlayerConnected(PhotonPlayer other) //여기서 Player들 정보 받고, 참거짓 확인함.
    {
        Debug.Log("Connected");

       for(int i=0; i<playerReady.Length; i++)
        {
            if(playerReady[i]==null)
            {
                playerNumber[other.ID] = i;
                playerReady[i] = false;
                break;
            }
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        playerReady[playerNumber[other.ID]] = null;
    }

    public void OnClickReady() // 버튼 누르면 MasterClient에게 player.ID 보내주고
    {

        pv.RPC("Ready", PhotonTargets.MasterClient,PhotonNetwork.player.ID);
    }

    [PunRPC]
    public void Ready(int other) // MasterClient가 player.ID 받아서 여기서 true/false 바꿔주는 작업을 한다.
    {
         playerReady[playerNumber[other]] = !playerReady[playerNumber[other]];
    }


    public void OnClickStartGame()  // 스타트 게임 버튼을 누르면 InGame 씬으로 전환
    {
        //PhotonNetwork.automaticallySyncScene; 어디?
        PhotonNetwork.LoadLevel("scNetInGame");
        
        //SceneManager.LoadScene("scNetInGame");
    }

}