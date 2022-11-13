using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csPianoKillCk : MonoBehaviour
{
    public GameObject[] players= new GameObject[3];
    private void Start()
    {
        players = new GameObject[3] { null, null, null };
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            for(int i = 0; i<players.Length; i++)
            {
                if(players[i] == null)
                {
                    players[i] = other.gameObject;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == other.gameObject)
                {
                    players[i] = null;
                }
            }
        }
    }
}
