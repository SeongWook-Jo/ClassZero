using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class csInGameUiManager : MonoBehaviour
{
    //  Esc
    public GameObject Info;  // Esc - Pop의 Info

    // Inven
    public GameObject Inven_Item; // 탈출 아이템 인벤 페이지 (Inven - Pop)
    public GameObject Inven_CluePage;  // 단서 페이지 (Inven - Pop)
    public GameObject Inven_Special;  // 특수 아이템 인벤 페이지 (Inven - Pop)
    //단서 프리팹
    public GameObject Inven_ClueList; //단서들 리스트 불러와지는 프리팹, ClueList_Prf 으로 태그달았음
    public GameObject Inven_Clue; //단서 프리팹, CluePrf 으로 태그 달았음
    public GameObject scrollContents; // 단서리스트랑 프리팹 생길 곳

    // Map
    public GameObject M1;  //1층
    public GameObject M2;  //2층
    public GameObject M3;  //3층

    public GameObject InGameUi; // 모든 InGameUi의 부모
    public GameObject[] Btns;  // InGameUi 의 버튼들

    //인벤토리 구현
    public Inventory theInventory;
    [SerializeField]
    private GameObject go_InventoryBase;  // 인벤토리 Page Onoff 시키기 위함... 부모로 하이라키에 만들어놓음
    public static bool inventoryActivated = false; // 인벤 켜지면 카메라나 다른 것 기능 안하게... 안움직이게 막는것
    [SerializeField]
    private GameObject MainMenu;
    public static bool MainMenuActivated = false;

    // Esc
    public void OnClickInfo()
    {
        Info.SetActive(true);
    }

    //public void OnCllick_SetBtn()//SetBtn - 사운드와 감도설정
    //{       
    //    SoundManager soundmanager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    //    soundmanager.SoundUiOpen();

    //    //감도부분 추가예정

    //}

    //Inven

    public bool InventoryOn()
    {
        inventoryActivated = !inventoryActivated;
        if (inventoryActivated)
        {
            OpenInventory();
            CloseMainMenu();
            return true;
        }
        else
        {
            CloseInventory();
            return false;
        }
    }
    public bool MainMenuOn()
    {
        MainMenuActivated = !MainMenuActivated;
        if (MainMenuActivated)
        {
            OpenMainMenu();
            CloseInventory();
            return true;
        }
        else
        {
            CloseMainMenu();
            return false;
        }
    }
    private void OpenMainMenu()
    {
        MainMenu.SetActive(true);
    }
    private void CloseMainMenu()
    {
        MainMenu.SetActive(false);
    }
    private void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }
    private void CloseInventory()
    {
        go_InventoryBase.SetActive(false);
    }
    public void OnClickInven_Item()
    {
        Inven_CluePage.SetActive(false);
        Inven_Special.SetActive(false);

        Inven_Item.SetActive(true);
    }
    public void OnClickInven_Clue()  // 단서 Btn 클릭하면 Scroll View (단서 리스트들이 뜰 곳) Popup 
    {
        Inven_Item.SetActive(false);
        Inven_Special.SetActive(false);

        Inven_CluePage.SetActive(true); //  scroll view 가 popup 되고

        // 그 영역에 instantiate(ClueList_prf)되고... ClueList_prf는 text 받아오고
        //foreach ()
        //{
        //    GameObject clueList_prf = (GameObject)Instantiate(Inven_ClueList);

        //    clueList_prf.transform.SetParent(scrollContents.transform, false);

        //    Item itemtext = clueList_prf.text.GetComponent<Item>();
        //    itemtext.itemtext = clueList_prf.text;
        //}


    }



    //public void OnClick_ClueList() // 단서들중에 하나 골라서 읽으려고 클릭-> 단서 내용 읽을 수 있는 Page Popup
    //{
    //    Inven_ClueList.SetActive(false);
    //    Inven_Clue.SetActive(true);

    //}

    public void OnClick_X() //단서 PopUp된것 닫기
    {
        Inven_Clue.SetActive(false);
        Inven_ClueList.SetActive(true);

    }

    public void OnClickInven_Special()
    {
        Inven_Item.SetActive(false);
        Inven_CluePage.SetActive(false);

        Inven_Special.SetActive(true);
    }


    //Map
    public void OnClick_M1()
    {
        M2.SetActive(false);
        M3.SetActive(false);

        M1.SetActive(true);
    }
    public void OnClick_M2()
    {
        M1.SetActive(false);
        M3.SetActive(false);

        M2.SetActive(true);
    }
    public void OnClick_M3()
    {
        M1.SetActive(false);
        M2.SetActive(false);

        M3.SetActive(true);
    }

    //ReturnBtn
    public void OnClick_InGame_ReturnBtn() //InGame Ui에서 나와서 다시 게임으로 돌아감, ReturnBtn
    {
        InGameUi.SetActive(false);
        MainMenuActivated = false;
        inventoryActivated = false;
    }

    //ExitBtn 
    public void OnClickReturnLobby()// InGame에서 Lobby로 나감. LeaveRoom()- 네트워크 Room에서 나가고, LogMsg로 나간 PlayerId 
    {
        // "안의 캔버스 컴포넌트 비활성화"
        //GameObject.Find("").GetComponent<Canvas>().enabled = false;

        SceneManager.LoadScene("scNetLobby");

        //PhotonNetwork.LeaveRoom();
    }

}
