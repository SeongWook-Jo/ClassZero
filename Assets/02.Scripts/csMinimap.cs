using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csMinimap : MonoBehaviour
{
    private Image map;
    private RectTransform trplayerDot;
    private Transform trBase;
    private Transform mytr;

    void Awake()
    {
        map = transform.Find("Map").GetComponent<Image>();
        trplayerDot = transform.Find("PlayerDot").GetComponent<RectTransform>();
        trBase = GameObject.Find("MapTest").GetComponent<Transform>();
        mytr = transform.root.transform;
    }
    // Update is called once per frame
    void Update()
    {
        trplayerDot.anchoredPosition = new Vector2(mytr.position.x* 3.84f+145f, mytr.position.z * 3.75f);
    }
}
