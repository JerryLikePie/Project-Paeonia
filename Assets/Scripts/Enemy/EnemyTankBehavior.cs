using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class EnemyTankBehavior : IEnemyCombatBehavior
{
    bool canFire;
    public int shotsInMag;
    EnemyProperty enemy;
    void Start()
    {
        canFire = true;
        enemy = transform.GetComponent<EnemyProperty>();
        shotsInMag = enemy.enemy_mag;
    }
    public override void checkDolls(EnemyCombat context)
    {
    }
    IEnumerator FireRate()
    {
        canFire = false;
        if (shotsInMag > 1)
        {
            yield return new WaitForSeconds(enemy.enemy_firerate);
            shotsInMag -= 1;
            canFire = true;
        }
        else
        {
            StartCoroutine(Reload());
        }
    }
    IEnumerator Reload()
    {
        //ReloadStart.Play();
        yield return new WaitForSeconds(enemy.enemy_reload - 2.5f);
        //ReloadEnd.Play();
        yield return new WaitForSeconds(2.5f);
        shotsInMag = enemy.enemy_mag;
        canFire = true;
    }
}
