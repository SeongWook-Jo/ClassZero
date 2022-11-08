using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csStageManager : MonoBehaviour
{
    public Transform[] playerSpawners;
    private int count;
    Room room;
    PhotonView pv;

    private void Awake()
    {
        //씬 전환할 때 메세지 중지 넘어온 후 다시 받기.
        PhotonNetwork.isMessageQueueRunning = true;
        pv = GetComponent<PhotonView>();
        playerSpawners = GameObject.Find("PlayerSpawner").GetComponentsInChildren<Transform>();
        room = PhotonNetwork.room;

        //캐릭터 생성 코루틴 실행
        StartCoroutine(this.CreatePlayer());
        StartCoroutine(CreateEnemy());
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    //캐릭터 생성 코루틴, 자기 자신은 1인칭 플레이어 생성 다른 사람에겐 3인칭 플레이어 생성.
    IEnumerator CreatePlayer()
    {
        PhotonNetwork.Instantiate("Player"+room.PlayerCount, playerSpawners[room.PlayerCount].position, playerSpawners[room.PlayerCount].rotation, 0);
        yield return null;
    }
    IEnumerator CreateEnemy()
    {
        yield return null;
    }
}
