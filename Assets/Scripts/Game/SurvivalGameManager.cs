using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("�׷� ������ ���ø�")]
    public GroupMemberSO[] groupMembers;

    [Header("������ ���ø�")]
    public ItemSO foodItem;
    public ItemSO fuelItem;
    public ItemSO medicineItem;

    [Header("Ư�� ������")]
    public GroupMemberSO[] aMember;


    [Header("���� UI")]
    public Text dayText;                       //���� ǥ��
    public Text[] memberStatusTexts;           //��� ���� ǥ��
    public Button nextDayButton;              //���� ��¥�� ����Ǵ� ��ư
    public Text inventoryText;                //�κ��丮 ǥ��

    [Header("������ ��ư")]
    public Button feedButton;
    public Button heatButton;                      //�����ϱ�
    public Button healButton;

    [Header("�������� ������ ���")]
    //public Button onlyoneFeedButton_01;
    //public Button onlyoneFeedButton_02;
   //public Button onlyoneFeedButton_03;
    //public Button onlyoneFeedButton_04;
   // public Button onlyoneHealButton_01;
    //public Button onlyoneHealButton_02;
   // public Button onlyoneHealButton_03;
    //public Button onlyoneHealButton_04;

    //���� ����
    [Header("Ư�� ��� ������ �Ҹ� ��ư")]
    public Button[] individualFoodButton;
    public Button[] individualHealButton;

    [Header("�̺�Ʈ �ý���")]
    public EventSO[] events;
    public GameObject eventPopup;
    public Text eventTitleText;
    public Text eventDescriptionText;
    public Button eventConfirmButton;

    [Header("���� ����")]
    int currentDay;                               //���� ��¥
    public int food = 5;                          //���� ����
    public int fuel = 3;                          //���� ����
    public int medicine = 4;                      //�� ����

    //��Ÿ�� ������
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

        //���� ����
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

    //���� ����
   // public void GiveFoodToMember0() { GiveFoodToMember(0); }
   // public void GiveFoodToMember1() { GiveFoodToMember(1); }
   // public void GiveFoodToMember2() { GiveFoodToMember(2); }
   // public void GiveFoodToMember3() { GiveFoodToMember(3); }



    void InitiallizeGroup()
    {
        int memberCount = groupMembers.Length;          //�׷� ����� ���� ��ŭ �ο��� �Ҵ�
        memberHealth = new int[memberCount];          //�׷� ��� ���� ��ŭ �迭 �Ҵ�
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

        inventoryText.text = $"����   : {food} ��\n" +
                             $"����   : {fuel} ��\n" +
                             $"�Ǿ�ǰ : {medicine} ��\n";

        for (int i = 0;i < groupMembers.Length;i++)
        {
            if(groupMembers[i] != null && memberStatusTexts[i] != null)
            {
                GroupMemberSO member = groupMembers[i];

                string ststus = GetMemberStatus(i);

                memberStatusTexts[i].text =
                    $"{member.memberName} \n" +
                    $"ü��   :{memberHealth[i]} \n" +
                    $"����� :{memberHungry[i]} \n" +
                    $"ü��   :{memberBodyTemp[i]} �� ";
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

            //���̿� ���� ����� ���� 
            float hungryMultiplier = member.ageGroup == GroupMemberSO.AgeGroup.Child ? 0.8f : 1.0f;

            //���� ����
            memberHungry[i] -= Mathf.RoundToInt(baseHungryLoss * hungryMultiplier);
            memberBodyTemp[i] -= Mathf.RoundToInt(baseTempLoss * member.coldResistance);

            //�ǰ�üũ
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
        CheckRandomEvent();                  //�̺�Ʈ üũ
        UpdateUI();
        CheckGameOver();
    }

    string GetMemberStatus(int memberIndex)
    {

        //��� üũ
        if(memberHealth[memberIndex] <= 0)
            return "(���)";

        //���� ������ ���º��� üũ
        if (memberBodyTemp[memberIndex] <= 30) return "(�ɰ��� ��ü����)";
        else if (memberHealth[memberIndex] <= 20) return "(����)";
        else if (memberHungry[memberIndex] <= 10) return "(���ָ�)";
        else if (memberBodyTemp[memberIndex] <= 32) return "(��ü����)";
        else if (memberHungry[memberIndex] <= 30) return "(�����)";
        else if (memberHealth[memberIndex] <= 50) return "(����)";
        else if (memberBodyTemp[memberIndex] <= 35) return "(����)";
        else return "(�ǰ�)";

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

    //Ư������� ������ ���
    public void GiveFoodToMember(int memberIndex)          //���� ����
    {
        if (food <= 0 || foodItem == null) return;
        if (memberHealth[memberIndex] <= 0) return;

        food--;
        ApplyItemEffect(memberIndex, foodItem);
        UpdateUI();

    }

    public void HealMember(int memberIndex)              //���� ���� 
    {
        if (medicine <= 0 || medicineItem == null) return;
        if (memberHealth[memberIndex] <= 0) return;

        medicine--;
        ApplyItemEffect(memberIndex, medicineItem);
        UpdateUI();
    }
    public void UseFoodItem()                                      //���� ������ ���
    {
        if (food <= 0 || foodItem == null) return;                        //���� ó�� ����

        food--;
        UseItemOnAllMembers(foodItem);
        UpdateUI();
    }
    //public void UseOneFoodItem_01()                                      //���� �׽�Ʈ�� 1
    //{
        //if (food <= 0 || foodItem == null) return;                        //���� ó�� ����

        //food--;
        //ApplyItemEffect(0,foodItem);
        //UpdateUI();
    //}

    //public void UseOneFoodItem_02()                                      //���� �׽�Ʈ�� 1
    //{
    //    if (food <= 0 || foodItem == null) return;                        //���� ó�� ����

    //    food--;
    //    ApplyItemEffect(1, foodItem);
    //    UpdateUI();
    //}

    //public void UseOneFoodItem_03()                                      //���� �׽�Ʈ�� 1
    //{
    //    if (food <= 0 || foodItem == null) return;                        //���� ó�� ����

    //    food--;
    //    ApplyItemEffect(2, foodItem);
    //    UpdateUI();
    //}

    //public void UseOneFoodItem_04()                                      //���� �׽�Ʈ�� 1
    //{
    //    if (food <= 0 || foodItem == null) return;                        //���� ó�� ����

    //    food--;
    //    ApplyItemEffect(3, foodItem);
    //    UpdateUI();
    //}

    public void UseFuelItem()                                         //���� ������ ���
    {
        if (fuel <= 0 || fuelItem == null) return;                 //���� ó�� ����

        fuel--;
        UseItemOnAllMembers(fuelItem);
        UpdateUI();
    }


    public void UseMedicionItem()                                   //�� ������ ���
    {
        if (medicine <= 0 || medicineItem == null) return;                  //���� ó�� ����

        medicine--;
        UseItemOnAllMembers(medicineItem);
        UpdateUI();
    }
    //public void UseOneMedicineItem_01()                                   //���� �׽�Ʈ�� 1
    //{
    //    if (medicine <= 0 || medicineItem == null) return;                  //���� ó�� ����

    //    medicine--;
    //    ApplyItemEffect(0, medicineItem);
    //    UpdateUI();
    //}
    //public void UseOneMedicineItem_02()                                   //���� �׽�Ʈ�� 1
    //{
    //    if (medicine <= 0 || medicineItem == null) return;                  //���� ó�� ����

    //    medicine--;
    //    ApplyItemEffect(1, medicineItem);
    //    UpdateUI();
    //}
    //public void UseOneMedicineItem_03()                                   //���� �׽�Ʈ�� 1
    //{
    //    if (medicine <= 0 || medicineItem == null) return;                  //���� ó�� ����

    //    medicine--;
    //    ApplyItemEffect(2, medicineItem);
    //    UpdateUI();
    //}
    //public void UseOneMedicineItem_04()                                   //���� �׽�Ʈ�� 1
    //{
    //    if (medicine <= 0 || medicineItem == null) return;                  //���� ó�� ����

    //    medicine--;
    //    ApplyItemEffect(3, medicineItem);
    //    UpdateUI();
    //}

    void UseItemOneMember(ItemSO item)           //���� �׽�Ʈ�� 1
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

        //���� Ư���� �����ؼ� ���̤��� ȿ�� ���
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

    //�̺�Ʈ�� ���� ���� ��ġ �Լ�
    void ApplyEventEffects(EventSO eventSO)
    {
        food += eventSO.foodChange;
        fuel += eventSO.fuelChange;
        medicine += eventSO.medicineChange;

        //�ڿ� �ּҰ� ����
        food = Mathf.Max(0, food);
        fuel = Mathf.Max(0, fuel);
        medicine = Mathf.Max(0, medicine);

        //����ִ� ������� ���� ��ȭ ����
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
        //�˾� Ȱ��ȭ
        eventPopup.SetActive(true);
        //�ؽ�Ʈ ����
        eventTitleText.text = eventSO.eventTitle;
        eventDescriptionText.text = eventSO.eventDescription;
        //�̺�Ʈ ȿ�� ����
        ApplyEventEffects(eventSO);
        //���� ���� �Ͻ�����
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
            return;                      //��� �̺�Ʈ Ȯ���� 0�̸� �̺�Ʈ ����

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
