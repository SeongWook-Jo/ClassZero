using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backSound : MonoBehaviour
{
    //스테이지
    public int stage;

    //SoundManager컴포넌트를 연결할 변수 -> 인스펙터 뷰에는 꼭 필요한 변수만 노출시킴
    private SoundManager _sMgr;

    //테스트 변수
    //public AudioClip soundClip;
    //private float soundTime;

    private void Awake()
    {
        //SoundManger 게임오브젝트의  SoundManger 컴포넌트 연결 
        _sMgr = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        //  //로딩 완료후 설명서 비활성화 사운드 ui활성화
        //  play매니져 끝에서 해줬던걸 씬 로드되서 해줄때는 밑에서처럼
        // GameObject.Find ("ExPlainUi").gameObject.SetActive (false);
        // GameObject.Find ("SoundCanvas").GetComponent<Canvas> ().enabled = true;

        //배경 사운드 플레이
        _sMgr.PlayBackground(stage);


    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Time.time > soundTime)
    //    {
    //        //3.5초마다 번개사운드 연출
    //        LightningSound();

    //        soundTime = Time.time + 3.5f;
    //    }
    //}

    ////병렬 처리를 위한 코루틴 함수 호출
    //void LightningSound()
    //{
    //    StartCoroutine(this.PlayEffctSound(soundClip)); //StartCoroutine으로 코루틴함수호출
    //}

    //Effect  테스트 사운드를 Coroutine으로 생성
    //IEnumerator PlayEffctSound(AudioClip _clip)
    //{
    //    // 공용 사운드 함수 호출
    //    _sMgr.PlayEffct(transform.position, _clip);
    //    yield return null;
    //}
}
