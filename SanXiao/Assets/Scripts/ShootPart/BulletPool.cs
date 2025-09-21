using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance {  get; private set; }

    //�ӵ�
    private IObjectPool<GameObject> bulletPool;
    public GameObject bulletsPrefab;//�ӵ�Ԥ����
    private int mixSize = 10;
    private int maxSize = 100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {          
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        bulletPool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDes,true,mixSize,maxSize);
        
    }

    

    public GameObject GetBullet()
    {
        GameObject b = bulletPool.Get();
        if (b==null)
        {
            print("��");
            return Instantiate(bulletsPrefab);
        }
        return b;
        
    }
    public void ReleaseBullet(GameObject b)
    {
        bulletPool.Release(b);
    }

    public void ClearPool()
    {
        bulletPool.Clear();
    }



    #region �ӵ�����س�ʼ��4������
    private GameObject OnCreate()
    {
       // print("ʵ�����ӵ�");
        GameObject b = Instantiate(bulletsPrefab);
        b.SetActive(false);
        return b;
    }

    private void OnGet(GameObject b)
    {
        //print("�Ӷ���ػ���ӵ��ӵ�");       
    }

    private void OnRelease(GameObject b)
    {
        b.SetActive(false);
        //b.gameObject.transform.position = Vector3.zero;

    }

    private void OnDes(GameObject b)
    {
        Destroy(b);
    }
    #endregion

}
