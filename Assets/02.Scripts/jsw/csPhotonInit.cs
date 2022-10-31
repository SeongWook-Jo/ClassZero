//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;  

//public class csPhotonInit : MonoBehaviour
//{
//    //App의 버전 정보 
//    public string version = "Ver 0.1.0";

//    //네트워크 로그를 모두 확인하기 위해 설정
//    public PhotonLogLevel LogLevel = PhotonLogLevel.Full;

//    void Awake()
//    {
//        //연결이 안됐으면 연결 -> 인게임에서 로비로 나왔을 때 재연결되면 안돼서
//        if (!PhotonNetwork.connected)
//        {
//            PhotonNetwork.ConnectUsingSettings(version);

//            PhotonNetwork.logLevel = LogLevel;

//            //현재 클라이언트 유저의 이름을 포톤에 설정
//            //PhotonView 컴포넌트의 요소 Owner의 값이 된다.
//            PhotonNetwork.playerName = "GUEST " + Random.Range(1, 9999);
//        }
//    }
//    private void Start()
//    {
//        Debug.Log("게임실행");
//    }


//    //로비 접속 시 호출되는 콜백함수. 포톤 Auto-join Lobby 꼭 체크
//    void OnJoinedLobby()
//    {
//        //로비 접속 확인
//        Debug.Log("Joined Lobby !!!");

//        //테스트용 Room 조인, 실패시 방 생성
//        PhotonNetwork.JoinRandomRoom();
//     }

//    //포톤 클라우드는 Random Match Making 기능 제공(로비 입장 후 이미 생성된 룸 중에서 무작위로 선택해 입장)
//    //무작위 룸 접속(입장)에 실패한 경우 호출되는 콜백 함수 
//    void OnPhotonRandomJoinFailed()
//    {

//        //테스트용 룸을 생성하는 함수 (룸이 없으니깐 내가 방장이되서 룸 생성)
//        bool isSucces = PhotonNetwork.CreateRoom("MyRoom");
//        Debug.Log("[정보] 게임 방 생성 완료 : " + isSucces);
//    }

//    //룸 생성 실패시 호출되는 콜백함수
//    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
//    {
//        //오류 코드( ErrorCode Class )
//        Debug.Log(codeAndMsg[0].ToString());
//        //오류 메시지
//        Debug.Log(codeAndMsg[1].ToString());
//        // 여기선 디버그로 표현했지만 릴리즈 버전에선 사용자에게 메시지 전달
//        Debug.Log("Create Room Failed = " + codeAndMsg[1]);
//    }

//    //룸에 들어갔을 때 실행되는 콜백함수 씬 이동
//    void OnJoinedRoom()
//    {
//        Debug.Log("Enter Room");
//        StartCoroutine(this.LoadStage());
//    }

//    IEnumerator LoadStage()
//    {
//        PhotonNetwork.isMessageQueueRunning = false;

//        AsyncOperation ao = SceneManager.LoadSceneAsync("scTestIngame");

//        yield return ao;

//        Debug.Log("로딩 완료");

//    }
//}
