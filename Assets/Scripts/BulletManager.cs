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
    public bool onCollision;

    void Start()
    {
        GameObject newSound = Instantiate(RoundFire, transform.position, Quaternion.identity);
        Destroy(newSound, 9f);
        Rigidbody obj = gameObject.GetComponent<Rigidbody>();
        randomDisplacement = new Vector3(Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange), Random.Range(-randomRange, randomRange));
        WhereTheShotWillGo += randomDisplacement;
        if (speed > 0) {
            obj.velocity = CalculateVelocity(WhereTheShotWillGo, sender, speed);
        }
        else if (speed < 0)
        {
            transform.Rotate(randomDisplacement);
        }
        Destroy(gameObject,8f);
    }

    // Update is called once per frame
    void Update()
    {
        if (onCollision)
        {
            CheckIfCollide();
        }
        else
        {
            CheckIfHit();
        }
    }
    void CheckIfHit()
    {
        // 查看是否已经打到了目标（持续检查）
        if (speed < 0)
        {
            // 如果初速度为0，给一个初速度
            transform.position += transform.forward * (-speed) * Time.deltaTime;
        }
        if (transform.position.y <= 0 || Vector3Equal(transform.position, WhereTheShotWillGo))
        {
            // 1. 打到了地面（y轴小于0），或者2.到达了目标坐标
            // 因为“锁敌人”的写法，如果在开炮的时候敌人移动，炮弹会追踪过去
            // 所以炮弹不锁任何敌人，而是锁地面坐标，此时敌人移开了那就是miss了
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
                        // 玩家打敌人
                        HitEnemy();
                    }
                    else if (whoShotMe == "enemy")
                    {
                        // 敌人打玩家
                        HitPlayer();
                    }
                    else if (whoShotMe == "hitAll")
                    {
                        // 炸弹和榴弹是打所有人
                        HitEnemy();
                        HitPlayer();
                    }
                }
                catch { }
                
            }
            // 删除该实体
            // 虽然回收弹药是更好的实现方式，但是那样又要关闭光效和特效还有声效……算了。
            Destroy(gameObject,0.05f);
        }
    }

    void CheckIfCollide()
    {
        // 这个是切换至collider来判定之后会使用的代码
        if (speed < 0)
        {
            transform.position += transform.forward * (-speed) * Time.deltaTime;
        }
        if (transform.position.y <= 0)
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
            Destroy(gameObject, 0.05f);
        }
    }

    //private void OnTriggerEnter(Collider collision)
    //{
    //    try
    //    {
    //        if (onCollision)
    //        {
    //            if (whoShotMe == "player")
    //            {
    //                if (collision.gameObject.tag == "Enemy")
    //                {

    //                    GameObject newSound = Instantiate(hitSoundEffect, transform.position, Quaternion.identity);
    //                    Destroy(newSound, 5f);
    //                    GameObject newVisual = Instantiate(hitVisualEffect, transform.position, Quaternion.identity);
    //                    Destroy(newVisual, 5f);
    //                    firstImpact = false;
    //                    HitEnemy(collision);
    //                    Destroy(gameObject, 0.05f);
    //                }
    //            }
    //            else if (whoShotMe == "enemy")
    //            {
    //                if (collision.gameObject.tag == "Friendly")
    //                {
    //                    GameObject newSound = Instantiate(hitSoundEffect, transform.position, Quaternion.identity);
    //                    Destroy(newSound, 5f);
    //                    GameObject newVisual = Instantiate(hitVisualEffect, transform.position, Quaternion.identity);
    //                    Destroy(newVisual, 5f);
    //                    firstImpact = false;
    //                    HitPlayer(collision);
    //                    Destroy(gameObject, 0.05f);
    //                }
    //            }
    //            else if (whoShotMe == "hitAll")
    //            {
    //                if (collision.gameObject.tag == "Friendly" || collision.gameObject.tag == "Enemy")
    //                {
    //                    GameObject newSound = Instantiate(hitSoundEffect, transform.position, Quaternion.identity);
    //                    Destroy(newSound, 5f);
    //                    GameObject newVisual = Instantiate(hitVisualEffect, transform.position, Quaternion.identity);
    //                    Destroy(newVisual, 5f);
    //                    firstImpact = false;
    //                    HitEnemy();
    //                    HitPlayer();
    //                    Destroy(gameObject, 0.05f);
    //                }
    //            }
    //        }
    //    }
    //    catch 
    //    {
    //        Debug.LogError("Well this bullet system is not done yet so.");
    //    }
        
    //}

    void HitPlayer()
    {
        // 打我方单位
        for (int i = 0; i <= dollsList.transform.childCount - 1; i++)
        {
            try
            {
                dolls = dollsList.transform.GetChild(i).GetComponent<DollsCombat>();
                if (transform.position.y <= 5 && dolls)
                {
                    if (Vector3.Distance(dolls.transform.position, transform.position) < 17.4f * DamageRange && dolls.gameObject.activeSelf && dolls.gameObject != sender)
                    {
                        // 如果：这个doll在这个位置且：dolls存在且：该炮弹不由此dolls发出
                        randomDisplacement = new Vector3(Random.Range(-5f, 5f), Random.Range(-1f, 1f), Random.Range(-5f, 5f));
                        if (damageIndicate == "miss")
                        {
                            GameObject damageText = Instantiate(DamageIndicator, dolls.transform.position + randomDisplacement, Quaternion.identity);
                            damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.gray;
                            damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
                            Destroy(damageText, 1.5f);
                            continue;
                        }
                        // 判定是否可以击穿
                        // 先给出入射角
                        Vector3 senderangle = sender.transform.position - dolls.transform.position;
                        if (shotType == 1 || shotType == 4)
                        {
                            // 如果是高爆和炸弹的话，计算角度则由爆炸点到敌人位置
                            senderangle = transform.position - dolls.transform.position;

                        }
                        senderangle.y = 0.0f;
                        float armor = 0.0f;
                        if (Vector3.Magnitude(senderangle) < 1.0f)
                        {
                            // 因为会有双方同时进入一个地块的情况出现，在同一地块上发起的攻击视为白刃战，无视护甲且伤害翻倍
                            damage *= 2;
                        }
                        else
                        {
                            // 通过入射角计算装甲等效
                            armor = dolls.CalculateArmorVal(senderangle);
                            //Debug.Log(penetration + ", " + armor);
                            // 是否击穿？
                            if (penetration < armor)
                            {
                                // 未击穿！
                                damageIndicate = "hit";
                                damage = 0;
                            }
                        }
                        
                        // 显示伤害
                        if (damageIndicate == "hit")
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
                            if (shotType == 1 || shotType == 4)
                            {
                                // 1是高爆 4是炸弹
                                // 高爆伤害不以射入角评判
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

    //void HitPlayer(Collider col)
    //{
    //    dolls = col.GetComponentInParent<DollsCombat>();
    //    if (dolls == null)
    //    {
    //        return;
    //    }
    //    if (!dolls.gameObject.activeSelf)
    //    {
    //        return;
    //    }
    //    randomDisplacement = new Vector3(Random.Range(-5f, 5f), Random.Range(-1f, 1f), Random.Range(-5f, 5f));
    //    if (damageIndicate == "miss")
    //    {
    //        GameObject damageText = Instantiate(DamageIndicator, dolls.transform.position + randomDisplacement, Quaternion.identity);
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.gray;
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
    //        Destroy(damageText, 1.5f);
    //    }
    //    else if (damageIndicate == "hit")
    //    {
    //        GameObject damageText = Instantiate(DamageIndicator, dolls.transform.position + randomDisplacement, Quaternion.identity);
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
    //        Destroy(damageText, 1.5f);
    //    }
    //    else
    //    {
    //        GameObject damageText = Instantiate(DamageIndicator, dolls.transform.position + randomDisplacement, Quaternion.identity);
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
    //        Destroy(damageText, 1.5f);
    //        if (shotType == 2 || shotType == 4)
    //        {
    //            Debug.Log("高爆弹");
    //            dolls.RecieveExplosiveDamage(damage);
    //        }
    //        else
    //        {
    //            dolls.RecieveDamage(damage);
    //        }

    //    }
    //}

    void HitEnemy()
    {
        // 我方打敌人
        for (int i = 0; i <= enemyList.transform.childCount - 1; i++)
        {
            enemy = enemyList.transform.GetChild(i).GetComponent<EnemyCombat>();
            if (transform.position.y <= 5)
            {
                if (Vector3.Distance(enemy.transform.position, transform.position) < 17.4f * DamageRange && enemy.gameObject.activeSelf)
                {
                    if (shotType == 1 || shotType == 4)
                    {
                        // 1为高爆 4为炸弹，高爆伤害会因为距离衰减
                        damage = damage * enemy.enemy.enemy_damage_recieved_multiplier[shotType] * (1 / (Vector3.Distance(enemy.transform.position, transform.position) / 17.3f));
                    } else
                    {
                        // 根据不同弹种的乘区来计算伤害
                        damage = damage * enemy.enemy.enemy_damage_recieved_multiplier[shotType];
                    }

                    // 计算敌方的装甲等效
                    // 先给出入射角
                    Vector3 senderangle = sender.transform.position - enemy.transform.position;
                    if (shotType == 1 || shotType == 4)
                    {
                        // 如果是高爆和炸弹的话，计算角度则由爆炸点到敌人位置
                        senderangle = transform.position - enemy.transform.position;

                    }
                    senderangle.y = 0.0f;
                    float armor = 0.0f;
                    if (Vector3.Magnitude(senderangle) < 1.0f)
                    {
                        // 因为会有双方同时进入一个地块的情况出现，在同一地块上发起的攻击视为白刃战，无视护甲且伤害翻倍
                        damage *= 2;
                    }
                    else
                    {
                        // 通过入射角计算装甲等效
                        armor = enemy.CalculateArmorVal(senderangle);
                        // 是否击穿？
                        if (penetration < armor)
                        {
                            // 未击穿！
                            damageIndicate = "hit";
                            damage = 0;
                        }
                    }

                    randomDisplacement = new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 1f), Random.Range(-3f, 3f));
                    
                    if (damageIndicate == "miss")
                    {
                        GameObject damageText = Instantiate(DamageIndicator, enemy.transform.position + randomDisplacement, Quaternion.identity);
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
                        if (shotType == 1 || shotType == 4)
                        {
                            // 1是高爆弹 4是航空炸弹
                            // 高爆无视我方和敌方的数量
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

    //void HitEnemy(Collider col)
    //{
    //    enemy = col.GetComponentInParent<EnemyCombat>();
    //    if (enemy == null)
    //    {
    //        return;
    //    }
    //    if (!enemy.gameObject.activeSelf)
    //    {
    //        return;
    //    }
    //    if (shotType == 2 || shotType == 4)
    //    {
    //        damage = damage * enemy.enemy.enemy_damage_recieved_multiplier[shotType] * (1 / (Vector3.Distance(enemy.transform.position, transform.position) / 17.3f));
    //    }
    //    else
    //    {
    //        damage = damage * enemy.enemy.enemy_damage_recieved_multiplier[shotType];

    //    }
    //    randomDisplacement = new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 1f), Random.Range(-3f, 3f));

    //    if (damageIndicate == "miss")
    //    {
    //        GameObject damageText = Instantiate(DamageIndicator, enemy.transform.position + randomDisplacement, Quaternion.identity);
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
    //        Destroy(damageText, 1.5f);
    //    }
    //    else if (enemy.enemy.enemy_armor_front > penetration)
    //    {
    //        GameObject damageText = Instantiate(DamageIndicator, enemy.transform.position + randomDisplacement, Quaternion.identity);
    //        damage = 0;
    //        damageIndicate = "hit";
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
    //        Destroy(damageText, 1.5f);
    //    }
    //    else
    //    {
    //        GameObject damageText = Instantiate(DamageIndicator, enemy.transform.position + randomDisplacement, Quaternion.identity);
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().color = Color.white;
    //        damageText.GetComponentInChildren<TMPro.TextMeshPro>().text = damageIndicate;
    //        Destroy(damageText, 1.5f);
    //        if (shotType == 2 || shotType == 4)
    //        {
    //            enemy.RecieveExplosiveDamage(damage);
    //        }
    //        else
    //        {
    //            enemy.RecieveDamage(damage);
    //        }
    //    }
    //}

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