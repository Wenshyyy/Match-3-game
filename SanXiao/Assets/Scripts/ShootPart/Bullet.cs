using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum E_BulletType
{
    Dian = 0,
    Fire = 1,
    Ice = 2,
    Stone = 3,
    Wood = 5,
    Water =4,
    Money = 6
}

public class Bullet : MonoBehaviour
{
    public E_BulletType bullteType;
    public List<Sprite> SpritesList; //子弹样式List
    public SpriteRenderer bulletsprite; //子弹样式图片
    public float basicAtk = 5f;
    public float atk;

    public List<AudioClip> audioClips;
       
    
    
    public void BirthBullet(E_BulletType type)
    {       
        //设置子弹的类型 子弹类型等于传进来的类型
        bullteType = type;
        int typeIndex = (int)type;
        //设置子弹图片样式
        bulletsprite.sprite = SpritesList[typeIndex];

        switch (type)
        {
            case E_BulletType.Water:
                atk *= 0.8f;
                break;
            case E_BulletType.Stone:
                atk = basicAtk* 1.4f;
                break;
            case E_BulletType.Dian:
                atk = basicAtk* 1.6f;
                break;
            case E_BulletType.Fire:               
            case E_BulletType.Ice:
                atk = basicAtk* 2f;                      
                break;
            default:
                atk = basicAtk;
                break;
        }
       
    }

    public void ToRelease(GameObject b)
    {
        BulletPool.Instance.ReleaseBullet(b);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Mouster"))
        {
            ToRelease(this.gameObject);          
            switch (bullteType)
            {
                case E_BulletType.Dian:
                    AudioSource.PlayClipAtPoint(audioClips[0],transform.position);
                    break;              
                case E_BulletType.Fire:
                    AudioSource.PlayClipAtPoint(audioClips[1], transform.position);
                    break;
                case E_BulletType.Ice:
                    AudioSource.PlayClipAtPoint(audioClips[2], transform.position);
                    break;
                case E_BulletType.Water:
                    AudioSource.PlayClipAtPoint(audioClips[3], transform.position);
                    break;
                default:
                    AudioSource.PlayClipAtPoint(audioClips[4], transform.position,2f);
                    break;
            }
            other.gameObject.GetComponent<Monster>().GetHit(this);
            return;
        }
        if (other.CompareTag("Wall"))
        {
            ToRelease(this.gameObject);           
        }
    }
    

}
