using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplePlayerPrefs : MonoBehaviour
{
    public InputField nameInput;         //글씨를 입력 받을 수 있는 UI
    public Text scoreText;               //스코어 UI 텍스트
    public Button saveButton;            //저장버튼
    public Button loadButton;            //로드버튼

    int currentScore = 0;                      //현재 스코어
    void Start()
    {
        saveButton.onClick.AddListener(SaveData);
        loadButton.onClick.AddListener(LoadData);

        LoadData();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            currentScore += 10;
            scoreText.text = "score " + currentScore;
        }
    }
    void SaveData()
    {
        PlayerPrefs.SetString("PlayerName", nameInput.text);
        PlayerPrefs.SetInt("HighScore", currentScore);
        PlayerPrefs.Save();

        Debug.Log("저장완료");
    }

    void LoadData()
    {
        string saveName = PlayerPrefs.GetString("PlayerName", "PlayerName");
        int savedScore = PlayerPrefs.GetInt("HighScore", 0);

        nameInput.text = saveName;
        currentScore = savedScore;
        scoreText.text = "score " + currentScore;

        Debug.Log("불러오기 완료");
    }
}
