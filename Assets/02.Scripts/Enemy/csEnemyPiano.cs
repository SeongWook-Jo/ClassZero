using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csEnemyPiano : MonoBehaviour
{
    [HideInInspector]public int playerCount;
    // Start is called before the first frame update
    void Start()
    {
        //초기화
        playerCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCount > 0) PianoPlay();
        else PianoStop();
            
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //other.GetComponent<csPlayerCtrl>().Die();
        }
    }
    public void PianoPlay()
    {

    }

    public void PianoStop()
    {

    }

}
