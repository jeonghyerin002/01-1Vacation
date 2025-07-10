using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//JSON 으로 저장할 데이터 클래스
[System.Serializable]                //직렬화 데이터 표시
public class PlayerData
{
    public string playerName;
    public int level;
    public int gold;
    public float playtime;
    public Vector3 position;
}
