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

    [SerializeField] private float downWalkSpeed = 2.0f;
    [SerializeField] private float slowWalkSpeed = 3.0f;
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;

    // 케릭터 이동속도
    public float movSpeed;
    // 케릭터 회전속도
    public float rotSpeed = 2.0f;

    //최대 상하 각도 조절 변수
    public float maxXRot = 80;
    public float minXRot = -80;
    public float smoothTime = 20f;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private Transform playerCamera;
    //열거형 변수 선언
    State playerState = State.IDLE;

    //3인칭 캐릭터 애니메이션 변수
    int playerNetAnim;

    // 케릭터 이동 방향
    Vector3 moveDirection;

    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    float hor;
    float ver;


    public PhotonView pv;

    //상태변수
    [HideInInspector] public bool isDie = false;
    private bool isSlow = false;
    private bool isDown = false;
    private bool isRun = false;


    void Awake()
    {
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
        else
        {
            //플레이어 상태에 따라 애니메이션 동기화 코루틴 구현 예정
            StartCoroutine(this.PlayerAnimState());
        }
        isSlow = false;
        isDown = false;
        isRun = false;
        hor = 0;
        ver = 0;
        movSpeed = walkSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        //죽으면 Update 함수 실행 종료 추후 변경될 수 있음. + Coroutine까지 종료하거나 없애거나?
        if (isDie)
        {
            return;
            //StopAllCoroutines();
        }
        //추후 진행 예정 일단 3인칭 애니메이션 작동부터 확인.
        //CharacterMoveState();
        CharacterMove();
        CharacterView();
        MouseOnOff();
        MoveState();

        //테스트 Scene 뷰에서 레이캐스트
        TestRay();
        //캐릭터 State 변경하는 부분 함수로 구현 예정
        CharacterState();
    }

    //키 입력에 따라서 앉기, 천천히 걷기, 달리기 등 변수 변경 -- 임시로 설정했으나 추후 회의 후 변경예정 // 이동 속도도 여기서 변경
    void MoveState()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRun = true;
            isDown = false;
            isSlow = false;
            movSpeed = runSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRun = false;
            movSpeed = walkSpeed;
        }
        else if (Input.GetKey(KeyCode.CapsLock) && !isSlow)
        {
            isSlow = true;
            isDown = false;
            isRun = false;
            movSpeed = slowWalkSpeed;
        }
        else if (Input.GetKey(KeyCode.CapsLock) && isSlow)
        {
            isSlow = false;
            isDown = false;
            isRun = false;
            movSpeed = walkSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && !isDown)
        {
            isDown = true;
            isSlow = false;
            isRun = false;
            movSpeed = downWalkSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && isDown)
        {
            isDown = false;
            isSlow = false;
            isRun = false;
            movSpeed = walkSpeed;
        }
    }

    //플레이어 상태에 따라 애니메이션 동기화 코루틴
    //넘겨준 NetAnim숫자에 따라 애니메이션 동작  -> 추후 동작에 따라 순서 변경
    IEnumerator PlayerAnimState()
    {
        switch (playerNetAnim)
        {
            case 0:
                anim.SetTrigger("Die");
                break;
            case 1:
                anim.SetBool("Run", true);
                break;
            case 2:
                anim.SetBool("DownWalk", true);
                break;
            case 3:
                break;
            case 4:
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(0.1f);
    }

    //테스트레이
    void TestRay()
    {
        Debug.DrawRay(playerCamera.position, playerCamera.transform.forward * 3f);
    }

    //캐릭터 State 변경하는 부분 함수로 구현 예정
    void CharacterState()
    {
        if (isDie)
        {
            playerState = State.DIE;
            playerNetAnim = 0;
        }
        else if (controller.velocity != Vector3.zero)
        {
            if (isRun)
            {
                playerNetAnim = 1;
            }
            else if (isDown)
            {
                playerNetAnim = 2;
            }
            else if (isSlow)
            {
                playerNetAnim = 3;
            }
            else
            {
                playerNetAnim = 4;
            }
        }
        else
        {
            playerNetAnim = 5;
        }
    }



    //캐릭터 상태 열거형
    public enum State
    {
        IDLE = 0,
        SLOWWALK,
        DOWNWALK,
        WALK,
        RUN,
        DOOROPEN,
        DIE,
        SWITCHONOFF,
        ITEMPICKUP,
        ITEMFIND,
        CHAIR,
        CABINET,
        DOWN,
        //부적 먹었을 때 무적 발동 상태?
        RECOVERY
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

    //캐릭터 시야 회전 함수
    void CharacterView()
    {
        if (pv.isMine)
        {
            float yRot = Input.GetAxis("Mouse X") * rotSpeed;
            float xRot = Input.GetAxis("Mouse Y") * rotSpeed;

            //오일러와 쿼터니언에 대한 개념 필요 *상하 회전*
            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            // 좌우 회전
            transform.localRotation = Quaternion.Slerp(transform.localRotation, m_CharacterTargetRot,
                smoothTime * Time.deltaTime);
            // 상하 회전
            playerCamera.localRotation = Quaternion.Slerp(playerCamera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
        }
        else
        {
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 3.0f);
        }
    }

    // 마우스 OnOff 함수
    void MouseOnOff()
    {
        if (pv.isMine)
        {
            //ESC입력으로 마우스 보이게 하기
            if (Input.GetKey(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    //캐릭터 이동 함수
    void CharacterMove()
    {
        //네트워크상 자신일때만 캐릭터 조작
        if (pv.isMine)
        {
            #region 캐릭터 조작
            hor = Input.GetAxis("Horizontal") * movSpeed;
            ver = Input.GetAxis("Vertical") * movSpeed;

            moveDirection = new Vector3(hor, 0, ver);

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
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 3.0f);
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

    ////위치값,회전값,애니메이션값 전달
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

