using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;


enum E_MoveDicType
{
    Up,
    Down,
}

public class Gun : MonoBehaviour
{
    private Vector3 gunPos;//枪位置
    public int minHeight;
    public int maxHeight;

    //private BulletPool bulletPool;
    [Header("射击设置")]
    public Transform shootPos;//发射位置  
    public float shootTime =0.5f;//子弹发射间隔时间
    public float shootForce = 20f;

    private Queue<GameObject> bulletsQueue = new Queue<GameObject>();

    public AudioClip shootClip;
    

    private void Start()
    {
        //开启协程
        StartCoroutine(CheckBulletCount());
    }

    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(E_MoveDicType.Up);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(E_MoveDicType.Down);
        }

       
       //检测有没有待添加的子弹类型
        if (PlayerMgr.Instance.bulletTypeQueue.Count > 0)
        {
            E_BulletType type = PlayerMgr.Instance.bulletTypeQueue.Dequeue();
            AddBulletToQueue(type);

        }

        StartCoroutine(CheckBulletCount());
        
    }

    private void OnDisable()
    {
        BulletPool.Instance.ClearPool();
    }

    private void Move(E_MoveDicType type)
    {
        switch (type)
        {
            case E_MoveDicType.Up:
                if (transform.position.y<maxHeight)
                {
                    this.transform.position += new Vector3(0, 1.5f, 0);
                }
                    
                break;

            case E_MoveDicType.Down:
                if (transform.position.y>minHeight)
                {
                    this.transform.position -= new Vector3(0, 1.5f, 0);
                }                
                break;
        }
    }

    /// <summary>
    /// 发射子弹的方法，每执行一次从弹夹中拿出一个子弹
    /// </summary>
    public void Shoot()
    {
        //将子弹从队列里拿出
        GameObject tepB = bulletsQueue.Dequeue();
        tepB.gameObject.SetActive(true);                   
        Rigidbody2D rb = tepB.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.AddForce(shootPos.right*-1*shootForce,ForceMode2D.Impulse);

        AudioSource.PlayClipAtPoint(shootClip,transform.position);
        
    }

    public void AddBulletToQueue(E_BulletType type)
    {
        //从缓存池中拿出子弹
        GameObject bullet = BulletPool.Instance.GetBullet();
        //设置子弹的父物体
        //bullet.transform.SetParent(transform);
        bullet.transform.position = this.transform.position;
        //设置子弹信息
        bullet.GetComponent<Bullet>().BirthBullet(type);
        //将子弹添加queue
        bulletsQueue.Enqueue(bullet);
        Debug.Log("向queue中添加了一个子弹");
        print("现有子弹："+bulletsQueue.Count);
    }

    IEnumerator CheckBulletCount()
    {       
        while (bulletsQueue.Count>0)
        {           
            Shoot();
            yield return new WaitForSeconds(shootTime);
        }

    }   
}
