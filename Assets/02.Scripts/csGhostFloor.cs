using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csGhostFloor : MonoBehaviour
{
    //마지막 트리거 발동시간 -10초로 셋팅해놔야 시작부터 트리거 발동 가능
    private float lastTime = -10f;
    //재발동 대기시간
    private float coolTime = 10f;
    private AudioSource myAudio;
    //실행 될 클립
    public AudioClip sound;
    //자식에 있는 게임오브젝트
    private GameObject ghost;
    private void Awake()
    {
        ghost = GetComponentInChildren<GameObject>();
        myAudio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //20퍼센트 확률로 발동 1~100 랜덤 반환.
        if (Random.Range(1, 101) < 20 && lastTime + coolTime > Time.time && other.tag == "Player")
        {
            ghost.SetActive(true);
            myAudio.PlayOneShot(sound);
            //3초뒤 SetAcitve false로 변경하는 함수 실행
            Invoke("SetFalse", 3.0f);
            lastTime = Time.time;
        }
    }
    private void SetFalse()
    {
        ghost.SetActive(false);
    }
}
