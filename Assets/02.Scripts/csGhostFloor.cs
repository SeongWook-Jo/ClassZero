using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csGhostFloor : MonoBehaviour
{
    //마지막 트리거 발동시간 -10초로 셋팅해놔야 시작부터 트리거 발동 가능
    private float lastTime = -50f;
    //재발동 대기시간
    private float coolTime = 50f;
    private AudioSource audio;
    //실행 될 클립
    public AudioClip sound;
    //자식에 있는 게임오브젝트
    private GameObject ghost;

    private PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        ghost = GetComponentInChildren<GameObject>();
        audio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (pv.isMine)
        {
            if (other.tag == "Player")
            {
                //20퍼센트 확률로 발동 1~100 랜덤 반환.
                if (Random.Range(1, 101) <= 20 && Time.time > lastTime + coolTime)
                {
                    pv.RPC("GhostPlay", PhotonTargets.All);
                    lastTime = Time.time;
                }
            }
        }
    }
    [PunRPC]
    void GhostPlay()
    {
        ghost.SetActive(true);
        audio.PlayOneShot(sound);
        //3초뒤 SetAcitve false로 변경하는 함수 실행
        Invoke("SetFalse", 3.0f);
    }
    private void SetFalse()
    {
        ghost.SetActive(false);
    }
}
