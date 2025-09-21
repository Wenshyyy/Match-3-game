using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum E_MonsterState
{
    nullState,
    dian,
    fire,
    ice,
    water
}

/// <summary>
/// 怪物基类
/// </summary>
public class Monster : MonoBehaviour
{
    public int maxHp = 50;
    public int nowHp;
    public float speed = 1;
    private float nowspeed ;

    public Image hpBar;
    public Image stateImage;
    public List<Sprite> stateSpite;
    public bool canFly = true;
    public Vector3 birthPos;
    public E_MonsterState monsterState = E_MonsterState.nullState;

    //public Animator anim;

    private Rigidbody2D rb;
    //private BoxCollider2D boxCollider;
    


    void Start()
    {
        nowspeed = speed;
        SetHp();
        rb = gameObject.AddComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        rb.gravityScale = 0f;       
        CheckCanFly();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
    }   

    public void SetHp()
    {
        nowHp = maxHp;       
        hpBar.fillAmount = 1;
    }

    public void Run()
    {
        transform.Translate(Vector3.right*-1*nowspeed*Time.deltaTime);
    }
    public void CheckCanFly()
    {
        //如果怪物不能飞，为怪物添加rigitbody2d组件
        if (canFly == false)
        {
            rb.gravityScale = 1f;
        }
    }
    public void GetHit(Bullet bullet)
    {
        //根据怪物的状态和子弹的类型计算伤害
        float harm = CaluHarmAndSetState(bullet);
        //LostHp((int)bullet.atk);
        LostHp((int)harm);
    }

    /// <summary>
    /// 去减血
    /// </summary>
    /// <param name="value"></param>
    private void LostHp(int value)
    {
        UIMgr.Instance.HarmInfo.text = value.ToString();
        //修改怪物的HP
        nowHp-=value; 
        //判断Hp是否小于0
        //如果小于0，让Hp等于0，触发怪物死亡
        if (nowHp <=0)
        {
            nowHp = 0;
            Dead();
        }      
        //更新血条显示
        float amount = nowHp /(float)maxHp;      
        hpBar.fillAmount = amount;   
    }

    private float CaluHarmAndSetState(Bullet b)
    {
        E_BulletType type = b.bullteType;
        float harm = b.atk;
        switch (monsterState)
        {
            case E_MonsterState.nullState:
                switch (type)
                {
                    case E_BulletType.Wood:
                    case E_BulletType.Stone:                       
                        break;
                    case E_BulletType.Dian:
                        SetState(E_MonsterState.dian);
                        break;
                    case E_BulletType.Fire:
                        SetState(E_MonsterState.fire);
                        break;
                    case E_BulletType.Ice:
                        SetState(E_MonsterState.ice);                                              
                        break;
                    case E_BulletType.Water:
                        SetState(E_MonsterState.water);
                        break;
                }
                break;
            case E_MonsterState.dian:
                switch (type)
                {
                    case E_BulletType.Fire:
                        //播放爆炸特效
                        print("BOOM!!");
                        SetState(E_MonsterState.fire);
                        harm = b.atk * 1.5f;
                        break;
                    case E_BulletType.Ice:
                        SetState(E_MonsterState.ice);                       
                        break;
                    case E_BulletType.Water:
                        SetState(E_MonsterState.water);
                        harm = b.atk * 1.5f;
                        break;
                    default:                           
                        break;
                }
                break;
            case E_MonsterState.ice:
                harm = b.atk * 0.5f;
                switch (type)
                {
                    case E_BulletType.Wood:
                    case E_BulletType.Stone:                       
                        SetState(E_MonsterState.nullState);
                        break;
                    case E_BulletType.Fire:
                        SetState(E_MonsterState.water);
                        nowspeed = speed;
                        break;
                    default:
                        break;
                }
                break;
            case E_MonsterState.fire:
                switch (type)
                {
                    case E_BulletType.Dian:
                        print("BOOM!!");
                        harm = b.atk * 1.5f;
                        SetState(E_MonsterState.nullState);
                        break;
                    case E_BulletType.Ice:
                        print("蒸发");
                        harm = b.atk * 1.5f;
                        SetState(E_MonsterState.nullState);
                        break;
                    default:
                        break;
                }
                break;
            case E_MonsterState.water:
                switch (type)
                {
                    case E_BulletType.Dian:
                        harm = b.atk * 1.5f;
                        SetState(E_MonsterState.dian);
                        break;
                    case E_BulletType.Ice:
                        harm = b.atk * 1.5f;
                        SetState(E_MonsterState.ice);

                        Invoke("RemoveIce",5f);
                        break;
                    case E_BulletType.Fire:
                        harm = b.atk*1.5f;
                        SetState(E_MonsterState.fire);
                        break;
                    default:
                        break;
                }
                break;
     
        }

        return harm;
    }

    private void RemoveIce()
    {
        if (monsterState == E_MonsterState.ice)
        {
            SetState(E_MonsterState.nullState);
        }
        
    }

    private void Dead()
    {
        //将怪物回收到对象池
        //MonsterPool.Instance.ReleaseMonster(this.gameObject);
        GetComponent<PooledMonster>()?.ReturnToPool();
        PlayerMgr.Instance.killCount += 1;
        PlayerMgr.Instance.score += maxHp;
        UIMgr.Instance.UpdateKillCount(PlayerMgr.Instance.killCount);
        
    }

    public void SetState(E_MonsterState state)
    {
        monsterState = state;
        //设置文字信息
        switch (state)
        {
            case E_MonsterState.nullState:
                stateImage.sprite = stateSpite[4];
                if (nowspeed == 0)
                {
                    nowspeed = speed;
                    //anim.speed = 1f;
                }
                state = E_MonsterState.nullState;
                break;
            case E_MonsterState.dian:
                state = E_MonsterState.dian;
                stateImage.sprite = stateSpite[0];
                break;
            case E_MonsterState.ice:
                state = E_MonsterState.ice;
                stateImage.sprite = stateSpite[1];

                Invoke("RemoveIce",3f);
                nowspeed = 0;
                break;
            case E_MonsterState.fire:
                state = E_MonsterState.fire;
                stateImage.sprite = stateSpite[2];
                break;
            case E_MonsterState.water:
                state = E_MonsterState.water;
                stateImage.sprite = stateSpite[3];
                break;

        }
    }

    
}
