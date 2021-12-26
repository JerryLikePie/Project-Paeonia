using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utilities;

public class BulletManager : MonoBehaviour
{
    [HideInInspector] public Vector3 parentVelocity;
    [HideInInspector] public float damage;
    [HideInInspector] public string damageIndicate;
    [HideInInspector] public GameObject sender;
    [HideInInspector] public GameObject enemyList;
    [HideInInspector] public GameObject dollsList;
    [HideInInspector] public Vector3 WhereTheShotWillGo;
    public GameObject bullet;
    [HideInInspector] public string whoShotMe;
    [HideInInspector] public float speed;
    [HideInInspector] public bool firstImpact;
    [HideInInspector] public float penetration;
    public GameObject DamageIndicator;
    [HideInInspector] public int shotType;
    Vector3 velocity;
    //public ParticleManager parti;
    //public GameObject parti;
    public GameObject RoundFire;
    public GameObject hitSoundEffect,hitVisualEffect;
    EnemyCombat enemy;
    DollsCombat dolls;
    public float DamageRange;
    Vector3 randomDisplacement;
    public float randomRange;

    void Start()
    {
        GameObject newSound = Instantiate(RoundFire, transform.position, Quaternion.identity);
        Destroy(newSound, 9f);
        Rigidbody obj = gameObject.GetComponent<Rigidbody>();
        randomDisplacement = new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
        WhereTheShotWillGo += randomDisplacement;
        if (speed != 0) {
            obj.velocity = CalculateVelocity(WhereTheShotWillGo, sender, speed);
        }
        Destroy(gameObject,9f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckIfHit();
    }
    void CheckIfHit()
    {
        if (transform.position.y <= 0 || Vector3Equal(transform.position, WhereTheShotWillGo))
        {
            bullet.SetActive(false);
            if (firstImpact == true)
            {
                try
                {
                    GameObject newSound = Instantiate(hitSoundEffect, transform.position, Quaternion.identity);
                    Destroy(newSound, 5f);
                    GameObject newVisual = Instantiate(hitVisualEffect, transform.position, Quaternion.identity);
                    Destroy(newVisual, 5f);
                    firstImpact = false;
                    if (whoShotMe == "player")
                    {
                        HitEnemy();
                    }
                    else if (whoShotMe == "enemy")
                    {
                        HitPlayer();
                    }
                    else if (whoShotMe == "hitAll")
                    {
                        HitEnemy();
                        HitPlayer();
                    }
                }
                catch { }
                
            }
            Destroy(gameObject,0.5f);
        }
    }
    void HitPlayer()
    {
        for (int i = 0; i <= dollsList.transform.childCount - 1; i++)
        {
            try
            {
                dolls = dollsList.transform.GetChild(i).GetComponent<DollsCombat>();
                if (transform.position.y <= 5 && dolls)
                {
                    if (Vector3.Distance(dolls.transform.position, transform.position) < 17.4f * DamageRange && dolls.gameObject.activeSelf && dolls.gameObject != sender)
                    {
                        
                        randomDisplacement = new Vector3(Random.Range(-5f, 5f), Random.Range(-1f, 1f), Random.Range(-5f, 5f));
                        if (damageIndicate == "miss")
                        {
                            GameObject damageText = Instantiate(DamageIndicator, dolls.transform.position + randomDisplacement, Quaternion.identity);
                            damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
                            damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
                            Destroy(damageText, 1.5f);
                        }
                        else if (damageIndicate == "hit")
                        {
                            GameObject damageText = Instantiate(DamageIndicator, dolls.transform.position + randomDisplacement, Quaternion.identity);
                            damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
                            damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
                            Destroy(damageText, 1.5f);
                        }
                        else
                        {
                            GameObject damageText = Instantiate(DamageIndicator, dolls.transform.position + randomDisplacement, Quaternion.identity);
                            damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
                            damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
                            Destroy(damageText, 1.5f);
                            if (shotType == 2 || shotType == 4)
                            {
                                Debug.Log("¸ß±¬µ¯");
                                dolls.RecieveExplosiveDamage(damage);
                            }
                            else
                            {
                                dolls.RecieveDamage(damage);
                            }
                            
                        }
                        
                    }
                }
            } catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
    void HitEnemy()
    {
        for (int i = 0; i <= enemyList.transform.childCount - 1; i++)
        {
            enemy = enemyList.transform.GetChild(i).GetComponent<EnemyCombat>();
            if (transform.position.y <= 5)
            {
                if (Vector3.Distance(enemy.transform.position, transform.position) < 17.4f * DamageRange && enemy.gameObject.activeSelf)
                {
                    if (shotType == 2 || shotType == 4)
                    {
                        damage = damage * enemy.enemy.enemy_damage_recieved_multiplier[shotType] * (1 / (Vector3.Distance(enemy.transform.position, transform.position) / 17.3f));
                    } else
                    {
                        damage = damage * enemy.enemy.enemy_damage_recieved_multiplier[shotType];

                    }
                    randomDisplacement = new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 1f), Random.Range(-3f, 3f));
                    
                    if (damageIndicate == "miss")
                    {
                        GameObject damageText = Instantiate(DamageIndicator, enemy.transform.position + randomDisplacement, Quaternion.identity);
                        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
                        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
                        Destroy(damageText, 1.5f);
                    }
                    else if (enemy.enemy.enemy_armor_front > penetration)
                    {
                        GameObject damageText = Instantiate(DamageIndicator, enemy.transform.position + randomDisplacement, Quaternion.identity);
                        damage = 0;
                        damageIndicate = "hit";
                        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
                        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
                        Destroy(damageText, 1.5f);
                    }
                    else
                    {
                        GameObject damageText = Instantiate(DamageIndicator, enemy.transform.position + randomDisplacement, Quaternion.identity);
                        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
                        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
                        Destroy(damageText, 1.5f);
                        if (shotType == 2 || shotType == 4)
                        {
                            enemy.RecieveExplosiveDamage(damage);
                        }
                        else
                        {
                            enemy.RecieveDamage(damage);
                        }
                    }
                    
                    
                }
            }
           
        }
    }

    Vector3 CalculateVelocity(Vector3 target, GameObject sender, float speed)
    {
        //define the distance x and y first
        Vector3 origin = sender.transform.position;
        Vector3 distance = target - origin;
        float time = distance.sqrMagnitude / speed;
        //trail.time = time;
        Vector3 distance_x_z = distance;
        distance_x_z.Normalize();
        distance_x_z.y = 0;

        //creating a float that represents our distance 
        float sy = distance.y;
        float sxz = distance.magnitude;


        //calculating initial x velocity
        //Vx = x / t
        float Vxz = (sxz / time) + parentVelocity.magnitude;

        ////calculating initial y velocity
        //Vy0 = y/t + 1/2 * g * t
        float Vy = sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

        Vector3 result = distance_x_z * Vxz;
        result.y = Vy;

        return result;
    }
}