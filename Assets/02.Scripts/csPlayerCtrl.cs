using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]


public class csPlayerCtrl : MonoBehaviour
{
    // CharacterController 컴포넌트를 위한 레퍼런스
    CharacterController controller;
    NavMeshAgent nav;
    //3인칭 캐릭터 애니메이터 연결
    private Animator anim;

    // 중력 
    public float gravity = 300.0f;

    // 케릭터 이동속도
    public float movSpeed = 5.0f;
    // 케릭터 회전속도
    public float rotSpeed = 2.0f;

    //최대 상하 각도 조절 변수
    public float maxXRot = 80;
    public float minXRot = -80;
    public float smoothTime = 20f;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private Transform playerCamera;
    PlayerState playerState;

    //3인칭 캐릭터 애니메이션 변수
    int playerNetAnim;

    // 케릭터 이동 방향
    Vector3 moveDirection;

    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    public PhotonView pv;

    //상태변수
    [HideInInspector] public bool isDie = false;

    void Awake()
    {
        transform.Find("Player_FP").gameObject.SetActive(false);
        transform.Find("Player_TP").gameObject.SetActive(false);
        pv = GetComponent<PhotonView>();
        nav = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        currPos = transform.position;
        currRot = transform.rotation;
        StartCoroutine(this.ActivePlayer());
    }

    // Start is called before the first frame update
    void Start()
    {
        if (pv.isMine)
        {
            playerCamera = Camera.main.transform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            m_CharacterTargetRot = transform.localRotation;
            m_CameraTargetRot = playerCamera.localRotation;
        }
    }

    //1인칭, 3인칭 캐릭터 활성화
    IEnumerator ActivePlayer()
    {
        //자기자신은 FP 나머지는 TP를 SetActive
        //PhotonNetwork.Instantiate로 생성하면 문제생겨서 SetActive로 변경
        if (pv.isMine)
        {
            transform.Find("Player_FP").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Player_TP").gameObject.SetActive(true);
            //3인칭 캐릭터 애니메이션 연결
            anim = GetComponentInChildren<Animator>();
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        //캐릭터 조작 함수
        CharacterControl();


        if(controller.velocity != Vector3.zero)
        {
            if (!pv.isMine)
                anim.SetFloat("Move", 0.5f);
        }
        //캐릭터 상태
        //CharacterState();


        ////캐릭터 애니메이션 함수
        //if (!pv.isMine)
        //{
        //    CharacterAnim();
        //}
    }

    //캐릭터 상태 열거형
    public enum PlayerState
    {
        IDLE = 0,
        DIE,
        MOVE,
        PICKUP,


    }

    //캐릭터 상태 셋팅
    //void CharacterState()
    //{
    //    if (controller.velocity != Vector3.zero)
    //    {
    //        playerState = PlayerState.MOVE;
    //        playerNetAnim = 0;
    //        if(pv.isMine)
    //        {
    //            anim.SetFloat("Move", 0.5f);
    //        }
    //    }
    //}

    //3인칭 캐릭터만 애니메이션이 재생되기 때문에  playerNetAnim 값으르 기준으로 한다.
    //void CharacterAnim()
    //{
    //    switch (playerNetAnim)
    //    {
    //        case 0:
    //            anim.SetFloat("Move", Mathf.Abs(controller.velocity.z));
    //            break;
    //    }
    //}
    //캐릭터 조작 함수
    void CharacterControl()
    {
        //네트워크상 자신일때만 캐릭터 조작
        if (pv.isMine)
        {
            //ESC입력으로 마우스 보이게 하기
            if (Input.GetKey(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            #region 캐릭터 조작
            float hor = Input.GetAxis("Horizontal") * movSpeed;
            float ver = Input.GetAxis("Vertical") * movSpeed;
            float yRot = Input.GetAxis("Mouse X") * rotSpeed;
            float xRot = Input.GetAxis("Mouse Y") * rotSpeed;


            moveDirection = new Vector3(hor, 0, ver);

            //오일러와 쿼터니언에 대한 개념 필요 *상하 회전*
            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);


            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            playerCamera.localRotation = Quaternion.Slerp(playerCamera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);

            // transform.TransformDirection 함수는 인자로 전달된 벡터를 
            // 월드좌표계 기준으로 변환하여 변환된 벡터를 반환해 준다.
            //즉, 로컬좌표계 기준의 방향벡터를 > 월드좌표계 기준의 방향벡터로 변환
            moveDirection = transform.TransformDirection(moveDirection);


            // 디바이스마다 일정 속도로 케릭에 중력 적용
            moveDirection.y -= gravity * Time.deltaTime;
            // CharacterController의 Move 함수에 방향과 크기의 벡터값을 적용(디바이스마다 일정)
            controller.Move(moveDirection * Time.deltaTime);
            #endregion
        }
        else
        {
            //Debug.Log(currPos + "currPos위치값");
            //Debug.Log(transform.position + "transPos위치값");

            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 3.0f);
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 3.0f);

        }
    }
    //X값 회전에 제한을 주기 위한 함수
    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, minXRot, maxXRot);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    ////위치값과 회전값 전송
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("탄다");
        //isMine과 동일
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(playerNetAnim);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            playerNetAnim = (int)stream.ReceiveNext();
        }
    }


}

