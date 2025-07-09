using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 100;         //체력 변수 선언


    void Start()
    {
        GetComponent<Renderer>().material.color = Color.green;            //적을 초록색으로 만든다.
    }

    public void TakeDamage(int damage)                     //데미지 받는 함수 설정
    {
        health -= damage;                                //데미지 연출 실행
        StartCoroutine(DamageEffect());

        if (health < 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator DamageEffect()
    {
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Renderer>().material.color = Color.green;
    }
    IEnumerator Die()
    {
        GetComponent<Renderer>().material.color= Color.red;
        Vector3 startScale = transform.localScale;
        float timer = 0;

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timer / 0.5f);
            yield return null;
        }
    }
}
