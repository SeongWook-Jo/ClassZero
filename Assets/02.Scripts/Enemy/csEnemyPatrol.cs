using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class csEnemyPatrol : MonoBehaviour
{
    private NavMeshAgent nav;
    //플레이어들 불러오기
    private GameObject[] players;
    //추적할 거리
    public float traceDis = 5.0f;
    //죽이는 거리
    public float killDis = 2.0f;
    private Transform traceTarget;
    //다음순찰지역할당
    private int nextDestCk;
    //순찰루트
    private Transform[] baseTr;
    //순찰 목적지
    private Transform baseTarget;
    private Animator anim;
    //중복으로 목적지 체크되는 것 방지
    private float checkTime;
    private PhotonView pv;
    //Nav layer받아서 교실과 복도를 나눔
    private NavMeshHit navHit;
    //동기화 변수
    private Vector3 currPos;
    private Quaternion currRot;
    private int netAnim;
    public EnemyKind enemyKind;
    private void Awake()
    {
        //네비연결
        nav = GetComponent<NavMeshAgent>();
        //플레이어 인원 확인
        //순찰루트받기
        baseTr = GameObject.Find("PatrolSpwn").GetComponentsInChildren<Transform>();
        anim = GetComponentInChildren<Animator>();
        pv = GetComponent<PhotonView>();
        if (!PhotonNetwork.isMasterClient)
        {
            nav.enabled = false;
        }
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //바로 실행하면 players 못불러와서 시간 주고 실행.
        yield return new WaitForSeconds(3.0f);
        if (pv.isMine)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            //초기화
            if (enemyKind == EnemyKind.ONE) { baseTarget = baseTr[1]; nextDestCk = -1; }
            else { baseTarget = baseTr[baseTr.Length-1]; nextDestCk = 1; }
            

            traceTarget = players[0].transform;
            
            //범위안의 적 추적 다른 층은 못찾아감 + 계단도 못따라감.
            StartCoroutine("EnemyMovement");
        }
    }
    void Update()
    {
        //다음위치 순찰
        //if(Vector3.Distance(transform.position, baseTarget.position) < 0.1f) NextBase();
        if (pv.isMine)
        {
            currPos = transform.position;
            currRot = transform.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 3.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 3.0f);
            NetAnimSync();
        }

    }
    //플레이어 위치 체크 추적
    IEnumerator EnemyMovement()
    {
        while (true)
        {
            for (int i = 0; i < players.Length; i++)
            {
                NavMesh.Raycast(players[i].transform.position, players[i].transform.position + new Vector3(0, 1f, 0), out navHit, 5);
                //플레이어가 같은 층이고 추적거리 내에있으면.
                if (Mathf.Abs(transform.position.y - players[i].transform.position.y) < 0.1f && Vector3.Distance(transform.position, players[i].transform.position) < traceDis)
                {
                    //타겟이 다른층이거나 다른 적이 더 가까우면. 타겟과 Enemy의 거리가 더 가깝더라도 다른층일 수 있음.
                    if (Mathf.Abs(transform.position.y - traceTarget.position.y) < 0.1f || Vector3.Distance(transform.position, players[i].transform.position) < Vector3.Distance(transform.position,
                        traceTarget.position) && players[i].tag == "Player" && navHit.mask == 1)
                    {
                        traceTarget = players[i].transform;
                    }
                }
            }

            //테스트 결과 복도에서는 navHit.mask가 1 클래스룸에서는 0으로 나옴,, 추후 추적하는 것도 층별로 레이어 줘서 진행하면 더 좋을듯.
            NavMesh.Raycast(traceTarget.position, traceTarget.position + new Vector3(0, 1f, 0), out navHit, 5);


            if (Vector3.Distance(transform.position, traceTarget.position) < killDis && navHit.mask == 1)
            {
                traceTarget.GetComponent<csPlayerCtrl>().Die();
                nav.isStopped = true;
                AnimSync("Kill");
                yield return new WaitForSeconds(5.0f);
                anim.SetTrigger("End");
                players = GameObject.FindGameObjectsWithTag("Player");
                traceTarget = players[0].transform;
                nav.isStopped = false;
            }
            //최종적으로 같은층 + 범위내면 추적
            else if (Mathf.Abs(transform.position.y - traceTarget.position.y) < 1f && Vector3.Distance(transform.position, traceTarget.position) < traceDis && navHit.mask == 1)
            {
                nav.SetDestination(traceTarget.position);
                AnimSync("Run");
            }
            else
            {
                nav.SetDestination(baseTarget.position);
                AnimSync("Walk");
                if (nav.remainingDistance < 0.1f && Time.time > checkTime + 2.0f)
                {
                    NextBase();
                    checkTime = Time.time;
                }
            }
            yield return null;
        }
    }
    //애니메이션 변경 및 쏴주기
    void AnimSync(string animState)
    {
        if (animState == "Kill")
        {
            anim.Play("Kill");
            netAnim = 1;
        }
        else if (animState == "Run")
        {
            anim.Play("Run");
            netAnim = 2;
        }

        else
        {
            anim.Play("Walk");
            netAnim = 3;
        }
    }

    //네트워크애니메이션 동기화
    void NetAnimSync()
    {
        if (netAnim == 1)
        {
            anim.Play("Kill");
        }
        else if (netAnim == 2)
        {
            anim.Play("Run");
        }
        else
        {
            anim.Play("Walk");
        }
    }

    //다음 baseTarget으로 넘김
    void NextBase()
    {
        nextDestCk += 1;
        if (nextDestCk % 5 == 0)
        {
            baseTarget = baseTr[Random.Range(1, 5)];
        }
        else if (nextDestCk % 5 == 1)
        {
            baseTarget = baseTr[Random.Range(5, 8)];
        }
        else if (nextDestCk % 5 == 2)
        {
            baseTarget = baseTr[Random.Range(8, baseTr.Length)];
        }
        else if (nextDestCk % 5 == 3)
        {
            baseTarget = baseTr[Random.Range(5, 8)];
        }
        else if (nextDestCk % 5 == 4)
        {
            baseTarget = baseTr[Random.Range(1, 5)];
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(netAnim);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            netAnim = (int)stream.ReceiveNext();

        }
    }
    public enum EnemyKind
    {
        ONE = 0,
        TWO
    }
}
