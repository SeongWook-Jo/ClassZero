using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")] // =>프로젝트에서 Creat할 때 item 만들기 항목이 하나 생김.. 이 item class가 바로 생성
public class Item : ScriptableObject // ScriptableObject => 게임 오브젝트에 붙일 필요 없이 사용하게 해줌
{

    public string itemName; // 아이템의 이름.
    public ItemType itemType; // 아이템의 유형.
    public Sprite itemImage; // 아이템의 이미지.
    public GameObject itemPrefab; // 아이템의 프리팹.
    //clue를 위해 추가
    public Text clueText;
    public static string[] clueInText = { "1", "2", "3", "4", "5" };  // 텍스트 내용 받을 것

    //public string weaponType; // 무기 유형. 우린 무기 없음

    public enum ItemType
    {
        //Equipment,
        //Used,
        //Ingredient,
        //ETC
        Item, // 탈출을 위해 모으는 기본 아이템
        Clue  // 단서
    }

}
