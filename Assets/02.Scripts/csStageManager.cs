using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class csStageManager : MonoBehaviour
{
    public Transform[] playerSpawners;
    private Transform pianoSpawner;
    private Transform computerSpawner;
    private Transform patrolOneSpawner;
    private Transform patrolTwoSpawner;
    private int count;
    Room room;
    PhotonView pv;
    public GameObject[] players;
    public int dieCk;

    public csClearPoint clearChk;

    //아이템 스폰 관련 변수들
    GameObject[] phoneSpawns;
    GameObject[] dollSpawns;
    GameObject[] backpackSpawns;
    GameObject[] sneakersSpawns;
    GameObject[] noteSpawns;
    GameObject[] sheetMusicSpawns;
    Transform phoneSpawnTr;
    Transform dollSpawnTr;
    Transform backpackSpawnTr;
    Transform sneakersSpawnTr;
    Transform noteSpawnTr;
    Transform sheetMusicSpawnTr;

    //단서 관련 변수들
    int clue1idx;
    int clue2idx;
    int clue3idx;
    int clue4idx;
    int clue5idx;
    int clue6idx;
    int clue7idx;
    int clue8idx;
    Transform[] clue1SpawnsTr;
    Transform[] clue2SpawnsTr;
    Transform[] clue3SpawnsTr;
    Transform[] clue4SpawnsTr;
    Transform[] clue5SpawnsTr;
    Transform[] clue6SpawnsTr;
    Transform[] clue7SpawnsTr;
    Transform[] clue8SpawnsTr;
    GameObject clue3;
    GameObject clue4;
    GameObject clue5;
    GameObject clue6;
    GameObject clue7;
    GameObject clue8;
    string[] cluetext = new string[8];

    //귀신의 최종 학년 반[0][1]에 저장
    string enemyGrade;

    private void Awake()
    {

        //씬 전환할 때 메세지 중지 넘어온 후 다시 받기.
        PhotonNetwork.isMessageQueueRunning = true;
        PhotonNetwork.room.IsVisible = false;
        pv = GetComponent<PhotonView>();
        PhotonNetwork.automaticallySyncScene = true;
        //GetComponentsInChildren은 부모객체부터 가져와서 1~3의 숫자로 할당해야함
        playerSpawners = GameObject.Find("PlayerSpawner").GetComponentsInChildren<Transform>();
        pianoSpawner = GameObject.Find("PianoSpwn").GetComponent<Transform>();
        computerSpawner = GameObject.Find("ComputerSpwn").GetComponent<Transform>();
        patrolOneSpawner = GameObject.Find("PatrolOneSpwn").GetComponent<Transform>();
        patrolTwoSpawner = GameObject.Find("PatrolTwoSpwn").GetComponent<Transform>();
        clearChk = GameObject.Find("CheckPoint").GetComponent<csClearPoint>();
        room = PhotonNetwork.room;
        phoneSpawns = GameObject.FindGameObjectsWithTag("PhoneSpawns");
        dollSpawns = GameObject.FindGameObjectsWithTag("DollSpawns");
        backpackSpawns = GameObject.FindGameObjectsWithTag("BackpackSpawns");
        sneakersSpawns = GameObject.FindGameObjectsWithTag("SneakersSpawns");
        noteSpawns = GameObject.FindGameObjectsWithTag("NoteSpawns");
        sheetMusicSpawns = GameObject.FindGameObjectsWithTag("SheetMusicSpawns");

        //단서 선언 나중에 이차원 배열로 변환 필요할듯? 너무 무식함
        clue1SpawnsTr = GameObject.Find("Clue1Spwn").GetComponentsInChildren<Transform>();
        clue2SpawnsTr = GameObject.Find("Clue2Spwn").GetComponentsInChildren<Transform>();
        clue3SpawnsTr = GameObject.Find("Clue3Spwn").GetComponentsInChildren<Transform>();
        clue4SpawnsTr = GameObject.Find("Clue4Spwn").GetComponentsInChildren<Transform>();
        clue5SpawnsTr = GameObject.Find("Clue5Spwn").GetComponentsInChildren<Transform>();
        clue6SpawnsTr = GameObject.Find("Clue6Spwn").GetComponentsInChildren<Transform>();
        clue7SpawnsTr = GameObject.Find("Clue7Spwn").GetComponentsInChildren<Transform>();
        clue8SpawnsTr = GameObject.Find("Clue8Spwn").GetComponentsInChildren<Transform>();
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //캐릭터 생성 코루틴 실행

        StartCoroutine(CreatePlayer());
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(CreateItem());
            StartCoroutine(CreateEnemy());
        }
        //플레이어 생성이 최대 3초 걸리기 때문에 3.5초뒤에 플레이어 목록을 받아옴. players를 기준으로 GameOver 씬 넘김 진행할 예정.
        yield return new WaitForSeconds(3.5f);
        players = GameObject.FindGameObjectsWithTag("Player");
        //패배 조건 할당
        StartCoroutine("Lose");
        //승리 조건 할당
        StartCoroutine("GameClear");
    }
    IEnumerator CreateItem()
    {
        //아이템 스폰 위치 지정 및 스폰
        phoneSpawnTr = phoneSpawns[Random.Range(0, phoneSpawns.Length)].transform;
        PhotonNetwork.InstantiateSceneObject("ItemPhone", phoneSpawnTr.position, phoneSpawnTr.rotation, 0, null);

        dollSpawnTr = dollSpawns[Random.Range(0, dollSpawns.Length)].transform;
        PhotonNetwork.InstantiateSceneObject("ItemDoll", dollSpawnTr.position, dollSpawnTr.rotation, 0, null);

        backpackSpawnTr = backpackSpawns[Random.Range(0, backpackSpawns.Length)].transform;
        PhotonNetwork.InstantiateSceneObject("ItemBackpack", backpackSpawnTr.position, backpackSpawnTr.rotation, 0, null);


        sneakersSpawnTr = sneakersSpawns[Random.Range(0, sneakersSpawns.Length)].transform;
        PhotonNetwork.InstantiateSceneObject("ItemSneakers", sneakersSpawnTr.position, sneakersSpawnTr.rotation, 0, null);

        noteSpawnTr = noteSpawns[Random.Range(0, noteSpawns.Length)].transform;
        PhotonNetwork.InstantiateSceneObject("ItemNote", noteSpawnTr.position, noteSpawnTr.rotation, 0, null);

        //단서는 Instantiate로 생성해야해서 RPC로 모두에게 단서 생성 함수 호출// 호출할 때 랜덤값 넣어서 동일한 위치에 생성될 수 있도록 진행
        //단서 1번에 대한 랜덤값 적용
        int clue1idx1 = Random.Range(1, 4);
        int clue1idx2 = Random.Range(4, 7);
        int clue1idx3 = Random.Range(7, 10);
        char enemyGrade = backpackSpawnTr.name[0];
        Debug.Log(enemyGrade);
        char enemyClass = backpackSpawnTr.name[2];
        Debug.Log(enemyClass);


        clue3idx = Random.Range(1, clue3SpawnsTr.Length);
        clue4idx = Random.Range(1, clue4SpawnsTr.Length);
        clue5idx = Random.Range(1, clue5SpawnsTr.Length);
        clue6idx = Random.Range(1, clue6SpawnsTr.Length);
        clue7idx = Random.Range(1, clue7SpawnsTr.Length);
        clue8idx = Random.Range(1, clue8SpawnsTr.Length);

        pv.RPC("ClueSpawn", PhotonTargets.All, clue1idx1, clue1idx2, clue1idx3, backpackSpawnTr.name);

        yield return null;
    }
    [PunRPC]
    public void ClueSpawn(int clue1idx1, int clue1idx2, int clue1idx3, string gradeClass)
    {
        char enemyGrade = gradeClass[0];
        Debug.Log(enemyGrade);
        char enemyClass = gradeClass[2];
        Debug.Log(enemyClass);

        cluetext[0] = "오늘은 서울 전학 첫날이다..그동안 아빠 때문에 전학을 꽤 많이 다녔지만\n이젠 고등학생이니까, 그냥 혼자라도 여기서 계속 지내겠다고 했다..."
                            + enemyGrade + "-" + enemyClass + "으로 반 배정을 받고 선생님과 함께 반으로 가면서 오래 볼 친구들이"
                            + "생길거란 생각에 설레이면서도 걱정이 되었다.잘 지낼 수 있겠지? ";

        cluetext[1] = "담임 선생님이 내 소개를 해주시려고 하는데 갑자기 정문에서 뵈었던 남자분이 선생님을 불렀다."
                        + "손전등을 계속 들고 다니셔서 경비 아저씬 줄 알았는데, 그분이 주임 선생님이라고 한다."
                        + "실수하기 전에 알아서 다행이다. ";

        cluetext[2] = "처음으로 3년간 지낼 학교니까, 체육복을 새것으로 구매했다."
                        + "지난 학교에서는 탈의실이 따로 있어서 체육복을 탈의실에 두고 다녔는데 여기는 그냥"
                        + "여자애들끼리 반에서 갈아입거나 화장실에 가서 갈아입는다고 한다."
                        + "체육복 용 가방에 넣어서 사물함에 넣어워야겠다.";

        cluetext[3] = "야자 시간에 주임 선생님에게 크게 혼나고 가방에 달아놨던 인형을 뺴앗겼다."
                        + "ㅇㅇㅇ,ㅁㅁㅁ가 내가 가지고 있는 인형이 자기들 거라고 거짓말을 했기 때문이다..."
                        + "선생님은 내 말을 믿어주지 않아서 너무 속상했다..ㅇㅇㅇ, ㅁㅁㅁ는 평소에 선생님들하고"
                        + "친하게 지내는 데다가 성적도 좋은 편이라 선생님들이 걔들을 신뢰한다."
                        + "그 인형은 전학 온 학교에서 친한 친구가 이별 선물로 준 것이었는데..너무 속상하다";

        cluetext[4] = "화장실에서(랜덤)반 애들이 내 욕을 하는 걸 들었다.내 뒷담을 하는 친구들이 있어서"
                        + "차마 나가지 못하고 있다가 종이 치고 나서야 헐레벌떡 반으로 뛰어갔다."
                        + "그러느라 휴대폰을 두고 온 것 같은데, 다음 쉬는 시간에 다시 가보니 휴대폰이 없어져 있었다."
                        + "아무리 찾아봐도 없었는데, 누가 가져간 것 같다. 오늘은 엄마한테 혼나겠네...";

        cluetext[5] = "수군대는 것이 듣기 싫어서 요즘은 음악실에 자주 가는 것 같다."
                        + "점심시간에 들러서 혼자 피아노를 쳤는데, 누가 올까봐 지레 놀라서 악보를 다 떨어트리고 말았다."
                        + "줍지 못하고 반으로 뛰어왔는데... 음악실에 비치되어 있던 악보인데 낱장이 모두 망가져서 곤란하게 되버렸다."
                        + "음악 선생님께 악보를 돌려드리로 했는데 원래 악보의 순서를 모르겠어서 어떻게 해야 할 지 모르겠다.";

        cluetext[6] = "우리반의 몇몇은 자주 컴퓨터실에 몰려가서 장난 메세지를 보내곤 한다. "
                        + "아마 별로 좋은 친구들은 아닌 것 같은 느낌이 들기 시작했다. "
                        + "수행 평가 때문에 가끔 컴퓨터실에 가야 할 때는 조심해서 빨리 다녀와야 할 듯 싶다.";

        cluetext[7] = "애들이랑 따로 체육 수업에 가려고 기다렸다가 나왔는데, 신발 주머니가 없다."
                        + "복도에서 헤메고 있는데 화장실 쪽에서 나온 애들이 키득대면서 지나간다..그냥 실내화를 신고 체육 수업에 가야겠다.";

        //Clue1 생성 로직
        Transform[] clue1Spawns = GameObject.Find("Clue1Spwn").GetComponentsInChildren<Transform>();
        GameObject clue1_1 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_1"), clue1Spawns[clue1idx1].transform.position, clue1Spawns[clue1idx1].transform.rotation);
        GameObject clue1_2 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_1"), clue1Spawns[clue1idx2].transform.position, clue1Spawns[clue1idx1].transform.rotation);
        GameObject clue1_3 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_1"), clue1Spawns[clue1idx3].transform.position, clue1Spawns[clue1idx1].transform.rotation);
        clue1_1.GetComponent<ItemPickUp>().item.clueText = cluetext[0];
        clue1_2.GetComponent<ItemPickUp>().item.clueText = cluetext[0];
        clue1_3.GetComponent<ItemPickUp>().item.clueText = cluetext[0];

        clue3 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_3"), clue3SpawnsTr[clue3idx].transform.position, clue3SpawnsTr[clue3idx].transform.rotation);
        clue4 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_4"), clue4SpawnsTr[clue4idx].transform.position, clue4SpawnsTr[clue4idx].transform.rotation);
        clue5 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_5"), clue5SpawnsTr[clue5idx].transform.position, clue5SpawnsTr[clue5idx].transform.rotation);
        clue6 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_6"), clue6SpawnsTr[clue6idx].transform.position, clue6SpawnsTr[clue6idx].transform.rotation);
        clue7 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_7"), clue7SpawnsTr[clue7idx].transform.position, clue7SpawnsTr[clue7idx].transform.rotation);
        clue8 = Instantiate<GameObject>(Resources.Load<GameObject>("Clue_8"), clue8SpawnsTr[clue8idx].transform.position, clue8SpawnsTr[clue8idx].transform.rotation);


        clue3.GetComponent<ItemPickUp>().item.clueText = cluetext[2];
        clue4.GetComponent<ItemPickUp>().item.clueText = cluetext[3];
        clue5.GetComponent<ItemPickUp>().item.clueText = cluetext[4];
        clue6.GetComponent<ItemPickUp>().item.clueText = cluetext[5];
        clue7.GetComponent<ItemPickUp>().item.clueText = cluetext[6];
        clue8.GetComponent<ItemPickUp>().item.clueText = cluetext[7];




    }
    IEnumerator GameClear()
    {
        while (true)
        {

            if (clearChk.itemCount >= 6)
            {
                yield return new WaitForSeconds(3.0f);
                PhotonNetwork.LoadLevel("scGameClear");
            }
            yield return null;
        }

    }

    IEnumerator Lose()
    {
        while (true)
        {
            dieCk = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<csPlayerCtrl>().isDie) dieCk += 1;
            }

            if (dieCk >= room.PlayerCount)
            {
                yield return new WaitForSeconds(3.0f);
                PhotonNetwork.LoadLevel("scGameOver");
            }
            yield return null;
        }
    }

    //캐릭터 생성 코루틴, 자기 자신은 1인칭 플레이어 생성 다른 사람에겐 3인칭 플레이어 생성.
    IEnumerator CreatePlayer()
    {
        //랜덤값 적용으로 다른 플레이어와 코루틴이 동시에 실행돼서 같은 위치에 생성되는 현상 방지
        yield return new WaitForSeconds(Random.Range(0.1f, 3.0f));
        //입장한 플레이어가 방에서 몇번째 플레이어인지 확인
        int myNumInRoom = (int)PhotonNetwork.player.CustomProperties["NumberInRoom"];
        //플레이어 랜덤 위치 선정을 위해 난수 생성
        int randNum;
        //이미 다른 플레이어가 생성된 곳이면 다시 난수생성
        while (true)
        {
            randNum = Random.Range(1, 4);
            if (playerSpawners[randNum].gameObject.activeSelf)
                break;
        }
        //몇번째 플레이어냐에 따라서 캐릭터는 고정, 생성 위치는 랜덤
        PhotonNetwork.Instantiate("Player" + myNumInRoom, playerSpawners[randNum].position, playerSpawners[randNum].rotation, 0);
        //이미 생성된 위치는 Setactive False해줌
        pv.RPC("PlaySpwnDisable", PhotonTargets.All, randNum);
        yield return null;
    }

    [PunRPC]
    void PlaySpwnDisable(int num)
    {
        playerSpawners[num].gameObject.SetActive(false);
    }

    IEnumerator CreateEnemy()
    {
        //yield return new WaitForSeconds(1f);
        PhotonNetwork.InstantiateSceneObject("EnemyPiano", pianoSpawner.position, pianoSpawner.rotation, 0, null);
        PhotonNetwork.InstantiateSceneObject("EnemyComputer", computerSpawner.position, computerSpawner.rotation, 0, null);
        PhotonNetwork.InstantiateSceneObject("EnemyPatrolOne", patrolOneSpawner.position, patrolOneSpawner.rotation, 0, null);
        PhotonNetwork.InstantiateSceneObject("EnemyPatrolTwo", patrolTwoSpawner.position, patrolTwoSpawner.rotation, 0, null);

        yield return null;
    }
}
