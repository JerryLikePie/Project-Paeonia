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
        transform.position = airBase;
    }

    public override void CheckEnemy(DollsCombat context)
    {
        if (context.supportTargetCord != null)
        {
            if (firstTime)
            {
                context.Invoke("ResetCord", context.resetTime);
                Invoke("backToBase", 30f);
                firstTime = false;
            }
            AirRecon(context);
            Vector3 direction = flyEndCord - transform.position;
            Vector3 velocity = direction.normalized;
            velocity = Vector3.ClampMagnitude(velocity, 0.5f);
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
                                    context.planeVelocity = velocity;
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
    }
    void backToBase()
    {
        transform.position = airBase;
        Vector3 velocity = new Vector3(0f,0f,0f);
        transform.Translate(velocity);
    }
    public void AirRecon(DollsCombat context)
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