using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expedition : MonoBehaviour
{
    [Header("Ž�� ������")]
    public ExpeditionSO[] expeditions;           //Ž�� ����

    [Header("Ž�� UI")]
    public Button expeditionButton;      //Ž�� ���۹�ư
    public Button[] memberButton;        
    public GameObject memberSelectPanel;
    public Text expeditionInfoText;
    public Text resultText;

    private SurvivalGameManager gameManager;
    private ExpeditionSO currentExpedition;

    [Header("��� �ý���")]
    public EquipmentSO[] availableEquipmnets;
    public Dropdown equipmentDropdown;             //��Ӵٿ� UI

    public int selectedEquipmentIndex = 0;         //���õ� ��� ���ؽ�
    public int[] equipmentDurability;          //�� ����� ������


    public void Start()
    {
        gameManager = GetComponent<SurvivalGameManager>();

        memberSelectPanel.SetActive(false);
        resultText.text = "";
        expeditionInfoText.text = "";

        expeditionButton.onClick.AddListener(OpenMemberSelect);  

        for(int i = 0; i < memberButton.Length; i++)
        {
            int memberIndex = i;
            memberButton[i].onClick.AddListener(() => StartExpedition(memberIndex));
        }

        //������ �迭 �ֱ�ȭ
        InititalizeEquipmentDurability();

        //��Ӵٿ� ���� �߰�
        SetupEquipmentDropdown();
        equipmentDropdown.onValueChanged.AddListener(OnEquipmentChanged);        //��� �ٿ� ������ ���� �� �� �Լ��� ȣ���Ѵ�
    }

    void OnEquipmentChanged(int equipmentIndex)
    {
        selectedEquipmentIndex = equipmentIndex;
        UpdateExpeditionInfo();
    }
    void UpdateExpeditionInfo()
    {
        if(currentExpedition != null)
        {

            EquipmentSO selectedEquip = availableEquipmnets[selectedEquipmentIndex];

            //�η��� ���� ���ʽ� ����
            int equipBonus = (selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0) ? 0 : selectedEquip.sucessBonus;
            int totalSucessRate = currentExpedition.baseSucessRate + equipBonus;

            string durabilityInfo = "";

            if(selectedEquipmentIndex > 0)
            {
                if (equipmentDurability[selectedEquipmentIndex] <= 0) durabilityInfo = "(�η��� ���� - ȿ�� ����)";
                else durabilityInfo = $"(������ : {equipmentDurability[selectedEquipmentIndex]}/{selectedEquip.maxDurability})";
            }

            

            expeditionInfoText.text = $"Ž�� :{currentExpedition.expeditionName}\n" +
                                      $"{currentExpedition.description}\n" +
                                      $"�⺻ ������ : {currentExpedition.baseSucessRate}%\n" +
                                      $"��� ���ʽ� : +{equipBonus}% {durabilityInfo}\n" +
                                      $"���� ������ : {totalSucessRate}%";
        }
    }

    void UpdateMemberButtons()                       //��� ��ư ������Ʈ ����
    {
        for(int i = 0; i < memberButton.Length && i < gameManager.groupMembers.Length; i++)
        {
            GroupMemberSO member = gameManager.groupMembers[i];
            bool canGo = gameManager.memberHealth[i] > 20;          //ü���� 20 �̻��� ���� Ž�� ����

            Text buttonText = memberButton[i].GetComponentInChildren<Text>();
            buttonText.text = $"{member.memberName} \n ü�� : {gameManager.memberHealth[i]}";
            memberButton[i].interactable = canGo;
        }
    }

    public void OpenMemberSelect()
    {
        //���ο� Ž�� ���� ����
        if(expeditions.Length > 0)
        {
            currentExpedition = expeditions[Random.Range(0, expeditions.Length)];
            UpdateExpeditionInfo();
        }

        memberSelectPanel.SetActive(true);
        UpdateMemberButtons();
    }

    public void StartExpedition(int memberIndex)
    {
        if (currentExpedition == null) return;

        memberSelectPanel.SetActive(false);

        GroupMemberSO member = gameManager.groupMembers[memberIndex];
        EquipmentSO selectedEquip = availableEquipmnets[selectedEquipmentIndex];

        //�η��� ���� ȿ�� ����
        bool equipmentBroken = selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0;
        int equipBonus = equipmentBroken ? 0 : selectedEquip.sucessBonus;
        int rewardBonus = equipmentBroken ? 0 : selectedEquip.rewardBonus;

        //������ ��� [ExpenditionSO�� �⺻ ������ + ��� ���ʽ�]
        int finalSucessRate = currentExpedition.baseSucessRate + equipBonus;
        finalSucessRate = Mathf.Clamp(finalSucessRate, 5, 95);

        bool sucess = Random.Range(1, 101) <= finalSucessRate;

        //��� ������ ���� (�Ǽ� ����, �η����� ���� ���)
        if(selectedEquipmentIndex > 0 && !equipmentBroken)
        {
            equipmentDurability[selectedEquipmentIndex] -= 1;
            SetupEquipmentDropdown();
        }

        if(sucess)
        {
            gameManager.food += currentExpedition.sucessFoodReward + rewardBonus;
            gameManager.fuel += currentExpedition.sucessFuelReWard + rewardBonus;
            gameManager.medicine += currentExpedition.sucessMedicineReWard + rewardBonus;

            gameManager.memberHungry[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ����! (������ : {finalSucessRate}% \n" +
                         $"���� + {currentExpedition.sucessFoodReward + rewardBonus}, ���� + {currentExpedition.sucessFuelReWard + rewardBonus}," +
                         $"�Ǿ�ǰ +{currentExpedition.sucessMedicineReWard + rewardBonus}";

            resultText.color = Color.green;

        }
        else
        {
            gameManager.memberHealth[memberIndex] += currentExpedition.failHealthPenalty;
            gameManager.memberHungry[memberIndex] += currentExpedition.failHungerPenalty;
            gameManager.memberBodyTemp[memberIndex] += currentExpedition.failTempPenalty;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ����! (������ : {finalSucessRate}% \n" +
                         $"ü�� + {currentExpedition.failHealthPenalty}, ����� + {currentExpedition.failHungerPenalty}," +
                         $"�µ� +{currentExpedition.failTempPenalty}";

            resultText.color = Color.red;
        }

        GroupMemberSO memberSO = gameManager.groupMembers[memberIndex];
        gameManager.memberHungry[memberIndex] = Mathf.Max(0, gameManager.memberHungry[memberIndex]);
        gameManager.memberBodyTemp[memberIndex] = Mathf.Max(0, gameManager.memberBodyTemp[memberIndex]);
        gameManager.memberHealth[memberIndex] = Mathf.Max(0, gameManager.memberHealth[memberIndex]); 

        gameManager.UpdateUI();

        Invoke("ClearResultText", 3f);
    }

    void ClearResultText()
    {
        resultText.text = "";
    }

    void InititalizeEquipmentDurability()             //����� ������ ��ƾ�ϴ� �Լ�
    {
        equipmentDurability = new int[availableEquipmnets.Length];           //����� ���� ��ŭ �迭 ����(���� ����)

        for(int i = 0; i < availableEquipmnets.Length; i++)
        {
            equipmentDurability[i] = availableEquipmnets[i].maxDurability;    //��� ������ �������� �迭�� �־��ش�
        }
    }

    void SetupEquipmentDropdown()
    {
        equipmentDropdown.options.Clear();                //�ɼ��� �ʱ�ȭ �����ش�

        //��� �ɼǵ��� ��Ӵٿ �߰�(������ ����)
        for(int i = 0; i < availableEquipmnets.Length; ++i)
        {
            string equipName = availableEquipmnets[i].equipmentName;

            //�������� 0�̸� (�η���)ǥ��, �Ǽ� (���ؽ� 0_)�� ���� (������ ���)
            if (i == 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData(equipName));
            }
            else if (equipmentDurability[i] <= 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} (�η���)"));
            }
            else
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName}({equipmentDurability[i]}/ {availableEquipmnets[i].maxDurability})"));
            }
        }
    }
}
