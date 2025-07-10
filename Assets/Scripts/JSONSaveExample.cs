using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class JSONSaveExample : MonoBehaviour
{
    [Header("UI")]
    public InputField nameInput;                 
    public Text levelText;
    public Text goldText;
    public Text playTimeText;
    public Button saveButton;
    public Button loadButton;

    PlayerData playerData;
    string saveFilePath;
    // Start is called before the first frame update
    void Start()
    {
        //저장 파일 경로 설정
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");

        //데이터 초기
        playerData = new PlayerData();
        playerData.playerName = "새로운 플레이어";
        playerData.level = 1;
        playerData.gold = 100;
        playerData.playtime = 0f;
        playerData.position = Vector3.zero;

        saveButton.onClick.AddListener(SaveToJSON);
        loadButton.onClick.AddListener(LoadFromJSON);

        LoadFromJSON();
        UpdateUI();
        //Debug.Log
    }

    // Update is called once per frame
    void Update()
    {
        playerData.playtime += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.L))
        {
            playerData.level++;
            playerData.gold += 50;
        }

        if(Input.GetKeyDown(KeyCode.G))
        {
            playerData.gold += 10;
        }
    }

    void UpdateUI()
    {
        nameInput.text = playerData.playerName;
        levelText.text = "Lv. " + playerData.level;
        goldText.text = "Gold: " + playerData.gold;
        playTimeText.text = "PlayTime : " + playerData.playtime;
    }

    void SaveToJSON()
    {
        playerData.playerName = nameInput.text;

        string jsonData = JsonUtility.ToJson(playerData, true);

        File.WriteAllText(saveFilePath, jsonData);

        Debug.Log("저장 완료");
    }

    void LoadFromJSON()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);

            playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            Debug.Log("불러오기 완료");
        }
        else
        {
            Debug.Log("저장 파일이 없습니다.");
        }

        UpdateUI();
    }
}
