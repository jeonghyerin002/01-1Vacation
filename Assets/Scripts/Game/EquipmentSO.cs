using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Expedition", menuName = "Survival Game/EquipmentSO")]
public class EquipmentSO : ScriptableObject
{
    [Header("��� ����")]
    public string equipmentName = "�Ǽ�";

    [Header("Ž�� ���ʽ�")]
    [Range(0, 30)]
    public int sucessBonus = 0;           //������ ���ʽ�
    [Range(0, 3)]                         
    public int rewardBonus = 0;           //���� ����

    [Header("������")]
    [Range(1, 10)]
    public int maxDurability = 1;        //�ִ� ������(�� �� ��� ����)

    [Header("����")]
    public string description = "�⺻ ����"; 
}
