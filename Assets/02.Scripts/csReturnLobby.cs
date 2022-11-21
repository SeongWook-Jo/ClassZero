using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csReturnLobby : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(5.0f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PhotonNetwork.LoadLevel("scNetLobby");
        PhotonNetwork.LeaveRoom();
    }
}
