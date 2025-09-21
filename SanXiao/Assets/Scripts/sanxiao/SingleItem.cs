using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;


public class SingleItem : MonoBehaviour
{
    public int row;//行号
    public int col;//列号

    public Vector3 startPos = new Vector3();
    public float width;
    public float height;

    public List<Sprite> sprites;//存放图片
    public List<Sprite> blockSprites;
    public int type;//用于比较两个item是否相同
    public SpriteRenderer itemSprite;
    public SpriteRenderer blockSprite;
    public GameObject glowing;

    public event Action<SingleItem> mouseButton;//当鼠标点击时，该委托会被添加事件

    

    /// <summary>
    /// 设置item的位置
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void BirthItem(int row,int col)
    {
        SetItemIndex(row,col);
        //设置单个item在场景中的位置
        SetItemWorldPos(row,col);
        //设置随机样式的图片
        SetItemSpriteRandom();
    }

    /// <summary>
    ///注册函数，item在生成时会在itemController中自动注册
    /// </summary>
    /// <param name="itemAction">鼠标点击事件</param>
    public void RegisterMouseButtonAction(Action<SingleItem> itemAction)
    {
        mouseButton += itemAction;
    }

    public void SetSelect(bool value)
    {
        glowing.SetActive(value);
    }

    public void SetItemSpriteRandom()
    {
        int index = UnityEngine.Random.Range(0,sprites.Count);
        type = index;
        itemSprite.sprite = sprites[index];
        blockSprite.sprite = blockSprites[index];
    }

    /// <summary>
    /// 从外部传入参数设置item的索引
    /// </summary>
    /// <param name="r">行号</param>
    /// <param name="c">列号</param>
    public void SetItemIndex(int r,int c)
    {
        row = r; col = c;
    }

    /// <summary>
    /// 元素移动位置
    /// </summary>
    /// <param name="r">要移动的行号</param>
    /// <param name="c">要移动的列号</param>
    /// <param name="t">移动速度</param>
    public void MoveTo(int r,int c,float t)
    {
        SetItemIndex(r,c);//将元素index设置为
        Vector3 pos = startPos + new Vector3(c * width, -r * height, 0);
        transform.DOMove(pos,t);
        
    }

    /// <summary>
    /// 比较两个元素是否相邻
    /// </summary>
    /// <param name="compareItem"></param>
    /// <returns></returns>
    public bool CompareTypeWith(SingleItem compareItem)
    {
        if (compareItem.col == col && Math.Abs(compareItem.row - row)==1)
        {
            return true;
        }
        if (compareItem.row == row && Math.Abs(compareItem.col-col)==1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置item的位置
    /// </summary>
    /// <param name="r"></param>
    /// <param name="c"></param>
    private void SetItemWorldPos(int r,int c)
    {
        transform.position = startPos + new Vector3(c * width, -r * height, 0);
    }

    /// <summary>
    /// 在鼠标点击item时自动调用
    /// </summary>
    private void OnMouseDown()
    {
        if (PlayerMgr.Instance.isPause == true)
            return;
        //鼠标点击item后，若事件不为空，调用事件
        if (mouseButton!=null)
        {
            //将自己作为参数传入
            mouseButton(this);
        }
    }
}
