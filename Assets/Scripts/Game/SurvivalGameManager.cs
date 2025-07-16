using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("�׷� ������ ���ø�")]
    public GroupMemberSO[] groupMembers;

    [Header("���� UI")]
    public Text dayText;                       //���� ǥ��
    public Text[] memberStatusTexts;           //��� ���� ǥ��
    public Button nextDayButton;              //���� ��¥�� ����Ǵ� ��ư

    int currentDay;                               //���� ��¥

    //��Ÿ�� ������
    private int[] memberHealth;
    private int[] memberHungry;
    private int[] memberBodyTemp;
    void Start()
    {
        InitiallizeGroup();
        UpdateUI();

        currentDay = 1;

        nextDayButton.onClick.AddListener(NextDay);
    }


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
    

    void UpdateUI()
    {
        dayText.text =$"Day {currentDay}";

        for(int i = 0;i < groupMembers.Length;i++)
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
        int baseHungryLoss = -15;
        int baseTempLoss = 1;

        for (int i = 0; i < groupMembers.Length;i++)
        {
            if (groupMembers[i] == null) continue;
            GroupMemberSO member = groupMembers[i];

            //���̿� ���� ����� ���� 
            float hungryMultiplier = member.ageGroup == GroupMemberSO.AgeGroup.Child ? 0.8f : 1.0f;

            //���� ����
            memberHungry[i] -=Mathf.RoundToInt(baseHungryLoss * hungryMultiplier);
            memberBodyTemp[i] -= Mathf.RoundToInt(baseTempLoss * member.coldResistance);

            //�ǰ�üũ
            if (memberHungry[i] <= 0) memberHungry[i] -= 15;
            if (memberBodyTemp[i] <= 32) memberHealth[i] -= 10;
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
    void Update()
    {
        
    }
}
