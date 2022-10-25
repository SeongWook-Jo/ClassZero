using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyManager : MonoBehaviour
{
    // Lobby로 되돌아감
    public void ReturnLobby()
    {
        // "안의 캔버스 컴포넌트 비활성화"
        //GameObject.Find("").GetComponent<Canvas>().enabled = false;

        SceneManager.LoadScene("scNetLobby");
    }
}
