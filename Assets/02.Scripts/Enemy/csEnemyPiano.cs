using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csEnemyPiano : MonoBehaviour
{
    public int playerCount;
    [SerializeField] private float pauseTime = 5f;
    private float lastPlayTime = -5;
    private AudioSource audio;
    //노래 재생 변수
    public bool isPlay;
    private GameObject[] killPlayers;
    private Transform pianoChair;
    private Animator anim;
    private PhotonView pv;


    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        killPlayers = GetComponentInChildren<csPianoKillCk>().players;
        pianoChair = GameObject.Find("props_149").GetComponent<Transform>();
        anim = GetComponentInChildren<Animator>();
        pv = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //초기화
        audio.playOnAwake = false;
        playerCount = 0;
        isPlay = false;
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어가 있을 때 노래 재생
        if (playerCount > 0) PianoPlay();
        else PianoStop();

        //노래 끝날 때까지 KillRange에 있으면 킬
        if (isPlay && audio.time > 90f)
        {
            Debug.Log("노래끝날때까지 있어서 사망");
            PianoKill();
        }
    }

    [ContextMenu("pkTest")]
    void PianoKill()
    {
        for (int i = 0; i < killPlayers.Length; i++)
        {
            if (killPlayers[i] != null)

                killPlayers[i].GetComponent<csPlayerCtrl>().PianoDie(transform);
        }

        //플레이어와 의자를 자연스럽게 뒤로 밀면서 애니메이션 재생할예정 3초뒤 앞으로 오는 모션도 포함
        StartCoroutine("Move");
        anim.SetTrigger("Kill");
    }

    //의자랑 캐릭터 움직이는 스크립트
    IEnumerator Move()
    {
        float tempT = Time.time;
        //처음 위치 저장
        Vector3 currEnemyPo = transform.localPosition;
        Vector3 currChairPo = pianoChair.localPosition;
        while (tempT + 0.6f > Time.time)
        {
            transform.localPosition += new Vector3(0.011f, 0, 0);
            pianoChair.localPosition += new Vector3(0, 0, 0.008f);
            yield return null;
        }
        //4초 후 처음위치로 다시 이동
        yield return new WaitForSeconds(4.0f);
        while (transform.localPosition.x > currEnemyPo.x || pianoChair.localPosition.z > currChairPo.z )
        {
            if (transform.localPosition.x > currEnemyPo.x) transform.localPosition -= new Vector3(0.011f, 0, 0);
            if (pianoChair.localPosition.z > currChairPo.z) pianoChair.localPosition -= new Vector3(0, 0, 0.008f);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("부딪혀서 사망");
            PianoKill();
        }
    }
    public void PianoPlay()
    {

        //10초 실행 5초 멈춤 코루틴으로 적용
        if (!isPlay)
        {
            StartCoroutine("PlayRoutine");
        }
        //연주 안할 때 플레이어가 사운드 내면 PianoKill
        for (int i = 0; i < killPlayers.Length; i++)
        {
            if (killPlayers[i] != null)
            {
                if (killPlayers[i].GetComponent<AudioSource>().isPlaying && !audio.isPlaying)
                {
                    Debug.Log("소리내서 사망");

                    PianoKill();
                }
            }
        }
    }
    IEnumerator PlayRoutine()
    {
        isPlay = true;
        while (isPlay)
        {
            audio.Play();
            anim.SetBool("Play", true);
            yield return new WaitForSeconds(10f);
            audio.Pause();
            anim.SetBool("Play", false);
            yield return new WaitForSeconds(5f);
        }

    }

    public void PianoStop()
    {
        //다음 노래 시작시 처음부터 시작 초기화
        audio.Stop();
        isPlay = false;
        anim.SetBool("Play", false);
        StopCoroutine("PlayRoutine");
    }

}
