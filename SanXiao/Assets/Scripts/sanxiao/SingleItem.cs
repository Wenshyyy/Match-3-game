using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;


public class SingleItem : MonoBehaviour
{
    public int row;//�к�
    public int col;//�к�

    public Vector3 startPos = new Vector3();
    public float width;
    public float height;

    public List<Sprite> sprites;//���ͼƬ
    public List<Sprite> blockSprites;
    public int type;//���ڱȽ�����item�Ƿ���ͬ
    public SpriteRenderer itemSprite;
    public SpriteRenderer blockSprite;
    public GameObject glowing;

    public event Action<SingleItem> mouseButton;//�������ʱ����ί�лᱻ����¼�

    

    /// <summary>
    /// ����item��λ��
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void BirthItem(int row,int col)
    {
        SetItemIndex(row,col);
        //���õ���item�ڳ����е�λ��
        SetItemWorldPos(row,col);
        //���������ʽ��ͼƬ
        SetItemSpriteRandom();
    }

    /// <summary>
    ///ע�ắ����item������ʱ����itemController���Զ�ע��
    /// </summary>
    /// <param name="itemAction">������¼�</param>
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
    /// ���ⲿ�����������item������
    /// </summary>
    /// <param name="r">�к�</param>
    /// <param name="c">�к�</param>
    public void SetItemIndex(int r,int c)
    {
        row = r; col = c;
    }

    /// <summary>
    /// Ԫ���ƶ�λ��
    /// </summary>
    /// <param name="r">Ҫ�ƶ����к�</param>
    /// <param name="c">Ҫ�ƶ����к�</param>
    /// <param name="t">�ƶ��ٶ�</param>
    public void MoveTo(int r,int c,float t)
    {
        SetItemIndex(r,c);//��Ԫ��index����Ϊ
        Vector3 pos = startPos + new Vector3(c * width, -r * height, 0);
        transform.DOMove(pos,t);
        
    }

    /// <summary>
    /// �Ƚ�����Ԫ���Ƿ�����
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
    /// ����item��λ��
    /// </summary>
    /// <param name="r"></param>
    /// <param name="c"></param>
    private void SetItemWorldPos(int r,int c)
    {
        transform.position = startPos + new Vector3(c * width, -r * height, 0);
    }

    /// <summary>
    /// �������itemʱ�Զ�����
    /// </summary>
    private void OnMouseDown()
    {
        if (PlayerMgr.Instance.isPause == true)
            return;
        //�����item�����¼���Ϊ�գ������¼�
        if (mouseButton!=null)
        {
            //���Լ���Ϊ��������
            mouseButton(this);
        }
    }
}
