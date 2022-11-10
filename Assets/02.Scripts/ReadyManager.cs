using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class ReadyManager : MonoBehaviour
{
    private PhotonView pv;

    public bool?[] playerReady = new bool?[3] { null, null, null };  // Network room에 들어온 Player.ID 를 받을 배열(maxplayer가 3명까지라 size 3)
    public bool[] PR = { false, false, false };
    private int[] playerNumber = new int[100]; // Player가 room에 들어올 때 마다 1씩 찍히기 때문에(찍어본 결과 나갔다 들어오면 추가로 쌓이면서 찍힘) 여유있게 100으로 설정

    public GameObject StartBtn;
    public GameObject ReadyBtn;

    public GameObject[] Characters;  // ready 되면 캐릭터들 하나씩 On 

    //chat을 위한 변수 선언
    public Text chatLog;
    public Text msg;
    public InputField intput;
    ScrollRect scroll_rect = null;
    //chat을 위한 변수 선언
    //public Text chatLog;
    //public Text msg;
    //public InputField intput;
    //ScrollRect scroll_rect = null;
    public InputField chatMsg;  // 메시지 받고
    public Text txtChatMsg; // 메시지 표시하고


    void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true; // 보내는걸 멈춰놨던 메세지 다시 true 해줌

        pv = GetComponent<PhotonView>(); // PhotonView 사용을 위해 pv를 가져온다,,
        PhotonNetwork.automaticallySyncScene = true;
    }

    void Start()
    {
        {
            playerNumber[PhotonNetwork.player.ID] = 0;
            playerReady[0] = false;
        }

    }

    private void Update()
    {

        if (!playerReady[0] != null)
            PR[0] = (bool)playerReady[0];
        if (playerReady[1] != null)
            PR[1] = (bool)playerReady[1];
        if (playerReady[2] != null)
            PR[2] = (bool)playerReady[2];
        if (PR[0] && PR[1] && PR[2])
        {

            StartBtn.SetActive(true);

        }
        else
        {
            StartBtn.SetActive(false);
        }
        if (pv.isMine) 
            pv.RPC("CharacterOnOff", PhotonTargets.All, PR);
        //CharacterOnOff();

    }

    // Lobby로 되돌아감f
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

        for (int i = 0; i < playerReady.Length; i++)
        {
            if (playerReady[i] == null)
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

        pv.RPC("Ready", PhotonTargets.MasterClient, PhotonNetwork.player.ID);

    }

    [PunRPC]
    public void Ready(int other) // MasterClient가 player.ID 받아서 여기서 true/false 바꿔주는 작업을 한다.
    {
        playerReady[playerNumber[other]] = !playerReady[playerNumber[other]];
    }


    public void OnClickStartGame()  // 스타트 게임 버튼을 누르면 InGame 씬으로 전환 + 플레이어 값을 넘겨줌
    {
        //해쉬테이블로 NumberInRoom라는 Key값으로 Value저장
        //현재 방에 있는 유저목록 불러옴
        PhotonPlayer[] players =  PhotonNetwork.playerList;
        //Key, Value 저장을 위한 해쉬테이블 생성 해쉬테이블 배열로 했으나 NullReference로 인해서 변수 한개로 생성
        Hashtable playerOpth = new Hashtable();
        for(int i = 0;i<PhotonNetwork.room.PlayerCount; i++)
        {
            //배정된 위치를 할당해줌 PR[x] 
            playerOpth.Add("NumberInRoom", playerNumber[players[i].ID]);
            //커스텀 속성을 만들어서 저장
            players[i].SetCustomProperties(playerOpth);
            //배열이 아니기 때문에 지워주고 다시 for문 진행
            playerOpth.Remove("NumberInRoom");
        }

        //PhotonNetwork.automaticallySyncScene; 어디?
        PhotonNetwork.LoadLevel("scNetInGame");

        //SceneManager.LoadScene("scNetInGame");
    }

    [PunRPC]
    public void CharacterOnOff(bool[] other) // 캐릭터 SetActive(false) -> SetActive(true)
    {

        for (int i = 0; i < 3; i++)
        {
            if (other[i])
            {
                Characters[i].SetActive(true);
            }
            else
            {
                Characters[i].SetActive(false);
            }
        }
    }

    // Chatting --------------------------------
    //public void SendButtonOnClicked()  //전송 버튼이 눌리면 실행될 메소드, 메세지 전송을 담당
    //{
    //    //if()
    //}

    //[PunRPC]
    //public void ReceiveMsg(string msg) //전송된 메세지를 받을 이들이 실행할 것
    //{
    //    chatLog.text += "\n" + msg;
    //    scroll_rect.verticalNormalizedPosition = 0.0f;
    //}
    [PunRPC]
    void ChatMsg(string ctmsg)
    {
        //chatMsg.text = chatMsg.Tostring();
        txtChatMsg.text = txtChatMsg.text + ctmsg;  //채팅 메시지 Tex UI에 텍스트를 누적시켜서 표시
    }

    public void OnClickChatMsgSend()
    {
        //채팅 메시지에 출력할 문자열 생성
        string ctmsg = "\n\t<color=#ffffff>[" + PhotonNetwork.player.NickName + "] </color>" + chatMsg.text;  //"\n\t<color=#ffffff>[" + chatMsg.text + "] </color>";
        pv.RPC("ChatMsg", PhotonTargets.All, ctmsg);
        chatMsg.text = "";
    }

    //chatting ----------------------------------

}