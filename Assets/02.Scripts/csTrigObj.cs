using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csTrigObj : MonoBehaviour
{
    PhotonView pv;
    Animator anim;
    private float lastTime;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        if(gameObject.tag=="Chair"|| gameObject.tag =="OfficeChair")
        {
            pv.ObservedComponents.RemoveAt(0);
            pv.ObservedComponents.Add(transform);
        }
    }

    public void DoorOnOff()
    {
        pv.RPC("DoorAnim", PhotonTargets.All);
    }

    [ContextMenu("TestChair")]
    public void ChairOnOff()
    {
        if (transform.localPosition.x > 0.5) StartCoroutine("ChairOut");
        else StartCoroutine("ChairIn");
    }
    public void OfficeChairOnOff()
    {
        if (transform.localPosition.z < 0.3) StartCoroutine("OfficeChairOut");
        else StartCoroutine("OfficeChairIn");
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


    IEnumerator ChairOut()
    {
        while(transform.localPosition.x > 0.42f)
        {
            transform.localPosition -= new Vector3(0.02f, 0, 0);
            yield return null;
        }
    }
    IEnumerator ChairIn()
    {
        while (transform.localPosition.x < 0.6f)
        {
            transform.localPosition += new Vector3(0.02f, 0, 0);
            yield return null;
        }
    }
    IEnumerator OfficeChairOut()
    {
        while (transform.localPosition.z < 0.5f)
        {
            transform.localPosition += new Vector3(0, 0, 0.02f);
            yield return null;
        }
    }
    IEnumerator OfficeChairIn()
    {
        while (transform.localPosition.z > 0.0f)
        {
            transform.localPosition -= new Vector3(0, 0, 0.02f);
            yield return null;
        }
    }
}
