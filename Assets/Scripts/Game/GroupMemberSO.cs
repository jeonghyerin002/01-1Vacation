using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New GroupMember" , menuName = "Survival Game/Group Member")]
public class GroupMemberSO : ScriptableObject
{
    [Header("기본 정보")]
    public string memberName = "구성원";
    public Sprite protrait;             //초상화
    public Gender gender = Gender.Male;
    public AgeGroup ageGroup = AgeGroup.Adult;

    [Header("기본 스텟")]
    [Range(50, 100)]
    public int maxHealth = 100;
    [Range(50, 100)]
    public int maxHungry = 100;
    [Range(36, 38)]
    public int normalBodyTemp = 37;               //섭씨 37도

    [Header("특성")]
    [Range(0.5f, 2.0f)]
    public float coldResistance = 1.0f;
    [Range(0.5f, 2.0f)]
    public float foodEfficiency = 1.0f;
    [Range(0.8f, 1.5f)]
    public float recoveryRate = 1.0f;

    [Header("설명")]
    [TextArea(2, 3)]
    public string description = "그룹 구성원입니다";




    public enum Gender
    {
        Male,
        Female
    }
    public enum AgeGroup
    {
        Child,          
        Adult,
        Elder
    }
}
