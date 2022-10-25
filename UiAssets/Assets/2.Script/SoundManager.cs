using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//현재 스크립트, 현재 게임 오브젝트에서 반드시 필요한 컴포넌트는 Attribute로 명시해 자동생성, 삭제 막음
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioClip[] soundFile;

    public float soundVolume = 1.0f; //사운드 Volulme 설정 변수
    public bool isSoundMute = false; //사운드 Mute 설정 변수

    public Slider sl;
    public Toggle tg;

    public GameObject Sound; //sound 오브젝트 연결변수
    public GameObject PlaySoundBtn; //Sound ui버튼 오브젝트 연결 변수

    private AudioSource audio;  //오디오 소스의 오디오 사용 처음에 public/private

     void Awake()
    {
        audio = GetComponent<AudioSource>();
        //DontDestroyOnLoad(this.gameObject); // 씬 넘어가도 이 오브젝트는 계속 가져감

        LoadData(); //게임 로드(사운드 셋팅 저장된 값을 불러옴)
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData()
    {
        sl.value = PlayerPrefs.GetFloat("SOUNDVOLUME");

        //int형 데이터는 bool형으로 형변환
        tg.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("ISSOUNDMUTE"));

        //첫 세이브시 설정 -> 이 로직 없으면 첫 시작시 사운드 볼륨 0
        int isSave = PlayerPrefs.GetInt("ISSAVE");
        if (isSave == 0)
        {
            sl.value = 1.0f;
            tg.isOn = false;
            //첫 세이브는 soundVolume = 1.0; isSoundMute = false; 이 디폴트 값으로 저장 된다.
            //SaveData();
            PlayerPrefs.SetInt("ISSAVE", 1);
        }
    }
}
