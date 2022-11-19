using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClueOpen : MonoBehaviour
{
    public GameObject Inven_ClueList; //단서들 리스트 불러와지는 프리팹, ClueList_Prf 으로 태그달았음
    public GameObject Inven_Clue; //단서 프리팹, CluePrf 으로 태그 달았음
    public Text mytext;   // 부모 부모 의 첫번째 자식 안의 text에 넣어줘야...

    public void OnClick_ClueList() // 단서들중에 하나 골라서 읽으려고 클릭-> 단서 내용 읽을 수 있는 Page Popup
    {
        transform.parent.parent.GetChild(0).GetComponentInChildren<Text>().text = mytext.text;
        transform.parent.parent.GetChild(0).gameObject.SetActive(true);

    }

}
