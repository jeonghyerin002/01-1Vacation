using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplePlayerPrefs : MonoBehaviour
{
    public InputField nameInput;         //�۾��� �Է� ���� �� �ִ� UI
    public Text scoreText;               //���ھ� UI �ؽ�Ʈ
    public Button saveButton;            //�����ư
    public Button loadButton;            //�ε��ư

    int currentScore = 0;                      //���� ���ھ�
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

        Debug.Log("����Ϸ�");
    }

    void LoadData()
    {
        string saveName = PlayerPrefs.GetString("PlayerName", "PlayerName");
        int savedScore = PlayerPrefs.GetInt("HighScore", 0);

        nameInput.text = saveName;
        currentScore = savedScore;
        scoreText.text = "score " + currentScore;

        Debug.Log("�ҷ����� �Ϸ�");
    }
}
