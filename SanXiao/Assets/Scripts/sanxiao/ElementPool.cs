using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Pool;

public class ElementPool : MonoBehaviour
{
    //三消元素
    private ObjectPool<SingleItem> elementPool;
    public SingleItem elementPrefab;//三消元素预制体
    
   
    private void Awake()
    {
        elementPool = new ObjectPool<SingleItem>(OnElementCreate,OnElementGet,OnElementRelease,OnElementDes);       
    }

    /// <summary>
    /// 提供给外部的方法，从对象池中获得实例
    /// </summary>
    /// <returns></returns>
    public SingleItem GetElement()
    {
        return elementPool.Get();
    }
    /// <summary>
    /// 提供给外部的方法，将实例放回对象池
    /// </summary>
    /// <param name="obj"></param>
    public void ReleaseElement(SingleItem obj)
    {
        elementPool.Release(obj);
        
    }
    public void ClearPool()
    {
        elementPool.Clear();
    }

    

    #region 三消元素对象池初始化4个函数  
    private SingleItem OnElementCreate()
    {
        SingleItem go = Instantiate(elementPrefab);
        return go;
    }

    private void OnElementGet(SingleItem obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnElementRelease(SingleItem obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnElementDes(SingleItem obj)
    {
        Destroy(obj.gameObject);
    }
    #endregion

    

}
