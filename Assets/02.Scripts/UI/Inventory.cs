using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    //얘가 true일 때 i누르면 인벤창 꺼지고 false일 때 i 누르면 인벤창 켜짐
    public static bool inventoryActivated = false; // 인벤 켜지면 카메라나 다른 것 기능 안하게... 안움직이게 막는것


    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_InventoryBase;  // 인벤토리 Page Onoff 시키기 위함... 부모로 하이라키에 만들어놓음
    [SerializeField]
    private GameObject go_SlotsParent;  // slot 여러개 만들면서 부모로 Grid Setting 만들어놨음
    [SerializeField]
    private GameObject go_ClueSlotParent; // Inven_Clue => Scroll view 하위의 Content(단서 목록들 생성될 곳)

    // Item 슬롯들.
    private Slot[] slots;
    //Clue 슬롯들 (추가)
    private Slot[] clueSlots;

    PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }


    // Use this for initialization
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();  // Item slot 배열
        clueSlots = go_ClueSlotParent.GetComponentsInChildren<Slot>(); // clue slot 배열
    }
    // 슬롯에 아이템 채워넣기
    public void AcquireItem(Item _item)
    {
        if (Item.ItemType.Clue != _item.itemType)  // Clue가 아닐경우에... 즉, item일 경우에
        {


            for (int i = 0; i < 3; i++)//Item Type일 경우엔 Slot 빈자리 찾아서 넣어주고. 
            {
                if (slots[i].item == null)
                {
                    slots[i].AddItem(_item);  // AddItem()은 Slot script에 있음 
                    return;
                }
            }
        }
        else
        {
            //Clue Type일 경우엔 Clue Slot 빈자리 찾아서 넣어준다. 
            for (int i = 0; i < clueSlots.Length; i++)
            {
                clueSlots[i].AddItem(_item);
            }
        }

    }
    public void OnClickDrop0()
    {
        if (slots[0].item != null)
        {
            Transform playerTr = GameObject.Find("Player_FP").transform;
            //아이템 드랍시 네트워크적으로 만들어줘야 해서 마스터 클라이언트 ->  PhotonNetwork.InstantiateSceneObject 사용, 인자값으로 현재 캐릭터 위치, 프리팹 네임을 전달함.
            pv.RPC("ItemDrop", PhotonTargets.MasterClient, playerTr.position, slots[0].item.itemPrefab.name);
            //PhotonNetwork.InstantiateSceneObject(slots[0].item.itemPrefab.name, playerTr.position, Quaternion.identity, 0,null);
            slots[0].ClearSlot();
        }
    }
    public void OnClickDrop1()
    {
        if (slots[1].item != null)
        {
            Transform playerTr = GameObject.Find("Player_FP").transform;
            pv.RPC("ItemDrop", PhotonTargets.MasterClient, playerTr.position, slots[1].item.itemPrefab.name);
            //PhotonNetwork.InstantiateSceneObject(slots[1].item.itemPrefab.name, playerTr.position, Quaternion.identity, 0, null);
            slots[1].ClearSlot();
        }
    }
    public void OnClickDrop2()
    {
        if (slots[1].item != null)
        {
            Transform playerTr = GameObject.Find("Player_FP").transform;
            pv.RPC("ItemDrop", PhotonTargets.MasterClient, playerTr.position, slots[2].item.itemPrefab.name);
            //PhotonNetwork.InstantiateSceneObject(slots[2].item.itemPrefab.name, playerTr.position, Quaternion.identity, 0, null);
            slots[2].ClearSlot();
        }
    }

    [PunRPC]
    public void ItemDrop(Vector3 dropPo, string prfName)
    {
        PhotonNetwork.InstantiateSceneObject(prfName, dropPo, Quaternion.identity, 0, null);
    }
}

