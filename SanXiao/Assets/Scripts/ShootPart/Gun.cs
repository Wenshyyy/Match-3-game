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
    private Vector3 gunPos;//ǹλ��
    public int minHeight;
    public int maxHeight;

    //private BulletPool bulletPool;
    [Header("�������")]
    public Transform shootPos;//����λ��  
    public float shootTime =0.5f;//�ӵ�������ʱ��
    public float shootForce = 20f;

    private Queue<GameObject> bulletsQueue = new Queue<GameObject>();

    public AudioClip shootClip;
    

    private void Start()
    {
        //����Э��
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

       
       //�����û�д���ӵ��ӵ�����
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
    /// �����ӵ��ķ�����ÿִ��һ�δӵ������ó�һ���ӵ�
    /// </summary>
    public void Shoot()
    {
        //���ӵ��Ӷ������ó�
        GameObject tepB = bulletsQueue.Dequeue();
        tepB.gameObject.SetActive(true);                   
        Rigidbody2D rb = tepB.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.AddForce(shootPos.right*-1*shootForce,ForceMode2D.Impulse);

        AudioSource.PlayClipAtPoint(shootClip,transform.position);
        
    }

    public void AddBulletToQueue(E_BulletType type)
    {
        //�ӻ�������ó��ӵ�
        GameObject bullet = BulletPool.Instance.GetBullet();
        //�����ӵ��ĸ�����
        //bullet.transform.SetParent(transform);
        bullet.transform.position = this.transform.position;
        //�����ӵ���Ϣ
        bullet.GetComponent<Bullet>().BirthBullet(type);
        //���ӵ����queue
        bulletsQueue.Enqueue(bullet);
        Debug.Log("��queue�������һ���ӵ�");
        print("�����ӵ���"+bulletsQueue.Count);
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
