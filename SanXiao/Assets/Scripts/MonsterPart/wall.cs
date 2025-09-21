using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Mouster"))
        {
            //回收怪物
            collision.GetComponent<PooledMonster>()?.ReturnToPool();
            //玩家减血
            PlayerMgr.Instance.playerHp--;
            UIMgr.Instance.LostHeart();
        }
    }
}
