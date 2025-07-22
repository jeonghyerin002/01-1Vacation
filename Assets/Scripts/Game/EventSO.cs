using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Evnet", menuName = "Survival Game/Event")]
public class EventSO : ScriptableObject
{
    [Header("이벤트 정보")]
    public string eventTitle = "이벤트 발생!";
    [TextArea(3, 5)]
    public string eventDescription = "무슨 일이 일어났습니다.";

    [Header("자원 변화")]
    public int foodChange = 0;
    public int fuelChange = 0;
    public int medicineChange = 0;

    [Header("멤버 상태 변화")]
    public int healthChange = 0;
    public int hungerChange = 0;
    public int tempchange = 0;

    [Header("발생 확률")]
    [Range(1, 100)]
    public int probability = 30;
}
