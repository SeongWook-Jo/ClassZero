using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class csPhotonInit : MonoBehaviour
{
    //App의 버전 정보 (번들 버전과 일치 시키자...)
    public string version = "Ver 0.1.0";

    //예상하는 대로 동작하는 확신이 서면 로그 레벨을 Information으로 변경하자
    public PhotonLogLevel logLevel = PhotonLogLevel.Full;

    //플레이어 이름을 입력하는 UI 항목 연결을 위한 레퍼런스, 플레이어 이름 따로 받나?
    //public InputField UserId;   //이름 고정으로 삭제

    //룸 이름을 입력받을 UI 항목 연결 레퍼런스
    public InputField roomname;

    //RoomItem이 차일드로 생성될 Parent 객체 레퍼런스
    public GameObject scrollContents;

    //룸 목록만큼 생성될 RoomItem 프리팹 연결 레퍼런스
    public GameObject roomItem;

    //플레이어 생성 위치 저장 레퍼런스, 성욱님이 만드신 거랑 합쳐야 하는지 내거에도 있어야 하는지..
    //public Transform playerPos;

    // App 인증 및 로비연결
    void Awake()
    {
        // 룸-> 로비로 씬 전환시 반복해서 다시 포톤 클라우드 서버로 접속하는 로직이 실행되는 것을 막음.
        //그니까 룸에서 로비 나가면 다시 룸으로 자동입장X 로비에 머무르게 해주는것
        if (!PhotonNetwork.connected)
        {
            // 버전 정보를 전달하며 포톤 클라우드에 접속 (지역서버 접속-> 사용자 인증 -> 로비입장)
            // 인자로 전달하는 Version은 동일 버전의 게임이 설치된 유저만 같은 로비에 접속할 수 있다.(다른 버전으로 인한 오류 원천 차단)
            PhotonNetwork.ConnectUsingSettings(version);

            //예상하는 대로 동작하는 것을 확신하면 로그 레벨을 Informational로 변경하자
            PhotonNetwork.logLevel = logLevel;

            //현재 클라이언트 유저의 이름을 포톤에 설정
            //Photonview 컴포넌트의 요소 Owner의 값이 된다
            PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
        }

        // 룸 이름을 무작위로 설정
        roomname.text = "ROOM_" + Random.Range(0, 999).ToString("000");

        //ScrollContents의 Pivot 좌표를 Top, Left로 설정
        scrollContents.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1.0f);

    }

    /* 포톤은 로비 기반으로 서버가 이루어져 있음 = 연결은 로비로 연결 되는것
     * 로비에서 방을 만들거나, 특정 방에 연결하거나 랜덤으로 연결하게 됨
     */

    //포톤 클라우드에 정상적으로 접속한 후 로비에 입장하면 호출되는 콜백 함수
    //로비 입장 후 포톤 클라우드 서버가 클라이언트에게 정상적으로 로비에 입장했다는 콜백 함수를 호출
    void OnJoinedLobby()
    {
        Debug.Log("JoinedLobby !!!");

        //로비 입장 후 이미 생성된 룸(방) 중 무작위로 선택해 입장하는 (Random Match Making) 함수
        //PhotonNetwork.JoinRandomRoom();  //// Ui 버전에서는 주석 처리

        // 유저 아이디를 가져와 셋팅
        //UserId.text = GetUserId();
    }

    // 로컬에 저장된 플레이어 이름을 반환하거나 랜덤 생성하는 함수
    string GetUserId()
    {
        //(참고) 구글플레이 연동시 구글 아이디로 유저 아이디 가져옴
        string userId = PlayerPrefs.GetString("User_ID");

        // 유저 아이디가 NULL일 경우 랜덤 아이디 생성 
        if (string.IsNullOrEmpty(userId))
        {
            // 자릿수 맞춰서 반환
            userId = "USER_" + Random.Range(0, 999).ToString("000");
        }

        return userId;

    }

    // 포톤 클라우드는 Random Match making 기능 제공(로비 입장 후 이미 생성된 룸 중에서 무작위 선택해 입장)
    // 무작위 룸 접속(입장)에 실패한 경우 호출되는 콜백 함수
    void OnPhotonRandomJoinFailed()
    {
        //랜덤 매치 메이킹이 실패한 후 Console 뷰에 나타나는 메세지
        Debug.Log("No Rooms !!!");

        /* 룸 생성 함수
         * PhotonNetwork.CreateRoom( string roomName );
         * PhotonNetwork.CreateRoom( string roomName, RoomOptions roomOptions, TypedLobby typedLobby);
         * PhotonNetwork.CreateRoom( string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers);
         */

        /*
         ex) RoomOptions 클래스를 통해 룸의 속성을 셋팅

        bool isSucces = PhotonNetwork.JoinOrCreatRoom(
        "MyRoom",
        new RoomOptions()
        {
           MaxPlayers = 10,
           IsOpen = true;
           IsVisible = true,
        },
        null
        );
         */

        // Room : 동일한 공간(영역)에 접속, 서로 네트웨크 게임을 할 수 있는 Space
        // (CF) 포톤 클라우드 모니터링 웹페이지에선 Room을 Game이란 용어로 사용
        // 룸을 생성하는 함수(룸이 없으니까 내가 방장이 되어서 룸 생성)
        bool isSucces = PhotonNetwork.CreateRoom("MyRoom");

        Debug.Log("[정보] 게임 방 생성 완료 : " + isSucces);
    }

    //PhotonNetwork.CreatRoom 함수로 룸 생성시 실패한 경우는 대부분 룸의 이름 중복에 있다.
    //이건 경우 다음과 같이 OnPhotonCreatRooomFailed 콜백 함수가 호출, 이 함수에서 예외 처리 로직을 수행함
    //또, 다음과 같이 세부적인 오류 메세지를 받아올 수 있음. 사용자에게 전송해 오류 정보를 알림.
    //룸(방) 만들기에 실패한 경우 호출되는 콜백 함수
    void OnPhotonCreatRoomFailed(object[] codeAndMsg)
    {
        // 오류 코드(ErrorCode Class)
        Debug.Log(codeAndMsg[0].ToString());

        //오류 메세지
        Debug.Log(codeAndMsg[1].ToString());

        //여기선 디버그로 표현했지만 릴리즈 버전에서는 사용자에게 메세지 전달
        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
    }

    // 룸에 입장하면 호출되는 콜백 함수
    //  PhotonNetwork.CreatRoom 함수로 룸을 생성한 후 입장하거나
    //  PhotonNetwork.JoinRandomRoom, PhotonNetwork.JoinRoom 함수를 통해 입장해도 호출된다.
    void OnJoinedRoom()
    {
        Debug.Log("Enter Room");

        //플레이어를 생성하는 함수 호출(UI버전이라 일단 주석처리)
        //CreatPlayer();

        //룸 씬으로 전환하는 코루틴 실행
        StartCoroutine(this.LoadStage());
    }

    IEnumerator LoadStage()
    {
        // 씬 전환하는 동안 포톤 클라우드 서버로부터 네트워크 수신 중단(Instantiate, RPC 메세지, 모든 네트워크 이벤트 안받음)
        // 차후 전환된 scene의 초기화 설정 작업이 완료후 이 속성을 true로 변경
        PhotonNetwork.isMessageQueueRunning = false;

        // 백그라운드 씬 로딩
        AsyncOperation ao = SceneManager.LoadSceneAsync("scNetReady");

        //PhotonNetwork.LoadLevelAsync =>  PhotonNetwork.isMessageQueueRunning = false 로변경 후 인자로 전달된 씬을 로드

        // 씬 로딩이 완료될 때 까지 대기
        yield return ao;

        Debug.Log(" 로딩 완료 ");
    }

    public void OnClickJoinRandomRoom()
    {
        // 로컬 플레이어의 이름을 설정
        //PhotonNetwork.player.NickName = UserId.text;

        //무작위로 추출된 룸으로 입장
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnClickCreatRoom()
    {

        string _roomName = roomname.text;

        // 룸 이름이 없거나 NULL일 경우 룸 이름 지정
        if (string.IsNullOrEmpty(roomname.text))
        {
            _roomName = "ROOM_" + Random.Range(0, 999).ToString("000");
        }

        //PhotonNetwork.player.NickName = UserId.text;

        // 생성할 룸의 조건 설정 1
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.isVisible = true;
        roomOptions.MaxPlayers = 3;

        // 생성할 룸의 조건 생성2 (객체 생성과 동시에 멤버변수 초기화)
        // RoomOptions roomOptions = new RoomOptions() { IsOpen = true, IsVisible = true, MaxPlayers = 3 };

        // 지정할 조건에 맞는 룸 생성 함수
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);

    }


    //생성된 룸 목록이 변경되었을 때 호출되는 콜백함수(최초 룸 접속시 호출)
    void OnReceivedRoomListUpdate()
    {
        Debug.Log("리시브로그");
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOM_ITEM"))
        {
            Destroy(obj);
        }

        int rowCount = 0;

        // 스크롤 영역 초기화
        scrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;


        // 수신받은 룸 목록의 정보로 RoomItem 프리팹 객체를 생성
        // GetRoomList 함수는 RoomInfo 클래스 타입의 배열을 반환
        foreach (RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            Debug.Log("룸정보로그");
            Debug.Log(_room.Name);
            // RoomItem 프리팹을 동적으로 생성하자
            GameObject room = (GameObject)Instantiate(roomItem);

            //생성한 RoomItem 프리팹의 Parent를 지정
            room.transform.SetParent(scrollContents.transform, false);

            // 생성한 RoomItem에 룸 정보를 표시하기 위한 텍스트 정보 전달
            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayers = _room.MaxPlayers;

            roomData.DisplayRoomData();

            roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); Debug.Log("Room Click" + roomData.roomName); });

            scrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
            scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 10);
        }

    }

    void OnClickRoomItem(string roomName)
    {
        //로컬 플레이어의 이름을 설정
        //PhotonNetwork.player.NickName = UserId.text;

        // 인자로 전달된 이름에 해당하는 룸으로 입장
        PhotonNetwork.JoinRoom(roomName);
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }


    // 플레이어를 생성하는 함수 (UI 버전이라 주석처리)
    //void CreatPlayer()
    //{
    //    float pos = Random.Range(-100.0f, 100.0f);

    //    PhotonNetwork.Instantiate("MainPlayer", PlayerPos.Position, playerPos.rotation, 0);
    //}

}



