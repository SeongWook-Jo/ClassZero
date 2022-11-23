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

        if(item.itemType != Item.ItemType.Clue) // 아이템 타입이 clue가 아니라면. 즉 Item 타입일때
        {
            // Inven_Item 하위의 GridSetting에 InGame_Slot 프리맵에 Item_Image => sprite 받는 걸 함
            itemImage.sprite = item.itemImage;
        }
        else
        {

            textinfo = Instantiate<GameObject>(ClueList_prf, transform);
            Debug.Log(textinfo);
            textinfo.GetComponentInChildren<Text>().text = _item.clueText;

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
    }
}
