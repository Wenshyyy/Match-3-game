using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMgr :Singleton<PlayerMgr>
{
    public Queue<E_BulletType> bulletTypeQueue = new Queue<E_BulletType>();
    public bool isPause;
    public int killCount = 0;
    public int score = 0;

    public int playerHp = 3;

}
