﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //얘가 true일 때 i누르면 인벤창 꺼지고 false일 때 i 누르면 인벤창 켜짐


    [SerializeField]
    private GameObject go_SlotsParent;  // slot 여러개 만들면서 부모로 Grid Setting 만들어놨음
    private GameObject go_ClueSlotParent; // Inven_Clue => Scroll view 하위의 Content(단서 목록들 생성될 곳)

    // Item 슬롯들.
    private Slot[] slots;
    //Clue 슬롯들 (추가)
    private Slot[] clueSlots = new Slot[10]; 


    // Use this for initialization
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();  // Item slot 배열
    }
    // 슬롯에 아이템 채워넣기
    public void AcquireItem(Item _item)
    {
        if (Item.ItemType.Clue != _item.itemType)  // Clue가 아닐경우에... 즉, item일 경우에
        {
            //for (int i = 0; i < slots.Length; i++)  // 슬롯의 개수만큼 반복
            //{
            //    if (slots[i].item != null)  // i번째 슬롯칸이 null이 아니고
            //    {
            //        //if (slots[i].item.itemName == _item.itemName) // i번째에 슬롯 안의 itemName과 지금 먹으려는 것의 itemName이 같다면
            //        //{
            //        //    slots[i].SetSlotCount(_count);  // 개수만큼 증가시켜주는거라... 필요 없을듯
            //        //    return;
            //        //}
            //    }
            //}
            
            for (int i = 0; i < 3; i++)//Item Type일 경우엔 Slot 빈자리 찾아서 넣어주고. 
            {
                if (slots[i].item == null)
                {
                    slots[i].AddItem(_item);  // AddItem()은 Slot script에 있음 
                    return;
                }
            }
        }

        //Clue Type일 경우엔 Clue Slot 빈자리 찾아서 넣어준다. 
        for (int i = 0; i < clueSlots.Length; i++)
        {
            if (clueSlots[i].item == null)
            {
                clueSlots[i].AddItem(_item);
                _item.clueText.text = _item.itemtext[1]; // item text string[] 받아온다.. 추후 item spawner 위치에 따라서 string 배열에서 가져오는 달라짐
                return;
            }
        }
    }
}
