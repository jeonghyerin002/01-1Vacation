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
    }
    void UpdateExpeditionInfo()
    {
        if(currentExpedition != null)
        {
            expeditionInfoText.text = $"탐방 :{currentExpedition.expeditionName}\n" +
                                      $"{currentExpedition.description}\n" +
                                      $"기본 성공률 : {currentExpedition.baseSucessRate}%";
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

        int memberBouns = 0;
        int finalSucessRate = currentExpedition.baseSucessRate + memberBouns;
        finalSucessRate = Mathf.Clamp(finalSucessRate, 5, 95);

        bool sucess = Random.Range(1, 101) <= finalSucessRate;

        if(sucess)
        {
            gameManager.food += currentExpedition.sucessFoodReward;
            gameManager.fuel += currentExpedition.sucessFuelReWard;
            gameManager.medicine += currentExpedition.sucessMedicineReWard;

            gameManager.memberHungry[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} 성공! (성공률 : {finalSucessRate}% \n" +
                         $"음식 + {currentExpedition.sucessFoodReward}, 연료 + {currentExpedition.sucessFuelReWard}," +
                         $"의약품 +{currentExpedition.sucessMedicineReWard}";

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
        gameManager.memberHungry[memberIndex] += Mathf.Max(0, gameManager.memberHungry[memberIndex]);
        gameManager.memberBodyTemp[memberIndex] += Mathf.Max(0, gameManager.memberBodyTemp[memberIndex]);
        gameManager.memberHealth[memberIndex] += Mathf.Max(0, gameManager.memberHealth[memberIndex]); 

        gameManager.UpdateUI();

        Invoke("ClearResultText", 3f);
    }

    void ClearResultText()
    {
        resultText.text = "";
    }
}
