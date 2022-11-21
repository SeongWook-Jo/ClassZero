using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csClearPoint : MonoBehaviour
{
    Ray ray;
    public float rad;
    public Vector3 boxSize;
    Collider[] chkIems;
    public int itemCount;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
    // Update is called once per frame
    void Update()
    {
        itemCount = 0;
        ray.origin = transform.position;
        ray.direction = Vector3.zero;
        //Physics.Cub(ray,)
        chkIems = Physics.OverlapBox(transform.position, boxSize/2, Quaternion.identity, 1<< LayerMask.NameToLayer("Trigger"));
        foreach(var i in chkIems)
        {
            if (i.tag == "Item") itemCount += 1;
        }

        Debug.Log("아이템 갯수" + itemCount);
    }
}
