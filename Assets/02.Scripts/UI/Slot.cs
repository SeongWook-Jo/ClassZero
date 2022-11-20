using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    public Item item; // 획득한 아이템
    public Image itemImage; // 아이템의 이미지.
    public Text prf_Intext; // 단서 text
    

    public GameObject ClueList_prf;
    public GameObject scrollContents;
    GameObject textinfo;


    [SerializeField]
    private GameObject go_CountImage;// 획득한 아이템이 있을 경우 파란색 동그라미 띄우기...이거도 필요 없을듯


    // 이미지의 투명도 조절.(아이템 이미지의 투명도 조절.. 인벤 아이템창 칸 투명도가 아니라)
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 획득.. 다른곳에서 어떤 함수를 호출한 다음에 변수에 값들 채워줄거임
    public void AddItem(Item _item)
    {
        item = _item;  // 어떤 아이템을 먹었는지 아이템 정보를 받아와서
        //itemImage.sprite = item.itemImage; // 그 아이템 정보에 들어있는 이미지를 Slot의 sprite에 넣어준다.
        //itemCount = _count;
        //clue는 text를 받아와야...
        //clueText = item.clueText;  


        //우리는 AddItem에서 이 부분은 필요 없음.. 아이템 개수랑 개수 txt 띄워주는 부분
        //if (item.itemType != Item.ItemType.Clue)
        //{
        //    go_CountImage.SetActive(true);
        //    text_Count.text = itemCount.ToString();
        //}
        //else
        //{
        //    text_Count.text = "0";
        //    go_CountImage.SetActive(false);
        //}

        if(item.itemType != Item.ItemType.Clue) // 아이템 타입이 clue가 아니라면. 즉 Item 타입일때
        {
            // Inven_Item 하위의 GridSetting에 InGame_Slot 프리맵에 Item_Image => sprite 받는 걸 함
            itemImage.sprite = item.itemImage;
        }
        else
        {
            // 아이템 타입이 clue, text를 Inven_Clue의 Scroll View 하위의 Content 및에 ClueList_prf, Clueprf 
            // 의 text에 받아와 주는걸 함...
            //clueText = item.clueText;

            // 아이템 타입이 clue, text를 Inven_Clue의 Scroll View 하위의 Content 및에 ClueList_prf, Clueprf 
            // 의 text에 받아와 주는걸 함...
            //clueText.text = _item.clueInText[1];
            Debug.Log("Clue");
            // ClueList_prf를 ScrollView 영역에 생성하는 파트----------------
            //GameObject textinfo = (GameObject)Instantiate(ClueList_prf);
            textinfo = Instantiate<GameObject>(ClueList_prf, transform);
            Debug.Log(textinfo);
            textinfo.GetComponentInChildren<Text>().text = Item.clueInText[1];
            
            //textinfo.transform.SetParent(scrollContents.transform, false);

            int rowCount = 0;
            scrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
            scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 10);
        }

        SetColor(1);
    }

    // 슬롯 초기화.
    public void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        SetColor(0);
        //clueText.text = "";
        go_CountImage.SetActive(false);
    }
}
