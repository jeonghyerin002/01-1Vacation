using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTest : MonoBehaviour
{
    int Number = 0;
    public GameObject temp;
    // Start is called before the first frame update
    void Start()
    {
        FuntionTest_01();

        int myNumber = 10;

        int myNumber2 = FuntionTest_02(myNumber);

        Instantiate(temp);

        GameObject myTemp = Instantiate(temp);
        myTemp.transform.position = new Vector3(0, 0, 0);
    }

    
    void FuntionTest_01()            //따로 파라미터나 리턴 값이 필요 없을 때
    {
        Number += 1;
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
