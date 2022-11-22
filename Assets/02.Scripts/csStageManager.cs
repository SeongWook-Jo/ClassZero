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
    GameObject[] lockers;
    Transform[] itemSpawnersTr;

    private void Awake()
    {

        //씬 전환할 때 메세지 중지 넘어온 후 다시 받기.
        PhotonNetwork.isMessageQueueRunning = true;
        PhotonNetwork.room.IsVisible = false;
        pv = GetComponent<PhotonView>();
        //GetComponentsInChildren은 부모객체부터 가져와서 1~3의 숫자로 할당해야함
        playerSpawners = GameObject.Find("PlayerSpawner").GetComponentsInChildren<Transform>();
        pianoSpawner = GameObject.Find("PianoSpwn").GetComponent<Transform>();
        computerSpawner = GameObject.Find("ComputerSpwn").GetComponent<Transform>();
        patrolOneSpawner = GameObject.Find("PatrolOneSpwn").GetComponent<Transform>();
        patrolTwoSpawner = GameObject.Find("PatrolTwoSpwn").GetComponent<Transform>();
        clearChk = GameObject.Find("CheckPoint").GetComponent<csClearPoint>();
        room = PhotonNetwork.room;
        PhotonNetwork.automaticallySyncScene = true;

        //아이템스폰관련
        lockers = GameObject.FindGameObjectsWithTag("SpawnLockers");
        itemSpawnersTr = GameObject.Find("ItemSpawner").GetComponentsInChildren<Transform>();
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //캐릭터 생성 코루틴 실행

        StartCoroutine(CreatePlayer());
        StartCoroutine(CreateItem());
        if(PhotonNetwork.isMasterClient) StartCoroutine(CreateEnemy());
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
        Transform itemTr;
        //핸드폰 랜덤 스폰(락커에만)
        Transform phoneSpawnTr = lockers[Random.Range(0, lockers.Length)].transform;
       GameObject phone= PhotonNetwork.InstantiateSceneObject("Phone", phoneSpawnTr.position, phoneSpawnTr.rotation, 0, null);

        //phone.GetComponent<ItemPickUp>().item.clueText.text = " asdf"; 단서 예시.

        ////인형 랜덤 스폰
        //Transform dollSpawnTr = itemSpawners[Random.Range(0, itemSpawners.Length)].transform;
        //PhotonNetwork.InstantiateSceneObject("Doll", dollSpawnTr.position, dollSpawnTr.rotation, 0, null);

        //체육복 랜덤 스폰


        //악보 음악실 스폰


        //수행평가물 컴퓨터실 스폰


        //신발 화장실 아무대나 스폰



        GameObject test1 = GameObject.Find("test1");
        GameObject test2 = GameObject.Find("test2");
        PhotonNetwork.Instantiate("pig", test1.transform.position, Quaternion.identity, 0);
        Instantiate<GameObject>(Resources.Load<GameObject>("Pickaxe"), test1.transform.position, Quaternion.identity);
       
        yield return null;
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
