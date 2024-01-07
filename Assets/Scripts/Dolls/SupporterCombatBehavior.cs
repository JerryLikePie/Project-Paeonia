using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupporterCombatBehavior : IDollsCombatBehaviour
{
    private DollsCombat context;
    private Queue<Hex> toCancelFog;
    public GameObject questionMarker, spottedMarker;
    GameObject question, spot;
    bool inQuestion = false, enemyArtyWoke = false;

    void Start()
    {
        toCancelFog = new Queue<Hex>();
    }

    public override void WreckAfterDead(DollsCombat context, Transform deadbody)
    {
        // ���ɲк�
        if (context.wreckage == null)
        {
            // û�а󶨲к����ǾͲ�������
            Debug.Log("No wreckage?");
            return;
        }
        // TODO: дһ��½��dolls�Ĳк��ű�
    }
    public override void CheckEnemy(DollsCombat context)
    {
        if (context.supportTargetCord != null && context.canFire)
        {
            context.Attack();
            if (firstTime)
            {
                if (context.beingSpotted || enemyArtyWoke)
                {
                    spot = Instantiate(spottedMarker, transform.position, Quaternion.identity);
                    spot.transform.SetParent(transform.parent);
                    enemyArtyWoke = true;
                    Destroy(spot, 10f);
                    if (inQuestion)
                    {
                        Destroy(question);
                    }
                } else
                {
                    if (!inQuestion)
                    {
                        question = Instantiate(questionMarker, transform.position, Quaternion.identity);
                        question.transform.SetParent(transform.parent);
                        inQuestion = true;
                    } else
                    {
                        if (Vector3.Distance(transform.position, question.transform.position) <= 17.5f)
                        {
                            inQuestion = false;
                            Destroy(question);
                            spot = Instantiate(spottedMarker, transform.position, Quaternion.identity);
                            spot.transform.SetParent(transform.parent);
                            Destroy(spot, 10f);
                        } else
                        {
                            inQuestion = false;
                            Destroy(question);
                            question = Instantiate(questionMarker, transform.position, Quaternion.identity);
                            question.transform.SetParent(transform.parent);
                            inQuestion = true;
                        }
                    }
                }
                context.Invoke("ResetCord", context.resetTime);
                firstTime = false;
            }
            StartCoroutine(context.FireRate());
            context.counter = 0;
        }
    }
}
