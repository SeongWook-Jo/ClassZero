using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private float downWalkSpeed = 2.0f;
    [SerializeField] private float slowWalkSpeed = 3.0f;
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float runSpeed = 8.0f;

    // 케릭터 이동속도
    public float movSpeed;
    // 케릭터 회전속도
    public float rotSpeed = 2.0f;

    //최대 상하 각도 조절 변수
    public float maxXRot = 80f;
    public float minXRot = -80f;
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

    //애니메이션을 위한 상태변수
    [HideInInspector] public bool isDie = false;
    private bool isSlow = false;
    private bool isDown = false;
    private bool isRun = false;
    private bool isMove = false;
    private bool isFind = false;
    private bool isPickup = false;
    private bool isStop = false;
    private bool isObserve = false;

    //Raycast를 위한 변수
    private Ray ray;
    private RaycastHit[] hitInfo;
    //현재 public으로 떼다 붙여서 구현했지만 private로 하고 스크립트로 가져오고싶음 참고사항
    public GameObject TrigPopup;
    public Text txtPopup;

    //미니맵 연동
    private Image imgMap;
    private RectTransform rectrPlayerDot;
    public Sprite[] sprMap;
    private Text txtMap;

    // 자신의 Transform 간단하게 쓰기위해 선언
    private Transform mytr;

    //옵저버용 카메라
    private Transform observeCamera;
    private GameObject[] observeTarget = { null, null };
    Quaternion cameraRot;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        nav = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        currPos = transform.position;
        currRot = transform.rotation;
        observeCamera = transform.Find("ObserveCamera");
        StartCoroutine(this.ActivePlayer());

        if (pv.isMine)
        {
            TrigPopup = transform.Find("Player_FP").Find("Canvas_User_UI").Find("ObjectTrigPopup").gameObject;
            txtPopup = TrigPopup.transform.Find("txtObjectName").GetComponent<Text>();
            imgMap = transform.Find("Player_FP").Find("Canvas_User_UI").Find("Minimap").Find("imgMap").GetComponent<Image>();
            txtMap = transform.Find("Player_FP").Find("Canvas_User_UI").Find("Minimap").Find("imgMap").Find("txtMap").GetComponent<Text>();
            rectrPlayerDot = imgMap.transform.Find("PlayerDot").GetComponent<RectTransform>();
            mytr = GetComponent<Transform>();
        }
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
    // Update is called once per frame
    void Update()
    {
        //죽으면 Update 함수 실행 종료 + 관전모드 활성화
        if (isDie)
        {
            if (pv.isMine)
            {
                CharacterState();
                if (isObserve && Input.GetMouseButtonDown(0))
                {
                    //다른 플레이어 관전으로 바꾸는 행위. // Setactive false + camera OnOff
                    if (!observeTarget[0].transform.Find("ObserveCamera").gameObject.activeSelf)
                    {
                        observeTarget[1].transform.Find("Player_TP").gameObject.SetActive(true);
                        observeTarget[1].transform.Find("ObserveCamera").gameObject.SetActive(false);
                        observeTarget[0].transform.Find("Player_TP").gameObject.SetActive(false);
                        observeTarget[0].transform.Find("ObserveCamera").gameObject.SetActive(true);
                    }
                    else
                    {
                        observeTarget[0].transform.Find("Player_TP").gameObject.SetActive(true);
                        observeTarget[0].transform.Find("ObserveCamera").gameObject.SetActive(false);
                        observeTarget[1].transform.Find("Player_TP").gameObject.SetActive(false);
                        observeTarget[1].transform.Find("ObserveCamera").gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                PlayerAnimState();
            }
            MouseOnOff();
            return;
        }

        if (!isStop)
        {
            //내부적으로 isMine 선언되어 있음
            CharacterMove();
            CharacterView();
        }

        MouseOnOff();

        if (pv.isMine)
        {
            //움직임 변경사항 적용
            MoveState();
            //캐릭터 State 변경하는 부분 함수로 구현 예정
            CharacterState();
            //미니맵업데이트
            MinimapUpdate();
            //Raycast 트리거
            PlayerRaycast();


        }
        //레이캐스트로 상호작용 오브젝트 판별
        else
        {
            PlayerAnimState();
        }
        //옵저버카메라 위치 동기화
        ObserveCamera();
    }
    [PunRPC]
    void ObserveCamera()
    {
        //OnPhotonSerializeView 에서 cameraRot값 넘겨준걸로 카메라 회전 값 동기화 // 살짝 끊기는 감이 없잖아 있음.
        if (pv.isMine)
        {
            cameraRot = playerCamera.localRotation;
        }
        observeCamera.localRotation = Quaternion.Lerp(observeCamera.localRotation, new Quaternion(cameraRot.x, observeCamera.localRotation.y, observeCamera.localRotation.z, observeCamera.localRotation.w), Time.deltaTime * 15.0f);

    }
    void MinimapUpdate()
    {
        rectrPlayerDot.anchoredPosition = new Vector2((mytr.position.x * 4.106f) + 66.8f, (mytr.position.z * 4.172f) + 78.5f);
        if (transform.position.y > 0.78f)
        {
            txtMap.text = "3층";
            imgMap.sprite = sprMap[2];
        }
        else if (transform.position.y > -3.4f)
        {
            txtMap.text = "2층";
            imgMap.sprite = sprMap[1];
        }
        else
        {
            txtMap.text = "1층";
            imgMap.sprite = sprMap[0];
        }
    }
    //레이캐스트
    void PlayerRaycast()
    {
        //아무것도 안띄우는걸 Default로 설정 
        TrigPopup.SetActive(false);
        ray.origin = playerCamera.transform.position;
        ray.direction = playerCamera.transform.forward;
        /*
        //else문을 두개 넣어줘야 Raycast가 아무것도 없거나 콜라이더가 문이 아닐때 둘 다 false로 만들어줌
        if (Physics.Raycast(ray, out hitInfo, 1f))
        {
            if (hitInfo.collider.tag == "Door")
            {
                txtPopup.text = "문 열림";
                TrigPopup.SetActive(true);
            }
            else
            {
                TrigPopup.SetActive(false);
            }
        }
        else
        {
            TrigPopup.SetActive(false);
        }
        */

        //Raycast로 진행 시 이상한 충돌로 인해 정상적으로 안되는 경우가 있음. 무겁지만 RaycastAll로 진행
        hitInfo = Physics.RaycastAll(ray, 1.0f);


        foreach (RaycastHit a in hitInfo)
        {
            if (a.collider.tag == "Door" || a.collider.tag == "Locker")
            {
                txtPopup.text = "";
                TrigPopup.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    //전체 플레이어에게 동기화를 해주기 위해 pv.RPC로 쏴줌.
                    //csTrigObj 스크립트 내에 함수 실행
                    a.collider.transform.parent.parent.SendMessage("DoorOnOff", null, SendMessageOptions.DontRequireReceiver);
                }
                return;
            }
            if (a.collider.tag == "Chair")
            {
                txtPopup.text = "";
                TrigPopup.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    a.collider.transform.SendMessage("ChairOnOff", null, SendMessageOptions.DontRequireReceiver);
                }
                return;
            }
            if (a.collider.tag == "OfficeChair")
            {
                txtPopup.text = "";
                TrigPopup.SetActive(true);
                if (Input.GetMouseButtonDown(0))
                {
                    a.collider.transform.SendMessage("OfficeChairOnOff", null, SendMessageOptions.DontRequireReceiver);
                }
                return;
            }
            if (a.collider.tag == "Swich")
            {
                txtPopup.text = "";
                TrigPopup.SetActive(true);
                //부모오브젝트에서 아래에 있는 모든 Light Component OnOff 발동
                if (Input.GetMouseButtonDown(0))
                {
                    //Light[] li = a.collider.transform.parent.GetComponentsInChildren<Light>();
                    a.collider.transform.parent.SendMessage("LightOnOff", null, SendMessageOptions.DontRequireReceiver);
                    //foreach (Light l in li)
                    //{
                    //    l.enabled = !l.enabled;
                    //}
                }
                return;
            }
            if (a.collider.tag == "Item")
            {

            }
        }


    }

    //키 입력에 따라서 앉기, 천천히 걷기, 달리기 등 변수 변경 -- 임시로 설정했으나 추후 회의 후 변경예정 // 이동 속도도 여기서 변경
    void MoveState()
    {
        //키입력이 있으면 is Move를 True로 변경 추후 isDie를 넣어야할듯?
        if (Mathf.Abs(hor) + Mathf.Abs(ver) > 0.1f)
            isMove = true;
        else
            isMove = false;


        //앉을 때 천천히 앉고 일어설 때 천천히 일어서도록 구현
        if (isDown)
        {
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, new Vector3(0, 0.75f, 0), Time.deltaTime * 3.0f);
        }
        else
        {
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, new Vector3(0, 1.43f, 0), Time.deltaTime * 3.0f);
        }


        //Shift, Ctrl, CapsLock 버튼에 따른 상태 변수 변경
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
        else if (Input.GetKeyDown(KeyCode.CapsLock) && !isSlow)
        {
            isSlow = true;
            isDown = false;
            isRun = false;
            movSpeed = slowWalkSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.CapsLock) && isSlow)
        {
            isSlow = false;
            isDown = false;
            isRun = false;
            movSpeed = walkSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && !isDown)
        {
            isDown = true;
            isSlow = false;
            isRun = false;
            movSpeed = downWalkSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && isDown)
        {
            isDown = false;
            isSlow = false;
            isRun = false;
            movSpeed = walkSpeed;
        }
    }

    //플레이어 상태에 따라 애니메이션 동기화 코루틴
    //넘겨준 NetAnim숫자에 따라 애니메이션 동작  -> 추후 동작에 따라 순서 변경
    void PlayerAnimState()
    {
        switch ((State)playerNetAnim)
        {
            case State.DIE:
                anim.Play("Die");
                break;
            case State.IDLE:
                anim.Play("Idle");
                break;
            case State.RUN:
                anim.Play("Run");
                break;
            case State.DOWN:
                anim.Play("Down");
                break;
            case State.DOWNWALK:
                anim.Play("DownWalk");
                break;
            case State.SLOWWALK:
                anim.Play("SlowWalk");
                break;
            case State.WALK:
                anim.Play("Walk");
                break;
            default:
                anim.Play("Idle");
                break;
        }
    }


    //캐릭터 State 변경하는 부분 함수로 구현 예정
    void CharacterState()
    {
        if (isDie)
        {
            playerState = State.DIE;
            playerNetAnim = (int)State.DIE;
        }
        else if (isMove)
        {
            if (isRun)
            {
                playerState = State.RUN;
                playerNetAnim = (int)State.RUN;
            }
            else if (isDown)
            {
                playerState = State.DOWNWALK;
                playerNetAnim = (int)State.DOWNWALK;
            }
            else if (isSlow)
            {
                playerState = State.SLOWWALK;
                playerNetAnim = (int)State.SLOWWALK;
            }
            else
            {
                playerState = State.WALK;
                playerNetAnim = (int)State.WALK;
            }
        }
        else if (isDown)
        {
            playerState = State.DOWN;
            playerNetAnim = (int)State.DOWN;
        }
        else
        {
            playerState = State.IDLE;
            playerNetAnim = (int)State.IDLE;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 7.0f);
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
                if (Cursor.visible == false)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
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

            /*
            이대로 구현하면 대각선 이동 시 빠르게 이동돼서 Normalize로 정규화 해주고 movSpeed를 곱해준다.
            hor = Input.GetAxis("Horizontal") * movSpeed;
            ver = Input.GetAxis("Vertical") * movSpeed;

            moveDirection = new Vector3(hor, 0, ver);
            normalized 구현
            */

            //GetAxis로 하게되면 -1.0f ~ 1.0f 사이의 값을 반환하기 때문에 -1,0,1을 반환하는 GetAxisRaw로 구현
            hor = Input.GetAxisRaw("Horizontal");
            ver = Input.GetAxisRaw("Vertical");

            //떠있을 때 중력적용
            if (controller.isGrounded) moveDirection = Vector3.Normalize(new Vector3(hor, 0, ver)) * movSpeed;
            else moveDirection = Vector3.Normalize(new Vector3(hor, -2, ver)) * movSpeed;

            // transform.TransformDirection 함수는 인자로 전달된 벡터를 
            // 월드좌표계 기준으로 변환하여 변환된 벡터를 반환해 준다.
            //즉, 로컬좌표계 기준의 방향벡터를 > 월드좌표계 기준의 방향벡터로 변환
            moveDirection = transform.TransformDirection(moveDirection);

            // CharacterController의 Move 함수에 방향과 크기의 벡터값을 적용(디바이스마다 일정)
            controller.Move(moveDirection * Time.deltaTime);
            #endregion
        }
        else
        {
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 7.0f);
        }
    }


    ////위치값,회전값,애니메이션값 전달
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //isMine과 동일
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(playerNetAnim);
            stream.SendNext(cameraRot);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            playerNetAnim = (int)stream.ReceiveNext();
            cameraRot = (Quaternion)stream.ReceiveNext();
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


    //죽었을 때 코루틴으로 DieAction 쏴줌.
    [ContextMenu("DieTest")]
    public void Die()
    {
        StartCoroutine("DieAction");
    }
    public IEnumerator DieAction()
    {
        isDie = true;

        if (pv.isMine)
        {
            //비디오 재생 스크립트 넣어야함

            //비디오 재생 후 다른 상대 카메라 On 시킬 예정 시간은 변경 예정
            yield return new WaitForSeconds(5f);

            //관전으로 넘어가는 부분
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (gameObject.name != players[i].name)
                {
                    if (observeTarget[0] == null)
                    {
                        observeTarget[0] = players[i];
                    }
                    else
                    {
                        observeTarget[1] = players[i];
                    }
                }
            }
            observeTarget[0].transform.Find("Player_TP").gameObject.SetActive(false);
            transform.Find("Player_FP").gameObject.SetActive(false);
            observeTarget[0].transform.Find("ObserveCamera").gameObject.SetActive(true);
            isObserve = true;
        }
        else
        {
            yield return new WaitForSeconds(5f);
            //애니메이션 x초 이후 캐릭터 없애야함
            transform.Find("Player_TP").gameObject.SetActive(false);
        }
    }

    public void PianoDie(Transform enemyTr)
    {
        if (pv.isMine)
        {
            isStop = true;
            playerCamera.LookAt(enemyTr);
        }
        Die();
    }
}


