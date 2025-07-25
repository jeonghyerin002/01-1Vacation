using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("그룹 구성원 템플릿")]
    public GroupMemberSO[] groupMembers;

    [Header("아이템 템플릿")]
    public ItemSO foodItem;
    public ItemSO fuelItem;
    public ItemSO medicineItem;

    [Header("특정 구성원")]
    public GroupMemberSO[] aMember;


    [Header("참조 UI")]
    public Text dayText;                       //날씨 표시
    public Text[] memberStatusTexts;           //멤버 상태 표시
    public Button nextDayButton;              //다음 날짜롷 변경되는 버튼
    public Text inventoryText;                //인벤토리 표시

    [Header("아이템 버튼")]
    public Button feedButton;
    public Button heatButton;                      //난방하기
    public Button healButton;

    [Header("구성원당 아이템 사용")]
    //public Button onlyoneFeedButton_01;
    //public Button onlyoneFeedButton_02;
   //public Button onlyoneFeedButton_03;
    //public Button onlyoneFeedButton_04;
   // public Button onlyoneHealButton_01;
    //public Button onlyoneHealButton_02;
   // public Button onlyoneHealButton_03;
    //public Button onlyoneHealButton_04;

    //과제 정답
    [Header("특정 멤버 아이템 소모 버튼")]
    public Button[] individualFoodButton;
    public Button[] individualHealButton;

    [Header("이벤트 시스템")]
    public EventSO[] events;
    public GameObject eventPopup;
    public Text eventTitleText;
    public Text eventDescriptionText;
    public Button eventConfirmButton;

    [Header("게임 상태")]
    int currentDay;                               //현재 날짜
    public int food = 5;                          //음식 개수
    public int fuel = 3;                          //연료 개수
    public int medicine = 4;                      //약 개수

    //런타임 데이터
    public int[] memberHealth;
    public int[] memberHungry;
    public int[] memberBodyTemp;
    void Start()
    {

        currentDay = 1;

        InitiallizeGroup();
        UpdateUI();

        nextDayButton.onClick.AddListener(NextDay);
        feedButton.onClick.AddListener(UseFoodItem);
        heatButton.onClick.AddListener(UseFuelItem);
        healButton.onClick.AddListener(UseMedicionItem);

        //onlyoneFeedButton_01.onClick.AddListener(UseOneFoodItem_01);
        //onlyoneFeedButton_02.onClick.AddListener(UseOneFoodItem_02);
        //onlyoneFeedButton_03.onClick.AddListener(UseOneFoodItem_03);
        //onlyoneFeedButton_04.onClick.AddListener(UseOneFoodItem_04);

        //onlyoneHealButton_01.onClick.AddListener(UseOneMedicineItem_01);
        //onlyoneHealButton_02.onClick.AddListener(UseOneMedicineItem_02);
        //onlyoneHealButton_03.onClick.AddListener(UseOneMedicineItem_03);
        //onlyoneHealButton_04.onClick.AddListener(UseOneMedicineItem_04);

        //과제 정답
        //individualFoodButton[0].onClick.AddListener(GiveFoodToMember0);
        //individualFoodButton[1].onClick.AddListener(GiveFoodToMember1);
        //individualFoodButton[2].onClick.AddListener(GiveFoodToMember2);
        //individualFoodButton[3].onClick.AddListener(GiveFoodToMember3);

        for(int i = 0; i < individualFoodButton.Length && i < groupMembers.Length; i++)
        {
            int memberIndex = i;
            individualFoodButton[i].onClick.AddListener(() => GiveFoodToMember(memberIndex));
        }

        for(int i = 0; i < individualHealButton.Length && i < groupMembers.Length; i++)
        {
            int memberIndex = i;
            individualHealButton[i].onClick.AddListener(() => HealMember(memberIndex));
        }

        eventPopup.SetActive(false);
        eventConfirmButton.onClick.AddListener(CloseEventPopup);

    }

    //과제 정답
   // public void GiveFoodToMember0() { GiveFoodToMember(0); }
   // public void GiveFoodToMember1() { GiveFoodToMember(1); }
   // public void GiveFoodToMember2() { GiveFoodToMember(2); }
   // public void GiveFoodToMember3() { GiveFoodToMember(3); }



    void InitiallizeGroup()
    {
        int memberCount = groupMembers.Length;          //그룹 멤버의 길이 만큼 인원수 할당
        memberHealth = new int[memberCount];          //그룹 멤버 길이 만큼 배열 할당
        memberHungry = new int[memberCount];
        memberBodyTemp = new int[memberCount];

        for(int i = 0; i < memberCount; i++)
        {
            if(groupMembers[i] != null)
            {
                memberHealth[i] = groupMembers[i].maxHealth;
                memberHungry[i] = groupMembers[i].maxHungry;
                memberBodyTemp[i] = groupMembers[i].normalBodyTemp;
            }
        }
    }
    

    public void UpdateUI()
    {
        dayText.text =$"Day {currentDay}";

        inventoryText.text = $"음식   : {food} 개\n" +
                             $"연료   : {fuel} 개\n" +
                             $"의약품 : {medicine} 개\n";

        for (int i = 0;i < groupMembers.Length;i++)
        {
            if(groupMembers[i] != null && memberStatusTexts[i] != null)
            {
                GroupMemberSO member = groupMembers[i];

                string ststus = GetMemberStatus(i);

                memberStatusTexts[i].text =
                    $"{member.memberName} \n" +
                    $"체력   :{memberHealth[i]} \n" +
                    $"배고픔 :{memberHungry[i]} \n" +
                    $"체온   :{memberBodyTemp[i]} 도 ";
            }

            UpdateTextColor(memberStatusTexts[i], memberHealth[i]);
        }
    }

    void ProcessDailyChange()
    {
        int baseHungryLoss = 15;
        int baseTempLoss = 1;

        for (int i = 0; i < groupMembers.Length;i++)
        {
            if (groupMembers[i] == null) continue;
            GroupMemberSO member = groupMembers[i];

            //나이에 따른 배고픔 조정 
            float hungryMultiplier = member.ageGroup == GroupMemberSO.AgeGroup.Child ? 0.8f : 1.0f;

            //상태 감소
            memberHungry[i] -= Mathf.RoundToInt(baseHungryLoss * hungryMultiplier);
            memberBodyTemp[i] -= Mathf.RoundToInt(baseTempLoss * member.coldResistance);

            //건강체크
            if (memberHungry[i] <= 0) memberHealth[i] -= 15;
            if (memberHealth[i] <= 32) memberHealth[i] -= 10;
            if (memberBodyTemp[i] <= 30) memberHealth[i] -= 20;

            memberHungry[i] = Mathf.Max(0, memberHungry[i]);
            memberBodyTemp[i] = Mathf.Max(25, memberBodyTemp[i]);
            memberHealth[i] = Mathf.Max(0, memberHealth[i]);




        }
    }

    public void NextDay()
    {
        currentDay += 1;

        ProcessDailyChange();
        CheckRandomEvent();                  //이벤트 체크
        UpdateUI();
        CheckGameOver();
    }

    string GetMemberStatus(int memberIndex)
    {

        //사망 체크
        if(memberHealth[memberIndex] <= 0)
            return "(사망)";

        //가장 위험한 상태부터 체크
        if (memberBodyTemp[memberIndex] <= 30) return "(심각한 저체온증)";
        else if (memberHealth[memberIndex] <= 20) return "(위험)";
        else if (memberHungry[memberIndex] <= 10) return "(굶주림)";
        else if (memberBodyTemp[memberIndex] <= 32) return "(저체온증)";
        else if (memberHungry[memberIndex] <= 30) return "(배고픔)";
        else if (memberHealth[memberIndex] <= 50) return "(약함)";
        else if (memberBodyTemp[memberIndex] <= 35) return "(추위)";
        else return "(건강)";

    }

    void CheckGameOver()
    {
        int aliveCount = 0;
        for(int i = 0; i < memberHealth.Length; i++)
        {
            if (memberHealth[i] > 0) aliveCount++;
        }

        if (aliveCount == 0)
        {
            nextDayButton.interactable = false;
        }
    }    

    void UpdateTextColor(Text text, int health)
    {
        if (health < 0)
            text.color = Color.gray;
        else if (health <= 20)
            text.color = Color.red;
        else if (health <= 50)
            text.color = Color.yellow;
        else
            text.color = Color.white;
    }

    //특정멤버만 아이템 사용
    public void GiveFoodToMember(int memberIndex)          //과제 정답
    {
        if (food <= 0 || foodItem == null) return;
        if (memberHealth[memberIndex] <= 0) return;

        food--;
        ApplyItemEffect(memberIndex, foodItem);
        UpdateUI();

    }

    public void HealMember(int memberIndex)              //과제 정답 
    {
        if (medicine <= 0 || medicineItem == null) return;
        if (memberHealth[memberIndex] <= 0) return;

        medicine--;
        ApplyItemEffect(memberIndex, medicineItem);
        UpdateUI();
    }
    public void UseFoodItem()                                      //음식 아이템 사용
    {
        if (food <= 0 || foodItem == null) return;                        //오류 처리 방지

        food--;
        UseItemOnAllMembers(foodItem);
        UpdateUI();
    }
    //public void UseOneFoodItem_01()                                      //과제 테스트용 1
    //{
        //if (food <= 0 || foodItem == null) return;                        //오류 처리 방지

        //food--;
        //ApplyItemEffect(0,foodItem);
        //UpdateUI();
    //}

    //public void UseOneFoodItem_02()                                      //과제 테스트용 1
    //{
    //    if (food <= 0 || foodItem == null) return;                        //오류 처리 방지

    //    food--;
    //    ApplyItemEffect(1, foodItem);
    //    UpdateUI();
    //}

    //public void UseOneFoodItem_03()                                      //과제 테스트용 1
    //{
    //    if (food <= 0 || foodItem == null) return;                        //오류 처리 방지

    //    food--;
    //    ApplyItemEffect(2, foodItem);
    //    UpdateUI();
    //}

    //public void UseOneFoodItem_04()                                      //과제 테스트용 1
    //{
    //    if (food <= 0 || foodItem == null) return;                        //오류 처리 방지

    //    food--;
    //    ApplyItemEffect(3, foodItem);
    //    UpdateUI();
    //}

    public void UseFuelItem()                                         //연료 아이템 사용
    {
        if (fuel <= 0 || fuelItem == null) return;                 //오류 처리 방지

        fuel--;
        UseItemOnAllMembers(fuelItem);
        UpdateUI();
    }


    public void UseMedicionItem()                                   //약 아이템 사용
    {
        if (medicine <= 0 || medicineItem == null) return;                  //오류 처리 방지

        medicine--;
        UseItemOnAllMembers(medicineItem);
        UpdateUI();
    }
    //public void UseOneMedicineItem_01()                                   //과제 테스트용 1
    //{
    //    if (medicine <= 0 || medicineItem == null) return;                  //오류 처리 방지

    //    medicine--;
    //    ApplyItemEffect(0, medicineItem);
    //    UpdateUI();
    //}
    //public void UseOneMedicineItem_02()                                   //과제 테스트용 1
    //{
    //    if (medicine <= 0 || medicineItem == null) return;                  //오류 처리 방지

    //    medicine--;
    //    ApplyItemEffect(1, medicineItem);
    //    UpdateUI();
    //}
    //public void UseOneMedicineItem_03()                                   //과제 테스트용 1
    //{
    //    if (medicine <= 0 || medicineItem == null) return;                  //오류 처리 방지

    //    medicine--;
    //    ApplyItemEffect(2, medicineItem);
    //    UpdateUI();
    //}
    //public void UseOneMedicineItem_04()                                   //과제 테스트용 1
    //{
    //    if (medicine <= 0 || medicineItem == null) return;                  //오류 처리 방지

    //    medicine--;
    //    ApplyItemEffect(3, medicineItem);
    //    UpdateUI();
    //}

    void UseItemOneMember(ItemSO item)           //과제 테스트용 1
    {
        for(int i = 0; i < aMember.Length; i++)
        {
            if (aMember[i] != null && memberHealth[i] >0)
            {
                ApplyItemEffect(i, item);
            }
        }
    }

    void UseItemOnAllMembers(ItemSO item)
    {
        for(int i = 0; i < groupMembers.Length; i++)            
        {
            if (groupMembers[i] != null && memberHealth[i] > 0)
            {
                ApplyItemEffect(i, item);
            }
        }
    }
    void ApplyItemEffect(int memberIndex, ItemSO item)
    {
        GroupMemberSO member = groupMembers[memberIndex];

        //개인 특성을 적용해서 아이ㅏ템 효과 계산
        int actualHealth = Mathf.RoundToInt(item.healthEffect * member.recoveryRate);
        int actualHungry = Mathf.RoundToInt(item.hungerEffect * member.foodEfficiency);
        int actualTemp = item.tempEffect;

        memberHealth[memberIndex] += actualHealth;
        memberHungry[memberIndex] += actualHungry;
        memberBodyTemp[memberIndex] += actualTemp;

        memberHealth[memberIndex] = Mathf.Min(memberHealth[memberIndex], member.maxHealth);
        memberHungry[memberIndex] = Mathf.Min(memberHungry[memberIndex], member.maxHungry);
        memberBodyTemp[memberIndex] = Mathf.Min(memberBodyTemp[memberIndex], member.normalBodyTemp);

    }

    //이벤트에 따른 변경 수치 함수
    void ApplyEventEffects(EventSO eventSO)
    {
        food += eventSO.foodChange;
        fuel += eventSO.fuelChange;
        medicine += eventSO.medicineChange;

        //자원 최소값 보전
        food = Mathf.Max(0, food);
        fuel = Mathf.Max(0, fuel);
        medicine = Mathf.Max(0, medicine);

        //살아있는 멤버에게 상태 변화 적용
        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i]!= null && memberHealth[i] > 0)
            {
                memberHealth[i] += eventSO.healthChange;
                memberHungry[i] += eventSO.hungerChange;
                memberBodyTemp[i] += eventSO.tempchange;

                GroupMemberSO member  = groupMembers[i];
                memberHealth[i] = Mathf.Clamp(memberHealth[i], 0, member.maxHealth);
                memberHungry[i] = Mathf.Clamp(memberHungry[i], 0, member.maxHungry);
                memberBodyTemp[i] = Mathf.Clamp(memberBodyTemp[i], 0, member.normalBodyTemp);
            }
        }
    }

    void ShowEventPopup(EventSO eventSO)
    {
        //팝업 활성화
        eventPopup.SetActive(true);
        //텍스트 설정
        eventTitleText.text = eventSO.eventTitle;
        eventDescriptionText.text = eventSO.eventDescription;
        //이벤트 효과 적용
        ApplyEventEffects(eventSO);
        //게임 진행 일시정지
        nextDayButton.interactable = false;
    }

    public void CloseEventPopup()
    {
        eventPopup.SetActive(false);
        nextDayButton.interactable = true;
        UpdateUI();
    }
    void CheckRandomEvent()
    {
        int totalProbability = 0;

        for(int i = 0; i < events.Length; i++)
        {
            totalProbability += events[i].probability;
        }

        if (totalProbability == 0)
            return;                      //모든 이벤트 확률이 0이면 이벤트 없음

        int roll = Random.Range(1, totalProbability + 1 + 50);
        int cumualtive = 0;

        for (int i = 0; i < events.Length; i++)
        {
            cumualtive += events[i].probability;
            if(roll <= cumualtive)
            {
                ShowEventPopup(events[i]);
                return;
            }
        }
    }


}
