using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Expedition", menuName = "Survival Game/EquipmentSO")]
public class EquipmentSO : ScriptableObject
{
    [Header("장비 정보")]
    public string equipmentName = "맨손";

    [Header("탐험 보너스")]
    [Range(0, 30)]
    public int sucessBonus = 0;           //성공률 보너스
    [Range(0, 3)]                         
    public int rewardBonus = 0;           //보상 증가

    [Header("내구도")]
    [Range(1, 10)]
    public int maxDurability = 1;        //최대 내구도(몇 번 사용 할지)

    [Header("설명")]
    public string description = "기본 상태"; 
}
