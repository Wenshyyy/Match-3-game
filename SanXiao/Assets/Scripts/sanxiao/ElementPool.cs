using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Pool;

public class ElementPool : MonoBehaviour
{
    //����Ԫ��
    private ObjectPool<SingleItem> elementPool;
    public SingleItem elementPrefab;//����Ԫ��Ԥ����
    
   
    private void Awake()
    {
        elementPool = new ObjectPool<SingleItem>(OnElementCreate,OnElementGet,OnElementRelease,OnElementDes);       
    }

    /// <summary>
    /// �ṩ���ⲿ�ķ������Ӷ�����л��ʵ��
    /// </summary>
    /// <returns></returns>
    public SingleItem GetElement()
    {
        return elementPool.Get();
    }
    /// <summary>
    /// �ṩ���ⲿ�ķ�������ʵ���Żض����
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

    

    #region ����Ԫ�ض���س�ʼ��4������  
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
