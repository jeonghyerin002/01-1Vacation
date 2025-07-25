using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Expedition", menuName = "Survival Game/Expedition")]
public class ExpeditionSO : ScriptableObject
{
    [Header("탐방 기본 정보")]
    public string expeditionName = "숲탐방";
    [TextArea(2, 3)]
    public string description = "근처 숲을 탐방하여 자원을 찾습니다.";

    [Header("난이도")]
    [Range(1, 5)]
    public int difficulty = 2;

    [Header("성공 시 보상")]
    public int sucessFoodReward = 3;
    public int sucessFuelReWard = 2;
    public int sucessMedicineReWard = 1;

    [Header("실패 시 패널티")]
    public int failHealthPenalty = -20;
    public int failHungerPenalty = -10;
    public int failTempPenalty = -2;

    [Header("기본 성공률")]
    [Range(10, 90)]
    public int baseSucessRate = 60;
}
