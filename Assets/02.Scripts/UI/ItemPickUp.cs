using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{

    public Item item;
    PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    public void ItemDestroy()
    {
        pv.RPC("NetItemDestroy", PhotonTargets.All);
    }
    [PunRPC]
    public void NetItemDestroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }


}

