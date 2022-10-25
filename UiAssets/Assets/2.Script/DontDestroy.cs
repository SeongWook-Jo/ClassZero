using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);  //씬 전환시 사라지지 않음
        SceneManager.LoadScene("scNetLobby");
    }
}
