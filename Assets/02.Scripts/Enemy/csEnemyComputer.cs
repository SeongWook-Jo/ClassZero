using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csEnemyComputer : MonoBehaviour
{
    [SerializeField] private GameObject[] players = { null, null, null };
    [SerializeField] private GameObject[] screenQuadObj;
    [SerializeField] private MeshRenderer[] screenQuad;
    [SerializeField] private int playerCount = 0;
    [SerializeField] private bool isPlay = false;
    [SerializeField] private float inTime;
    [SerializeField] private AudioSource audio;
    private void Awake()
    {
        //모든 스크린 불러오기 처음엔 false로 되어있음
        screenQuadObj = GameObject.FindGameObjectsWithTag("ScreenQuad");
        audio = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //초기화
        playerCount = 0;
        isPlay = false;
        screenQuad = new MeshRenderer[screenQuadObj.Length];
        for (int i = 0; i < screenQuadObj.Length; i++)
        {
            screenQuad[i] = screenQuadObj[i].GetComponent<MeshRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //계속 코루틴이 실행되면 안돼서 isPlay 넣음.
        if (playerCount > 0 && !isPlay)
        {
            inTime = Time.time;
            isPlay = true;
            StartCoroutine("ScreenOnOff");
            StartCoroutine("SoundOnOff");
        }
        //플레이어 없을 때
        if (playerCount == 0)
        {
            StopAllCoroutines();
            isPlay = false;
            foreach (MeshRenderer m in screenQuad)
            {
                m.enabled = false;
            }
        }

        //진입하고 40초 이후에 사망
        if (Time.time > inTime + 40f && isPlay)
        {
            ComputerKill();
        }
    }

    //PlayerKill 함수
    void ComputerKill()
    {
        //죽인 뒤 초기화
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                players[i].GetComponent<csPlayerCtrl>().Die();
                players[i] = null;
            }
        }
        playerCount = 0;
    }

    //스크린 온 오프 1초 기준
    IEnumerator ScreenOnOff()
    {
        yield return new WaitForSeconds(5.0f);
        while (playerCount > 0)
        {
            foreach (MeshRenderer m in screenQuad)
            {
                m.enabled = !m.enabled;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    //사운드 온 오프
    IEnumerator SoundOnOff()
    {
        yield return new WaitForSeconds(5.0f);
        while (playerCount > 0)
        {
            Debug.Log("사운드재생");
            audio.Play();
            yield return new WaitForSeconds(3.0f);
        }
    }

    //플레이어 들어올 때 카운트 체크 및 배열에 넣어서 나중에 Kill할때 사용
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerCount += 1;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == null)
                {
                    players[i] = other.gameObject;
                    break;
                }
            }
        }
    }
    //플레이어 나갈 때 카운트 체크
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerCount -= 1;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == other.gameObject)
                {
                    players[i] = null;
                    break;
                }
            }
        }
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
