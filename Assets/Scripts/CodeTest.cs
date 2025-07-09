using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTest : MonoBehaviour
{

    [Header("이동 설정")]

    [Tooltip("이 변수는 속도 값을 설정한다.")]
    [SerializeField] private float speed;

    [SerializeField] private float speed_01;

    [Range(0, 100)]
    public int health;

    [Space(50)]
    [Header("오브젝트 설정")]
    public GameObject temp;




    // Start is called before the first frame update
    void Start()
    {
        FuntionTest_01();

        int myNumber = 10;

    }

    
    void FuntionTest_01()            //따로 파라미터나 리턴 값이 필요 없을 때
    {
        
    }

    int FuntionTest_02(int num)
    {
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
