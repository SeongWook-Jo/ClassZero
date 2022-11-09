using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class InGameManager : MonoBehaviour
{
    public void OnClickSet()  // set(sound)..
    {
        SoundManager soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        soundmanager.OnClickSoundUi_Close();
    }

    public void OnClickReturnLobby()
    {
        // "안의 캔버스 컴포넌트 비활성화"
        //GameObject.Find("").GetComponent<Canvas>().enabled = false;

        SceneManager.LoadScene("scNetLobby");

        PhotonNetwork.LeaveRoom();
    }
}
