using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTrigObj : MonoBehaviour
{
    PhotonView pv;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DoorOnOff()
    {
        pv.RPC("DoorAnim", PhotonTargets.All);
    }

    public void LightOnOff()
    {
        pv.RPC("LightAnim", PhotonTargets.All);
    }
    [PunRPC]
    void LightAnim()
    {
        Light[] li = GetComponentsInChildren<Light>();
        foreach(Light l in li)
        {
            l.enabled = !l.enabled;
        }
    }

    [PunRPC]
    void DoorAnim()
    {
        anim.SetTrigger("OnOff");
    }
}
