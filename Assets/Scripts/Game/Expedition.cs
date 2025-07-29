using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expedition : MonoBehaviour
{
    [Header("탐방 데이터")]
    public ExpeditionSO[] expeditions;           //탐방 종류

    [Header("탐방 UI")]
    public Button expeditionButton;      //탐방 시작버튼
    public Button[] memberButton;        
    public GameObject memberSelectPanel;
    public Text expeditionInfoText;
    public Text resultText;

    private SurvivalGameManager gameManager;
    private ExpeditionSO currentExpedition;

    [Header("장비 시스템")]
    public EquipmentSO[] availableEquipmnets;
    public Dropdown equipmentDropdown;             //드롭다운 UI

    public int selectedEquipmentIndex = 0;         //선택된 장비 인텍스
    public int[] equipmentDurability;          //각 장비의 내구도


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

        //내구도 배열 최기화
        InititalizeEquipmentDurability();

        //드롭다운 설정 추가
        SetupEquipmentDropdown();
        equipmentDropdown.onValueChanged.AddListener(OnEquipmentChanged);        //드롭 다운 선택이 변경 될 때 함수를 호출한다
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

            //부러진 장비는 보너스 없음
            int equipBonus = (selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0) ? 0 : selectedEquip.sucessBonus;
            int totalSucessRate = currentExpedition.baseSucessRate + equipBonus;

            string durabilityInfo = "";

            if(selectedEquipmentIndex > 0)
            {
                if (equipmentDurability[selectedEquipmentIndex] <= 0) durabilityInfo = "(부러진 상태 - 효과 없음)";
                else durabilityInfo = $"(내구도 : {equipmentDurability[selectedEquipmentIndex]}/{selectedEquip.maxDurability})";
            }

            

            expeditionInfoText.text = $"탐방 :{currentExpedition.expeditionName}\n" +
                                      $"{currentExpedition.description}\n" +
                                      $"기본 성공률 : {currentExpedition.baseSucessRate}%\n" +
                                      $"장비 보너스 : +{equipBonus}% {durabilityInfo}\n" +
                                      $"최종 성공률 : {totalSucessRate}%";
        }
    }

    void UpdateMemberButtons()                       //멤버 버튼 업데이트 정보
    {
        for(int i = 0; i < memberButton.Length && i < gameManager.groupMembers.Length; i++)
        {
            GroupMemberSO member = gameManager.groupMembers[i];
            bool canGo = gameManager.memberHealth[i] > 20;          //체력이 20 이상일 때만 탐방 가능

            Text buttonText = memberButton[i].GetComponentInChildren<Text>();
            buttonText.text = $"{member.memberName} \n 체력 : {gameManager.memberHealth[i]}";
            memberButton[i].interactable = canGo;
        }
    }

    public void OpenMemberSelect()
    {
        //새로운 탐방 랜덤 선택
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

        //부러진 장비는 효과 없음
        bool equipmentBroken = selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0;
        int equipBonus = equipmentBroken ? 0 : selectedEquip.sucessBonus;
        int rewardBonus = equipmentBroken ? 0 : selectedEquip.rewardBonus;

        //성공률 계산 [ExpenditionSO의 기본 성공률 + 장비 보너스]
        int finalSucessRate = currentExpedition.baseSucessRate + equipBonus;
        finalSucessRate = Mathf.Clamp(finalSucessRate, 5, 95);

        bool sucess = Random.Range(1, 101) <= finalSucessRate;

        //장비 내구도 감소 (맨손 제외, 부러지지 않은 장비만)
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

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} 성공! (성공률 : {finalSucessRate}% \n" +
                         $"음식 + {currentExpedition.sucessFoodReward + rewardBonus}, 연료 + {currentExpedition.sucessFuelReWard + rewardBonus}," +
                         $"의약품 +{currentExpedition.sucessMedicineReWard + rewardBonus}";

            resultText.color = Color.green;

        }
        else
        {
            gameManager.memberHealth[memberIndex] += currentExpedition.failHealthPenalty;
            gameManager.memberHungry[memberIndex] += currentExpedition.failHungerPenalty;
            gameManager.memberBodyTemp[memberIndex] += currentExpedition.failTempPenalty;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} 실패! (성공률 : {finalSucessRate}% \n" +
                         $"체력 + {currentExpedition.failHealthPenalty}, 배고픔 + {currentExpedition.failHungerPenalty}," +
                         $"온도 +{currentExpedition.failTempPenalty}";

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

    void InititalizeEquipmentDurability()             //장비의 내구도 셋틴하는 함수
    {
        equipmentDurability = new int[availableEquipmnets.Length];           //장비의 숫자 만큼 배열 선언(동작 선언)

        for(int i = 0; i < availableEquipmnets.Length; i++)
        {
            equipmentDurability[i] = availableEquipmnets[i].maxDurability;    //사용 가능한 내구도를 배열에 넣어준다
        }
    }

    void SetupEquipmentDropdown()
    {
        equipmentDropdown.options.Clear();                //옵션을 초기화 시켜준다

        //장비 옵션들을 드롭다운에 추가(내구도 포함)
        for(int i = 0; i < availableEquipmnets.Length; ++i)
        {
            string equipName = availableEquipmnets[i].equipmentName;

            //내구도가 0이면 (부러진)표시, 맨손 (인텍스 0_)은 제외 (무제한 사용)
            if (i == 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData(equipName));
            }
            else if (equipmentDurability[i] <= 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} (부러진)"));
            }
            else
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName}({equipmentDurability[i]}/ {availableEquipmnets[i].maxDurability})"));
            }
        }
    }
}
