using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csPianoMusicCk : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            transform.parent.GetComponent<csEnemyPiano>().playerCount += 1;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            transform.parent.GetComponent<csEnemyPiano>().playerCount -= 1;
    }

}
