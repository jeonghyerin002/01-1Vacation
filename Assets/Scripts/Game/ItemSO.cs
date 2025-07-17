using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Survival Game/Item")]

public class ItemSO : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName = "아이템";
    public Sprite itemIcon;
    public ItemType itemType;

    [Header("효과")]
    public int healthEffect = 0;                  //체력 회복량
    public int hungerEffect = 0;                  //허기 회복량
    public int tempEffect = 0;                    //체온 회복량

    [Header("설명")]
    [TextArea(2, 3)]
    public string description = "유용한 아이템 입니다.";


    public enum ItemType
    {
        Food,
        Fuel,                  //연료
        Medicine
    }
}
