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
    public int row;//行数
    public int col;//列数
    public float time;

    private SingleItem[,] itemList;//三消元素List
    private ElementPool itemPool; //获取对象池

    private SingleItem itemFirst;//第一次选择的元素
    //private bool canDelete;
    private List<SingleItem> deleteList;//删除列表

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
        //判断新生成的元素是否需要消除
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
    /// 实例化元素
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public SingleItem NewItem(int row,int col)
    {
        //从对象池中获得实例
        SingleItem item = itemPool.GetElement();
        //将item放到指定的位置
        item.transform.SetParent(this.transform);//将item的父物体设置为当前对象
        item.BirthItem(row,col);

        //在item生成的时候，为item注册鼠标点击事件,并将自己作为参数传入
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
            //交换前判断是否相邻
            if (item.CompareTypeWith(itemFirst))
            {
                //true交换位置              
                StartCoroutine(ExchangeItems(itemFirst, item)) ;//将第一次点击的元素和传入的元素交换位置               
            }
            itemFirst.SetSelect(false);
            item.SetSelect(true);
            itemFirst = item;
        }
    }

    /// <summary>
    /// 交换两个元素的位置
    /// </summary>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    private void Exchange(SingleItem item1,SingleItem item2)
    {
        //itemList 二维数组更换两个单个元素的数据
        itemList[item1.row, item1.col] = item2;
        itemList[item2.row, item2.col] = item1;
        //处理单个元素内部行列数据
        //临时变量
        int temp1_col ,temp1_row,temp2_col,temp2_row;
        temp1_col = item1.col;
        temp1_row = item1.row;
        temp2_col = item2.col;
        temp2_row = item2.row;

        item1.SetItemIndex(temp2_row,temp2_col);
        item2.SetItemIndex(temp1_row,temp1_col);
        //移动位置
        item1.MoveTo(item1.row,item1.col,time);
        item2.MoveTo(item2.row,item2.col,time);

    }

    /// <summary>
    /// 交换和不满足三消的条件执行的方法
    /// </summary>
    IEnumerator ExchangeItems(SingleItem item1,SingleItem item2)
    {
        Exchange(item1,item2);  
        yield return new WaitForSeconds(time);
        //bool canDelete = canDelete;
        if (CheckDelete())
        {
            //消除需要消除的元素
            DeleteItem();
        }
        else
        {          
            Exchange(item1,item2);
        }

        

    }
    private void DeleteItem()
    {
         //从存放删除元素的List中依此拿出来
        int count = deleteList.Count;
        for (int i=0;i<count;i++)
        {
            SingleItem item = deleteList[i];
            //申明临时行列号，存储当前元素的位置
            int temp_row = item.row;
            int temp_col = item.col;      
            itemPool.ReleaseElement(item);
            itemList[temp_row,temp_col] = null;//置空


            //将此元素先消除，再将其上方的所有元素依此向下挪动一次
             
            for (int j=temp_row-1; j>=0; j--)
            {
                //用一个临时变量存储每一向下移动的元素
                SingleItem itemTemp = itemList[j,temp_col];
                //将临时变量的位置设为正下方
                itemList[j + 1, itemTemp.col] = itemTemp;
                //原来的位置设置为空
                itemList[j, itemTemp.col] = null;
                //将临时变量内部的索引设置成当前位置
                itemTemp.SetItemIndex(j + 1, itemTemp.col);
                //游戏界面内移动到当前位置
                itemTemp.MoveTo(itemTemp.row,itemTemp.col,time);
            }

            //再在最顶上生成一个元素
            SingleItem newItem = itemPool.GetElement();
            newItem.transform.SetParent(transform);
            newItem.BirthItem(-1,item.col);
            newItem.MoveTo(0,newItem.col,time);
            itemList[0,newItem.col] = newItem;
        }
        //清空删除列表
        deleteList = new List<SingleItem>();

        //因为生成了新元素，所以还要再检查一下是否需要消除
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
                //判断横向三个元素是否都存在
                if (r < row - 2 && itemList[r,c] != null && itemList[r + 1, c] != null && itemList[r + 2, c] != null)
                {
                    //判断类型是否相同
                    if (itemList[r,c].type == itemList[r+1,c].type&& itemList[r, c].type == itemList[r + 2, c].type)
                    {
                        AddToDeleteList(itemList[r,c]);
                        AddToDeleteList(itemList[r+1, c]);
                        AddToDeleteList(itemList[r+2, c]);
                        canDelete = true;
                        //传递待添加子弹的类型
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
                        //传递待添加子弹的类型
                        PlayerMgr.Instance.bulletTypeQueue.Enqueue((E_BulletType)itemList[r, c].type);
                    }
                }
                

            }
        }
        return canDelete;
    }

    private void AddToDeleteList(SingleItem newItem)
    {
        //避免加入重复元素
        int index = deleteList.FindIndex(item=>item.row==newItem.row&&item.col==newItem.col);
        //不在list中才加入
        if (index ==-1)
        {
            deleteList.Add(newItem);
        }
    }
}
