using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//현재 스크립트,현재 게임오브젝트에서 반드시 필요로하는 컴퍼넌트 Attribute로 명시, 자동생성및 삭제 막음
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioClip[] soundFile; //오디오 클립 저장 배열 

    public float soundVolume = 1.0f;  // 사운드 볼륨 설정 변수
    public bool isSoundMute = false;  // 사운드 뮤트 설정 변수

    public Slider bgm_sl;   
    //public Slider sEffect_sl;

    public Toggle bgm_tg;
    //public Toggle sEffect_tg;

    public GameObject Sound;  // Sound 오브젝트 연결
    //public GameObject PlaySoundBtn; //Sound ui 버튼 추후 ESC 버튼 연결?

    private AudioSource audio;  //오디오 소스의 사용 처음에 public / private

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        //DontDestroyOnLoad(this.gameObject);  ////씬이 넘어가도 이 오브젝트를 계속 가져감

        LoadData();  // 게임 로드(사운드 셋팅 저장된 값을 불러옴)
    }

    void Start()
    {
        soundVolume = bgm_sl.value;
        //soundVolume = sEffect_sl.value;

        isSoundMute = bgm_tg.isOn; // 뮤트 토글박스에 체크표시가 되어야 Mute
        //isSoundMute = sEffect_tg.isOn;

        //PlaySoundBtn.SetActive(true);  //오픈 씬에서 비활, 누르면 활성화

        AudioSet();

    }

    // Slider와 Toggle 컴퍼넌트에서 이벤트 발생시 호출할 함수 (public 키워드로 외부접근 가능하게함)
    //호출시 초기화
    public void SetSound()
    {
        soundVolume = bgm_sl.value;
        //soundVolume = sEffect_sl.value;

        isSoundMute = bgm_tg.isOn;
        //isSoundMute = sEffect_tg.isOn;

        AudioSet();

    }

    void AudioSet()
    {
        audio.volume = soundVolume; // audioSource의 볼륨 셋팅
        audio.mute = isSoundMute;   // audioSource의 Mute 셋팅
    }

    //버튼 연결
    //사운드 UI 오픈
    public void SoundUiOpen()//버튼으로 만든건 OnClick~~으로 만드는게 관례다
    {
        Sound.SetActive(true);
        //PlaySoundBtn.SetActive(false);
    }

    public void OnClickSoundUi_Close()
    {
        Sound.SetActive(false);
        //PlaySoundBtn.SetActive(true);

        SaveData(); // 게임 세이브 
    }

    // 사운드 멀어지고 죽는 객체 쪽에서 소리가 나야함
    // 스테이지 시작시 호출되는 함수
    public void PlayBackground(int stage) //스테이지별 인덱스로 다른 사운드 재생
    {
        //audioSource 사운드 연결
        //GetComponent<AudioSource>().clip = soundFile[stage - 1];
        audio.clip = soundFile[stage - 2]; // soundFile 배열에 받은 audioclip의 두번째를 재생. 1번은 StartBGM 2번 PlayBGM

        //audiosource 셋팅
        AudioSet();

        //사운드 플레이 Mute설정시 사운드 안나옴
        //GetComponent<AudioSource>().Play();
        audio.Play();

    }

    //// 가까이가면 소리 커지고 멀어지면 작아짐..
    //public void PlayEffect(Vector3 pos, AudioClip sfx)
    //{
    //    // Mute 옵션 설정시 이 함수를 바로 빠져 나감. 음소거
    //    if(isSoundMute)
    //    {
    //        return;
    //    }

    //    GameObject _soundObj = new GameObject("sfx");
    //    _soundObj.transform.position = pos; //사운드 발생 위치 지정

    //    //생성한 게임오브젝트에 audioSource 컴퍼넌트 추가
    //    AudioSource _audioSource = _soundObj.AddComponent<AudioSource>();

    //    //Audiosource 속성 설정
    //    _audioSource.clip = sfx; // 사운드 파일 연결
    //    _audioSource.volume = soundVolume; // 설정되어있는 볼륨 적용 / soundVolume으로 게임 전체 사운드 볼륨 조절
    //    _audioSource.minDistance = 15.0f;  //사운드 3d 셋팅에 최소 범위 설정
    //    _audioSource.maxDistance = 30.0f;  //사운드 3d 셋팅에 최대 범위 설정

    //    _audioSource.Play();    // 사운드 실행

    //    Destroy(_soundObj, sfx.length + 0.2f);  //모든 사운드가 플레이 종료되면 동적생성 게임오브젝트 삭제
    //}

    // 게임 사운드의 볼륨, 뮤트 값등 정보 저장
    public void SaveData()
    {
        PlayerPrefs.SetFloat("SOUNDVOLUME", soundVolume);
        //PlayerPrefs 클래스 내부에 bool형 저장 함수 없음
        //bool형 데이터는 형변환을 해야 PlayerPrefs.setInt()함수 사용가능
        PlayerPrefs.SetInt("ISSOUNDMUTE", System.Convert.ToInt32(isSoundMute));
    }

    // 게임 사운드 데이터  불러오기
    // 바로 사운드 Ui 슬라이드와 토글에 적용
    public void LoadData()
    {
        bgm_sl.value = PlayerPrefs.GetFloat("SOUNDVOLUME");
        //sEffect_sl.value = PlayerPrefs.GetFloat("SOUNDVOLUME");

        //int형 데이터는 bool형으로 형변환
        bgm_tg.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("ISSOUNDMUTE"));
        //sEffect_tg.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("ISSOUNDMUTE"));

        //첫 세이브시 설정 -> 이 로직 없으면 첫 시작 사운드 볼륨0
        int isSave = PlayerPrefs.GetInt("ISSAVE");
        if(isSave ==0)
        {
            bgm_sl.value = 1.0f;
            //sEffect_sl.value = 1.0f;

            bgm_tg.isOn = false;
            //sEffect_tg.isOn = false;

            //첫 세이브는 soundVolume = 1,0f;  isSoundMute = false; 이 디폴트 값으로 저장됌
            SaveData();
            PlayerPrefs.SetInt("ISSAVE", 1);
        }
            
    }





}

