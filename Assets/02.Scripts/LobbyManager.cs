using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{ 
    public GameObject lobbyInfo_; // 게임설명
    public GameObject lobbyRoom_; // 방 목록
    public GameObject lobbyCredit_;
    private GameObject SoundManager;
    public GameObject[] lobbyBtn;  // 로비 화면 버튼들 참조 배열 
    
    private void Start()
    {
        //다른 씬에있는 사운드매니저오브젝트를 찾아줍니다.
        SoundManager =GameObject.Find("SoundManager");
    }
    public void UIOpenSound()
    {
        //찾아준 사운드매니저 오브젝트의 함수을 실행해줍니다.
        SoundManager.GetComponent<SoundManager>().UIOpenSound();
    }
    public void UICloseSound()
    {
        SoundManager.GetComponent<SoundManager>().UICloseSound();
    }
    public void UIBackSound()
    {
        SoundManager.GetComponent<SoundManager>().UIBackSound();
    }
    public void BGMLobby()
    {
        SoundManager.GetComponent<SoundManager>().LobbyBGM();
    }
    public void lobbySet_Open()
    {
        //lobbyBtn[0].SetActive(false); // 1.버튼 누르면 로비에 있는 버튼 비활성화시킴
        //lobbyBtn[1].SetActive(false);
        //lobbyBtn[2].SetActive(false);
        //lobbyBtn[3].SetActive(false);
        //lobbyBtn[4].SetActive(false);
        //lobbyBtn[5].SetActive(false);

        //lobbySet_.SetActive(true);     // 2. 패널 활성화
        SoundManager soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundmanager.SoundUiOpen();


    }

    //public void OnClickSet()
    //{
    //    GameObject.Find("SoundManager");
    //}

    public void lobbyInfo_Open()
    {
        lobbyBtn[0].SetActive(false);
        lobbyBtn[1].SetActive(false);
        lobbyBtn[2].SetActive(false);
        lobbyBtn[3].SetActive(false);
        lobbyBtn[4].SetActive(false);
        lobbyBtn[5].SetActive(false);

        lobbyInfo_.SetActive(true);
    }

    public void lobbyInfo_Close()
    {
        lobbyInfo_.SetActive(false);

        lobbyBtn[0].SetActive(true);
        lobbyBtn[1].SetActive(true);
        lobbyBtn[2].SetActive(true);
        lobbyBtn[3].SetActive(true);
        lobbyBtn[4].SetActive(true);
        lobbyBtn[5].SetActive(true);

    }

    public void lobbyRoom_Open()
    {
        lobbyBtn[0].SetActive(false);
        lobbyBtn[1].SetActive(false);
        lobbyBtn[2].SetActive(false);
        lobbyBtn[3].SetActive(false);
        lobbyBtn[4].SetActive(false);
        lobbyBtn[5].SetActive(false);

        lobbyRoom_.SetActive(true);
    }

    public void lobbyRoom_Close()
    {
        lobbyRoom_.SetActive(false);

        lobbyBtn[0].SetActive(true);
        lobbyBtn[1].SetActive(true);
        lobbyBtn[2].SetActive(true);
        lobbyBtn[3].SetActive(true);
        lobbyBtn[4].SetActive(true);
        lobbyBtn[5].SetActive(true);
    }

    public void lobbyCredit_Open()
    {
        lobbyBtn[0].SetActive(false);
        lobbyBtn[1].SetActive(false);
        lobbyBtn[2].SetActive(false);
        lobbyBtn[3].SetActive(false);
        lobbyBtn[4].SetActive(false);
        lobbyBtn[5].SetActive(false);

        lobbyCredit_.SetActive(true);
    }

    public void lobbyCredit_Close()
    {
        lobbyCredit_.SetActive(false);

        lobbyBtn[0].SetActive(true);
        lobbyBtn[1].SetActive(true);
        lobbyBtn[2].SetActive(true);
        lobbyBtn[3].SetActive(true);
        lobbyBtn[4].SetActive(true);
        lobbyBtn[5].SetActive(true);
    }

    // Room 선택 시 준비로 넘어감
    public void PlayGame()
    {
        // "안의 캔버스 컴포넌트 비활성화"
        //GameObject.Find("").GetComponent<Canvas>().enabled = false;
        PhotonNetwork.isMessageQueueRunning = false;
        SceneManager.LoadScene("scNetReady");
    }

    public void ExitGame()
    {
        //SceneManager.LoadScene("scNetUi");
        Application.Quit();
    }
}
