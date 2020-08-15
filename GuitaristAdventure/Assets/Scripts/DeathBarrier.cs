using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarrier : MonoBehaviour
{
    public void Kill(GameObject obj)
    {
        PlayerHealthManager player = obj.GetComponent<PlayerHealthManager>();
        if (player)
        {
            player.TakeDamage(100);
        }
        else
        {
            SmasherHealthManager smasher = obj.GetComponent<SmasherHealthManager>();
            if (smasher)
            {
                smasher.TakeDamage(100);
            }
            else
            {
                MonsterHealthManager monster = obj.GetComponent<MonsterHealthManager>();
                if (monster)
                {
                    monster.TakeDamage(100);
                    
                }
            }
        }
    }
}
