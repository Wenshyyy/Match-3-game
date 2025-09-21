using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class MonsterPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class MonsterPoolConfig
    {
        public string monsterType;  // 怪物类型标识
        public GameObject prefab;    // 怪物预制体
        public int defaultCapacity = 10; // 初始池大小
        public int maxSize = 50;     // 池最大容量
    }

    // 在Inspector中配置5种怪物
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

        //遍历config为每个怪物创造自己的对象池
        foreach (var config in monsterConfigs)
        {
            // 检查重复类型
            if (_monsterPools.ContainsKey(config.monsterType))
            {
                Debug.LogError($"Duplicate monster type: {config.monsterType}");
                continue;
            }

            _prefabLookup[config.monsterType] = config.prefab;

            // 为每种怪物类型创建对象池
            _monsterPools[config.monsterType] = new ObjectPool<GameObject>(
                createFunc: () => CreateMonster(config.monsterType),
                actionOnGet: (monster) => OnGetMonster(monster),
                actionOnRelease: (monster) => OnReleaseMonster(monster),
                actionOnDestroy: (monster) => Destroy(monster),
                collectionCheck: true,  // 防止重复释放
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );

            // 预加载怪物
            PreloadMonsters(config.monsterType, config.defaultCapacity);
        }
    }

    private GameObject CreateMonster(string monsterType)
    {
        if (_prefabLookup.TryGetValue(monsterType, out var prefab))
        {
            GameObject monster = Instantiate(prefab);
            // 添加池标识组件
            var poolable = monster.AddComponent<PooledMonster>();
            poolable.monsterType = monsterType;
            poolable.poolManager = this;
            monster.SetActive(false);
            return monster;
        }
        return null;
    }

    // 预加载一定数量的怪物
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
        //怪物初始化逻辑      
        monster.GetComponent<Monster>().SetHp(); 
    }

    private void OnReleaseMonster(GameObject monster)
    {
        monster.SetActive(false);        
    }

    // 外部获取指定类型怪物
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

    // 回收怪物
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

// 怪物池标识组件
public class PooledMonster : MonoBehaviour
{
    public string monsterType;
    public MonsterPoolManager poolManager;

    // 提供便捷的回收方法
    public void ReturnToPool()
    {
        poolManager?.ReleaseMonster(gameObject);
    }
}