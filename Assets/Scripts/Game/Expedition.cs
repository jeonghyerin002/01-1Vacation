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
            expeditionInfoText.text = $"Ž�� :{currentExpedition.expeditionName}\n" +
                                      $"{currentExpedition.description}\n" +
                                      $"�⺻ ������ : {currentExpedition.baseSucessRate}%";
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

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ����! (������ : {finalSucessRate}% \n" +
                         $"���� + {currentExpedition.sucessFoodReward}, ���� + {currentExpedition.sucessFuelReWard}," +
                         $"�Ǿ�ǰ +{currentExpedition.sucessMedicineReWard}";

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
