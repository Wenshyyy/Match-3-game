using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Threading;
using UnityEngine;


public class ItemController : MonoBehaviour
{
    public int row;//����
    public int col;//����
    public float time;

    private SingleItem[,] itemList;//����Ԫ��List
    private ElementPool itemPool; //��ȡ�����

    private SingleItem itemFirst;//��һ��ѡ���Ԫ��
    //private bool canDelete;
    private List<SingleItem> deleteList;//ɾ���б�

    public AudioClip clip;




    // Start is called before the first frame update
    void Start()
    {
        deleteList = new List<SingleItem>();
        itemList = new SingleItem[row, col];
        itemPool = GetComponent<ElementPool>();
        
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                SingleItem item = NewItem(i, j);
                itemList[i, j] = item;

            }
        }
        //�ж������ɵ�Ԫ���Ƿ���Ҫ����
        StartCoroutine(CheckDeleteNext());
        

    }
   

    private void OnDisable()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Destroy(itemList[i, j].gameObject);
                itemList[i, j] = null;
                /*itemPool.ReleaseElement(itemList[i, j]);
                itemList = null;*/
            }
        }
        itemPool.ClearPool();
    }



    /// <summary>
    /// ʵ����Ԫ��
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public SingleItem NewItem(int row,int col)
    {
        //�Ӷ�����л��ʵ��
        SingleItem item = itemPool.GetElement();
        //��item�ŵ�ָ����λ��
        item.transform.SetParent(this.transform);//��item�ĸ���������Ϊ��ǰ����
        item.BirthItem(row,col);

        //��item���ɵ�ʱ��Ϊitemע��������¼�,�����Լ���Ϊ��������
        Action<SingleItem> itemAction = SelectedItem;
        item.RegisterMouseButtonAction(itemAction);

        return item;
    }

    private void SelectedItem(SingleItem item)
    {
        AudioSource.PlayClipAtPoint(clip,transform.position,0.8f);
        if (itemFirst == null)
        {
            itemFirst = item;
            itemFirst.SetSelect(true);
        }
        else
        {
            //����ǰ�ж��Ƿ�����
            if (item.CompareTypeWith(itemFirst))
            {
                //true����λ��              
                StartCoroutine(ExchangeItems(itemFirst, item)) ;//����һ�ε����Ԫ�غʹ����Ԫ�ؽ���λ��               
            }
            itemFirst.SetSelect(false);
            item.SetSelect(true);
            itemFirst = item;
        }
    }

    /// <summary>
    /// ��������Ԫ�ص�λ��
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    private void Exchange(SingleItem item1,SingleItem item2)
    {
        //itemList ��ά���������������Ԫ�ص�����
        itemList[item1.row, item1.col] = item2;
        itemList[item2.row, item2.col] = item1;
        //������Ԫ���ڲ���������
        //��ʱ����
        int temp1_col ,temp1_row,temp2_col,temp2_row;
        temp1_col = item1.col;
        temp1_row = item1.row;
        temp2_col = item2.col;
        temp2_row = item2.row;

        item1.SetItemIndex(temp2_row,temp2_col);
        item2.SetItemIndex(temp1_row,temp1_col);
        //�ƶ�λ��
        item1.MoveTo(item1.row,item1.col,time);
        item2.MoveTo(item2.row,item2.col,time);

    }

    /// <summary>
    /// �����Ͳ���������������ִ�еķ���
    /// </summary>
    IEnumerator ExchangeItems(SingleItem item1,SingleItem item2)
    {
        Exchange(item1,item2);  
        yield return new WaitForSeconds(time);
        //bool canDelete = canDelete;
        if (CheckDelete())
        {
            //������Ҫ������Ԫ��
            DeleteItem();
        }
        else
        {          
            Exchange(item1,item2);
        }

        

    }
    private void DeleteItem()
    {
         //�Ӵ��ɾ��Ԫ�ص�List�������ó���
        int count = deleteList.Count;
        for (int i=0;i<count;i++)
        {
            SingleItem item = deleteList[i];
            //������ʱ���кţ��洢��ǰԪ�ص�λ��
            int temp_row = item.row;
            int temp_col = item.col;      
            itemPool.ReleaseElement(item);
            itemList[temp_row,temp_col] = null;//�ÿ�


            //����Ԫ�����������ٽ����Ϸ�������Ԫ����������Ų��һ��
             
            for (int j=temp_row-1; j>=0; j--)
            {
                //��һ����ʱ�����洢ÿһ�����ƶ���Ԫ��
                SingleItem itemTemp = itemList[j,temp_col];
                //����ʱ������λ����Ϊ���·�
                itemList[j + 1, itemTemp.col] = itemTemp;
                //ԭ����λ������Ϊ��
                itemList[j, itemTemp.col] = null;
                //����ʱ�����ڲ����������óɵ�ǰλ��
                itemTemp.SetItemIndex(j + 1, itemTemp.col);
                //��Ϸ�������ƶ�����ǰλ��
                itemTemp.MoveTo(itemTemp.row,itemTemp.col,time);
            }

            //�����������һ��Ԫ��
            SingleItem newItem = itemPool.GetElement();
            newItem.transform.SetParent(transform);
            newItem.BirthItem(-1,item.col);
            newItem.MoveTo(0,newItem.col,time);
            itemList[0,newItem.col] = newItem;
        }
        //���ɾ���б�
        deleteList = new List<SingleItem>();

        //��Ϊ��������Ԫ�أ����Ի�Ҫ�ټ��һ���Ƿ���Ҫ����
        StartCoroutine(CheckDeleteNext());
    }

    IEnumerator CheckDeleteNext()
    {
        yield return new WaitForSeconds(time);
        if (CheckDelete())
        {
            DeleteItem();
        }         
    }

    private bool CheckDelete()
    {
        bool canDelete = false;
        for (int c=0;c<col;c++)
        {
            for (int r=0;r<row;r++)
            {
                //�жϺ�������Ԫ���Ƿ񶼴���
                if (r < row - 2 && itemList[r,c] != null && itemList[r + 1, c] != null && itemList[r + 2, c] != null)
                {
                    //�ж������Ƿ���ͬ
                    if (itemList[r,c].type == itemList[r+1,c].type&& itemList[r, c].type == itemList[r + 2, c].type)
                    {
                        AddToDeleteList(itemList[r,c]);
                        AddToDeleteList(itemList[r+1, c]);
                        AddToDeleteList(itemList[r+2, c]);
                        canDelete = true;
                        //���ݴ�����ӵ�������
                        PlayerMgr.Instance.bulletTypeQueue.Enqueue((E_BulletType)itemList[r,c].type);
                    }
                }

                
                if (c < col - 2 && itemList[r, c] != null && itemList[r, c + 1] != null && itemList[r,c+2]!=null)
                {
                    if (itemList[r,c].type == itemList[r, c + 1].type && itemList[r, c].type == itemList[r,c+2].type)
                    {
                        AddToDeleteList(itemList[r, c]);
                        AddToDeleteList(itemList[r, c+1]);
                        AddToDeleteList(itemList[r, c+2]);
                        canDelete = true;
                        //���ݴ�����ӵ�������
                        PlayerMgr.Instance.bulletTypeQueue.Enqueue((E_BulletType)itemList[r, c].type);
                    }
                }
                

            }
        }
        return canDelete;
    }

    private void AddToDeleteList(SingleItem newItem)
    {
        //��������ظ�Ԫ��
        int index = deleteList.FindIndex(item=>item.row==newItem.row&&item.col==newItem.col);
        //����list�вż���
        if (index ==-1)
        {
            deleteList.Add(newItem);
        }
    }
}
