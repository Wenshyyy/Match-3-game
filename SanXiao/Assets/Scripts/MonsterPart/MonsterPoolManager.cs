using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class MonsterPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class MonsterPoolConfig
    {
        public string monsterType;  // �������ͱ�ʶ
        public GameObject prefab;    // ����Ԥ����
        public int defaultCapacity = 10; // ��ʼ�ش�С
        public int maxSize = 50;     // ���������
    }

    // ��Inspector������5�ֹ���
    public List<MonsterPoolConfig> monsterConfigs = new List<MonsterPoolConfig>();

    private Dictionary<string, ObjectPool<GameObject>> _monsterPools;
    private Dictionary<string, GameObject> _prefabLookup;

    private void Awake()
    {
        InitializeMonsterPools();
    }

    private void InitializeMonsterPools()
    {
        _monsterPools = new Dictionary<string, ObjectPool<GameObject>>();
        _prefabLookup = new Dictionary<string, GameObject>();

        //����configΪÿ�����ﴴ���Լ��Ķ����
        foreach (var config in monsterConfigs)
        {
            // ����ظ�����
            if (_monsterPools.ContainsKey(config.monsterType))
            {
                Debug.LogError($"Duplicate monster type: {config.monsterType}");
                continue;
            }

            _prefabLookup[config.monsterType] = config.prefab;

            // Ϊÿ�ֹ������ʹ��������
            _monsterPools[config.monsterType] = new ObjectPool<GameObject>(
                createFunc: () => CreateMonster(config.monsterType),
                actionOnGet: (monster) => OnGetMonster(monster),
                actionOnRelease: (monster) => OnReleaseMonster(monster),
                actionOnDestroy: (monster) => Destroy(monster),
                collectionCheck: true,  // ��ֹ�ظ��ͷ�
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );

            // Ԥ���ع���
            PreloadMonsters(config.monsterType, config.defaultCapacity);
        }
    }

    private GameObject CreateMonster(string monsterType)
    {
        if (_prefabLookup.TryGetValue(monsterType, out var prefab))
        {
            GameObject monster = Instantiate(prefab);
            // ��ӳر�ʶ���
            var poolable = monster.AddComponent<PooledMonster>();
            poolable.monsterType = monsterType;
            poolable.poolManager = this;
            monster.SetActive(false);
            return monster;
        }
        return null;
    }

    // Ԥ����һ�������Ĺ���
    private void PreloadMonsters(string monsterType, int count)
    {
        List<GameObject> preloadedMonsters = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            preloadedMonsters.Add(_monsterPools[monsterType].Get());
        }
        foreach (var monster in preloadedMonsters)
        {
            _monsterPools[monsterType].Release(monster);
        }
    }

    private void OnGetMonster(GameObject monster)
    {
        monster.SetActive(true);
        //�����ʼ���߼�      
        monster.GetComponent<Monster>().SetHp(); 
    }

    private void OnReleaseMonster(GameObject monster)
    {
        monster.SetActive(false);        
    }

    // �ⲿ��ȡָ�����͹���
    public GameObject GetMonster(string monsterType, Vector3 position)
    {
        if (_monsterPools.TryGetValue(monsterType, out var pool))
        {
            GameObject monster = pool.Get();
            monster.transform.position = position;
            monster.SetActive(true);
            monster.GetComponent<Monster>().SetState(E_MonsterState.nullState);
            return monster;
        }
        Debug.LogError($"Monster type {monsterType} not found in pool");
        return null;
    }

    // ���չ���
    public void ReleaseMonster(GameObject monster)
    {
        if (monster.TryGetComponent<PooledMonster>(out var poolable))
        {
            if (_monsterPools.TryGetValue(poolable.monsterType, out var pool))
            {
                pool.Release(monster);
                return;
            }
        }
        Debug.LogError($"Monster {monster.name} doesn't belong to any pool or pool not found");
        Destroy(monster);
    }
}

// ����ر�ʶ���
public class PooledMonster : MonoBehaviour
{
    public string monsterType;
    public MonsterPoolManager poolManager;

    // �ṩ��ݵĻ��շ���
    public void ReturnToPool()
    {
        poolManager?.ReleaseMonster(gameObject);
    }
}