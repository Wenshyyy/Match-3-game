using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterPool : MonoBehaviour
{
    public static MonsterPool Instance { get; private set; }

    private IObjectPool<GameObject> monsterPool;
    public List<GameObject> monsterPrefabs;
    private int mixSize = 5;
    private int maxSize = 50;
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

        monsterPool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDes, true, mixSize, maxSize);
        
    }

    public GameObject GetMonster()
    {
        GameObject monster = monsterPool.Get();
        return monster;
    }

    public void ReleaseMonster(GameObject m)
    {
        monsterPool.Release(m);
    }
    public void ClearPool()
    {
        monsterPool.Clear();
    }

    private GameObject OnCreate()
    {
        
        GameObject monster = Instantiate(monsterPrefabs[Random.Range(0,monsterPrefabs.Count)]);
        monster.SetActive(false);
        return monster;
    }

    private void OnGet(GameObject m)
    {
        m.SetActive(true);
        m.GetComponent<Monster>().SetHp();
    }

    private void OnRelease(GameObject m)
    {
        m.SetActive(false);
    }

    private void OnDes(GameObject b)
    {
        Destroy(b);
    }


}
