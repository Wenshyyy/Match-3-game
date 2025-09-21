using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterMgr : MonoBehaviour
{
    //游戏是否结束
    private bool gameover;
    //刷新怪物时间
    public int birthTime = 10;
    //怪物的出生点
    private Vector3[] birthPos =
    {     
        new Vector3(-11,-1,0),
        new Vector3(-11,0.5f,0),
        new Vector3(-11,2,0),
        new Vector3(-11,3.5f,0),
    };
    private int killCount;

    public MonsterPoolManager monsterPoolMgr;
    private string[] monsterTypes = {"bat", "blueBird", "chicken", "mushroom", "redPig" };

    private void Start()
    {
        gameover = false;
        monsterPoolMgr = GetComponent<MonsterPoolManager>();
        StartCoroutine(BirthMonster());
    }

    /// <summary>
    /// 在随机位置生成怪物
    /// </summary>
    IEnumerator BirthMonster()
    {
        while (!gameover)
        {
            string type = monsterTypes[Random.Range(0, monsterTypes.Length)];
            Vector3 pos = birthPos[Random.Range(0, birthPos.Length)];
            //从对象池获取怪物
            GameObject m = monsterPoolMgr.GetMonster(type,pos);           
            yield return new WaitForSeconds(birthTime);
        }
    }
}
