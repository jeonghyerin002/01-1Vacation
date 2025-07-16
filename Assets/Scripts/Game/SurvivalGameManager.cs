using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("그룹 구성원 템플릿")]
    public GroupMemberSO[] groupMembers;

    [Header("참조 UI")]
    public Text dayText;                       //날씨 표시
    public Text[] memberStatusTexts;           //멤버 상태 표시
    public Button nextDayButton;              //다음 날짜롷 변경되는 버튼

    int currentDay;                               //현재 날짜

    //런타임 데이터
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
                    $"체력   :{memberHealth[i]} \n" +
                    $"배고픔 :{memberHungry[i]} \n" +
                    $"체온   :{memberBodyTemp[i]} 도 ";
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

            //나이에 따른 배고픔 조정 
            float hungryMultiplier = member.ageGroup == GroupMemberSO.AgeGroup.Child ? 0.8f : 1.0f;

            //상태 감소
            memberHungry[i] -=Mathf.RoundToInt(baseHungryLoss * hungryMultiplier);
            memberBodyTemp[i] -= Mathf.RoundToInt(baseTempLoss * member.coldResistance);

            //건강체크
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
    void Update()
    {
        
    }
}
