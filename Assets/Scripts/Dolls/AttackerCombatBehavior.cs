using UnityEngine;
using UnityEditor;
using static Utilities;
using System.Collections.Generic;

// Air Unit
public class AttackerCombatBehavior : IDollsCombatBehaviour
{
    Vector3 airBase = new Vector3(-250, 0, 20);
    public Vector3 flyEndCord;
    private DollsCombat context;
    private Queue<Hex> toCancelFog;
    public float airSpeed;
    private long timenow;
    private float time;
    public bool canAttack;
    bool soundAlreadyPlayed = false;
    public AudioSource bombdrop;

    void Start()
    {
        toCancelFog = new Queue<Hex>();
        transform.position = airBase;
    }

    public override void CheckEnemy(DollsCombat context)
    {
        if (context.supportTargetCord != null && canAttack)
        {
            context.thisUnit.engineSound.volume = 1f;
            if (firstTime)
            {
                context.Invoke("ResetCord", context.resetTime);
                float distance = Vector3.Distance(transform.position, context.supportTargetCord.position);
                time = distance / airSpeed;
                //transform.position = Vector3.Lerp(transform.position, flyEndCord, time);
                firstTime = false;
            }
            transform.position = Vector3.Lerp(transform.position, flyEndCord, Time.deltaTime / time);
            AirRecon(context, 0);
            if (transform.position.x > context.supportTargetCord.position.x + 100)
            {
                if (context.canFire)
                {
                    context.planeVelocity = 0.4f * (flyEndCord - transform.position).normalized;
                    context.counter = 0;
                    context.Attack();
                    if (!soundAlreadyPlayed)
                    {
                        bombdrop.Play();
                        soundAlreadyPlayed = true;
                    }
                    StartCoroutine(context.FireRate());
                   
                }
            }
        } else if (context.supportTargetCord != null && !canAttack)
        {
            context.thisUnit.engineSound.volume = 1f;
            if (firstTime)
            {
                context.Invoke("ResetCord", context.resetTime);
                float distance = Vector3.Distance(transform.position, context.supportTargetCord.position);
                time = distance / airSpeed;
                //transform.position = Vector3.Lerp(transform.position, flyEndCord, time);
                firstTime = false;
            }
            transform.position = Vector3.Lerp(transform.position, flyEndCord, Time.deltaTime / time);
            AirRecon(context, 2);
        }
        else
        {
            context.thisUnit.engineSound.volume = 0f;
            transform.position = airBase;
        }
    }
    public void AirRecon(DollsCombat context, int increaseRange)
    {
        Hex NextTile;
        for (int i = 0; i <= context.map.transform.childCount - 1; i++)
        {
            try
            {
                NextTile = context.map.transform.GetChild(i).GetComponent<Hex>();
                if (FindDistance(gameObject, NextTile.gameObject) <= 17.5 * (context.dolls.dolls_view_range + increaseRange) && NextTile.isInFog == 0)
                {
                    if (!NextTile.blockVision)
                    {
                        NextTile.GainVisual();
                        toCancelFog.Enqueue(NextTile);
                        Invoke("DeFogOfWarOneAtATime", 10f);
                    } else
                    {
                        NextTile.render = true;
                        toCancelFog.Enqueue(NextTile);
                        Invoke("DeFogOfWarOneAtATime", 10f);
                    }
                }
                NextTile.UpdateFogStatus();

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
            hex.LoseVisual();
            hex.UpdateFogStatus();
        }
        catch
        {
        }
    }
}