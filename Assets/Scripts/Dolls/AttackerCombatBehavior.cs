using UnityEngine;
using UnityEditor;
using static Utilities;
using System.Collections.Generic;

// Air Unit
public class AttackerCombatBehavior : IDollsCombatBehaviour
{
    Vector3 airBase = new Vector3(-250, 0, 150);
    public Vector3 flyEndCord;
    private DollsCombat context;
    private Queue<Hex> toCancelFog;

    void Start()
    {
        toCancelFog = new Queue<Hex>();
    }

    public override void CheckEnemy(DollsCombat context)
    {
        if (context.supportTargetCord != null)
        {
            if (Vector3.Distance(transform.position, flyEndCord) < 30)
            {
                Invoke("ResetCord", context.resetTime);
            }
            AirRecon(context);
            Vector3 direction = flyEndCord - transform.position;
            Vector3 velocity = direction.normalized * 1.5f;
            //planeVelocity = velocity;
            velocity = Vector3.ClampMagnitude(velocity, direction.magnitude);
            transform.Translate(velocity);
            if (transform.position.x > context.supportTargetCord.position.x - 20)
            {
                try
                {
                    for (int i = 1; i <= context.enemy.Count; i++)
                    {
                        if (Find_Distance(transform.gameObject, context.enemy[i].gameObject) <= 40)
                        {
                            if (context.enemy[i].enemy.enemy_visible == true && context.enemy[i].gameObject.activeSelf)
                            {
                                if (context.canFire)
                                {
                                    context.setEnemy = context.enemy[i];
                                    context.counter = 0;
                                    context.Attack();
                                    StartCoroutine(context.FireRate());
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
        else
        {
            transform.position = airBase;
        }
    }

    private void AirRecon(DollsCombat context)
    {
        Hex NextTile;
        for (int i = 0; i <= context.map.transform.childCount - 1; i++)
        {
            try
            {
                NextTile = context.map.transform.GetChild(i).GetComponent<Hex>();
                if (Find_Distance(gameObject, NextTile.gameObject) <= 17.8 * context.dolls.dolls_view_range && NextTile.isInFog == 0)
                {
                    NextTile.isInFog += 1;
                    toCancelFog.Enqueue(NextTile);
                    Invoke("DeFogOfWarOneAtATime", 10f);
                }
                NextTile.ChangeTheFog();

            }
            catch
            {
                continue;
            }
        }
    }
    void DeFogOfWarOneAtATime()
    {
        try
        {
            Hex hex = toCancelFog.Dequeue();
            hex.isInFog -= 1;
            hex.ChangeTheFog();
        }
        catch
        {
        }
    }
}