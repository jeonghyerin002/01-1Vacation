using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField] private float attackRange = 5f;             //���� ������ 5
    [SerializeField] private int damage = 30;                    //�������� 30
     
    [Header("�ݺ��� ���� �ɼ�")]
    [SerializeField] private int loodType = 0;   
    void Update()
    {
        float h = Input.GetAxis("Horizontal");            //���� �̵� ����
        float v = Input.GetAxis("Vertical");              //���� �̵� ����
        transform.Translate(new Vector3(h, v, 0) * 5f * Time.deltaTime);   //�ش� ������Ʈ ������ ����

        if(Input.GetKeyDown(KeyCode.Space))
        {
            AreaAttack(); 
        }
    }


    void AreaAttack()
    {
        // ���� �� �� ã��
        List<Enemy> enemies = new List<Enemy>();                   //�� ����Ʈ�� �����Ѵ�
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);           //���� ���� �ȿ� �ִ� �ݶ��̴����� �����´�

        foreach(Collider col in colliders)                               //foreach ������ collider�迭�� �ִ� ��� ������Ʈ�� �����ؼ�
        {
            Enemy enemy = col.GetComponent<Enemy>();            
            if(enemy != null) enemies.Add(enemy);
        }

        switch (loodType)
        {
            case 0: //foreach
                foreach (Enemy enemy in enemies)
                {
                    enemy.TakeDamage(damage);
                }
                break;
            case 1: //For
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].TakeDamage(damage);
                }
                break;
            case 2: //While
                int j = 0;
                while (j < enemies.Count)
                {
                    enemies[j].TakeDamage(damage);
                    j++;
                }
                break;
            case 3: //DoWhile
                if(enemies.Count > 0)
                {
                    int k = 0;
                    do
                    {
                        enemies[k].TakeDamage(damage);
                        k++;
                    }
                    while (k < enemies.Count);
                }
                break;


        }    
    }
    private void OnDrawGizmos()                                  //����� ���ؼ� ���� ������ ������ ��ü ���̾�� ǥ���Ѵ�
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position , attackRange);
    }
}
