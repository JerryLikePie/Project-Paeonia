using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utilities;

public class DollsCombat : MonoBehaviour
{

    // Dolls不同的战斗行为
    public IDollsCombatBehaviour combatBehaviour;
    [HideInInspector] public DollsProperty dolls;
    [HideInInspector] public float accurancyBuff, rangeBuff, dodgeBuff;
    public float health;
    public Slider healthBar;
    float percentageHealth;
    Gradient healthGradient;

    [HideInInspector] public GameObject allEnemy, allDolls;
    [HideInInspector] public EnemyCombat setEnemy;
    [HideInInspector] public MapCreate map;
    [HideInInspector] public Unit thisUnit;
    [HideInInspector] public bool beingSpotted = false;

    [HideInInspector] public Transform supportTargetCord;
    [HideInInspector] public Queue<Hex> toCancelFogQueue = new Queue<Hex>();
    [HideInInspector] public List<EnemyCombat> enemyList;
    public UnitEntity[] dollsEntities;

    public int crewNum;
    Hex nextTile;
    [HideInInspector] public Hex currentTile;
    public GameObject bullet;
    public AudioSource reloadStartSound, reloadEndSound;
    public int shotsInMag;
    BulletManager shot = new BulletManager();
    //public GameObject lineOfSight;

    [HideInInspector] public Vector3 planeVelocity;
    [HideInInspector] public float[] healthRestrictLine = { 0, 0, 0, 0, 0, 0 };


    [HideInInspector] public bool canFire = true;
    bool firstTime;

    [HideInInspector] public int counter, healthLevel, height, reloadStartTime;
    public float resetTime;
    float newMaxHealth;

    void Start()
    {
        healthGradient = new Gradient();
        
        counter = 0;
        for (int i = 0; i < allEnemy.transform.childCount; i++)
        {
            enemyList.Add(allEnemy.transform.GetChild(i).GetComponent<EnemyCombat>());
        }
        dolls = transform.GetComponent<DollsProperty>();
        thisUnit = transform.GetComponent<Unit>();
        //crewNum = PlayerPrefs.GetInt(dolls.dolls_id + "_crewNum", 1);
        CheckCrewNumber();
        shotsInMag = dolls.dolls_mag;
        SetHealthGradient(healthGradient);
    }
    void CheckCrewNumber()
    {

        int maxCrewNum = dolls.dolls_ammount;
        float crewPercentage = crewNum / (float)maxCrewNum;
        newMaxHealth = dolls.dolls_max_hp * crewPercentage;
        health = newMaxHealth;
        healthLevel = crewNum - 1;
        for (int i = 0; i <= crewNum; i++)
        {
            healthRestrictLine[i] = health * ((float)i / (float)crewNum);
        }
        for (int j = crewNum; j < dolls.dolls_ammount; j++)
        {
            dollsEntities[j].gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            UpdateHealthBar();
            combatBehaviour.CheckEnemy(this);
        }
    }
    void FireBullet()
    {
        if (setEnemy != null && setEnemy.gameObject.activeSelf == true && counter < crewNum)
        {
            try
            {
                GameObject bulletThatWasShot = Instantiate(bullet, dollsEntities[counter].transform.position, Quaternion.identity);
                bulletThatWasShot.SetActive(true);
                bulletThatWasShot.transform.LookAt(setEnemy.transform);
                shot = bulletThatWasShot.GetComponent<BulletManager>();
                shot.shotType = dolls.dolls_ammo_type;
                shot.speed = dolls.dolls_shell_speed;
                shot.WhereTheShotWillGo = setEnemy.transform.position;
                shot.damage = (dolls.dolls_sts_attack * dolls.dolls_damage_multiplier) * Random.Range(0.95f, 1.05f);
                shot.damageIndicate = shot.damage.ToString("F0");
                float randomPen = dolls.dolls_penetration + Random.Range(-2f, 2f);
                shot.penetration = randomPen;
                if (Random.Range(0, 100) < setEnemy.dodge - (dolls.dolls_accuracy + accurancyBuff))
                {
                    shot.damage = 0;
                    //判定，被闪避了那就miss
                    shot.damageIndicate = "miss";
                }
                shot.penetration = randomPen;
                shot.sender = dollsEntities[counter].gameObject;
                counter++;
                shot.enemyList = allEnemy;
                shot.dollsList = allDolls;
                shot.whoShotMe = "player";
                shot.firstImpact = true;
            } catch
            {
                Debug.LogError(counter);
            }
        } else if (setEnemy == null || !setEnemy.gameObject.activeSelf)
        {
            SwitchTarget();
            counter--;
            FireBullet();
        }
    }
    void SwitchTarget()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] != null)
            {
                if (enemyList[i].enemy.enemy_visible == true && enemyList[i].gameObject.activeSelf)
                {
                    if (FindDistance(transform.gameObject, enemyList[i].gameObject) <= 17.32 * (dolls.dolls_range + rangeBuff))
                    {
                        if (!map.IsBlocked(currentTile, enemyList[i].transform.position))
                        {
                            setEnemy = enemyList[i];
                        }
                    }
                }
            }
        }
    }
    void FireHowitzer()
    {
        GameObject bulletThatWasShot = Instantiate(bullet, dollsEntities[counter].transform.position, Quaternion.identity);
        bulletThatWasShot.SetActive(true);
        bulletThatWasShot.transform.LookAt(supportTargetCord);
        shot = bulletThatWasShot.GetComponent<BulletManager>();
        shot.speed = dolls.dolls_shell_speed;
        shot.WhereTheShotWillGo = supportTargetCord.position;
        shot.shotType = dolls.dolls_ammo_type;
        shot.damage = (dolls.dolls_sts_attack * dolls.dolls_damage_multiplier) * Random.Range(0.9f, 1.1f);
        shot.damageIndicate = shot.damage.ToString("F0");
        float randomPen = dolls.dolls_penetration + Random.Range(-10f, 10f);
        shot.penetration = randomPen;
        shot.sender = dollsEntities[counter].gameObject;
        counter++;
        shot.enemyList = allEnemy;
        shot.dollsList = allDolls;
        shot.whoShotMe = "player";
        shot.firstImpact = true;
    }
    void ThrowBomb()
    {
        Vector3 randomPoint = supportTargetCord.position + Random.Range(-17, 17) * Vector3.left + Random.Range(-17, 17) * Vector3.forward + 3 * Vector3.up;
        GameObject bulletThatWasShot = Instantiate(bullet, randomPoint, Quaternion.identity);
        bulletThatWasShot.SetActive(true);
        shot = bulletThatWasShot.GetComponent<BulletManager>();
        shot.speed = 3000;
        shot.WhereTheShotWillGo = randomPoint;
        shot.shotType = dolls.dolls_ammo_type;
        shot.damage = (dolls.dolls_ats_attack * dolls.dolls_damage_multiplier) * Random.Range(0.95f, 1.05f);
        float randomPen = dolls.dolls_penetration + Random.Range(-1f, 5f);
        shot.penetration = randomPen;
        shot.sender = dollsEntities[counter].gameObject;
        counter++;
        shot.enemyList = allEnemy;
        shot.dollsList = allDolls;
        shot.whoShotMe = "player";
        shot.firstImpact = true;
        //shot.GetComponent<Rigidbody>().velocity = planeVelocity * 70f;
    }

    public void Attack()
    {
        StartCoroutine(SetInactiveAfterFire());
        counter = 0;
        for (int i = 0; i < crewNum; i++)
        {
            if (dolls.dolls_type == 1)
            {
                Invoke("FireBullet", Random.Range(0f, 1f));
            }
            else if (dolls.dolls_type == 2)
            {
                Invoke("FireHowitzer", Random.Range(0f, 2f));
            }
            else if (dolls.dolls_type == 3)
            {
                Invoke("ThrowBomb", Random.Range(0f, 0.4f));
            }
        }
        counter = 0;

    }
    void ResetCord()
    {
        supportTargetCord = null;
    }
    void Withdrawl()
    {
        deFogOfWar();
        map.Score.FriendlyDead();
        transform.GetComponent<DollsCombat>().enabled = false;
        transform.gameObject.SetActive(false);
        map.transform.Find("Map" + thisUnit.hang + "_" + thisUnit.lie).GetComponent<Hex>().haveUnit = false;
    }
    void UpdateHealthBar()
    {
        percentageHealth = health / dolls.dolls_max_hp;
        
        if (health <= healthRestrictLine[healthLevel + 1] && dolls.dolls_type == 3)
        {
            health = health + 0.15f; //我们取消了免费的维修套件和灭火器，现在只有空军有了
        }
        
        if (health < healthRestrictLine[healthLevel])
        {
            //health = healthRestrictLine[healthLevel];
            dollsEntities[healthLevel].gameObject.SetActive(false);
            crewNum -= 1;
            healthLevel -= 1;
        }
        healthBar.fillRect.GetComponent<Image>().color = healthGradient.Evaluate(percentageHealth);
        healthBar.value = Mathf.Lerp(healthBar.value, percentageHealth, 100f * Time.deltaTime);
        if (health <= 0)
        {
            Withdrawl();
        }
    }

    public void CheckStatus()
    {
        currentTile = map.transform.Find("Map" + thisUnit.hang + "_" + thisUnit.lie).GetComponent<Hex>();
        if (currentTile.isSpotted <= 0)
        {
            beingSpotted = false;
        }
        else
        {
            beingSpotted = true;
        }
    }
    public void FogOfWar()
    {
        for (int i = 0; i <= map.transform.childCount - 1; i++)
        {
            try
            {
                nextTile = map.transform.GetChild(i).GetComponent<Hex>();
                if (nextTile != null && FindDistance(gameObject, nextTile.gameObject) <= 17.5 * (dolls.dolls_view_range + rangeBuff))
                {
                    if (!map.IsBlocked(currentTile, nextTile))
                    {
                        nextTile.GainVisual();
                        toCancelFogQueue.Enqueue(nextTile);
                        nextTile.UpdateFogStatus();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        //Debug.Log("这次进入了" + j + "个地块，但是，k = " + k);
    }
    public void deFogOfWar()
    {
        while (toCancelFogQueue.Count != 0)
        {
            Hex hex = toCancelFogQueue.Dequeue();
            hex.LoseVisual();
            hex.UpdateFogStatus();
        }
    }
    IEnumerator SetInactiveAfterFire()
    {
        yield return new WaitForSeconds(1f);
        beingSpotted = true;
        yield return new WaitForSeconds(1.5f);
        try
        {
            beingSpotted = false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    IEnumerator Reload()
    {
        reloadStartSound.Play();
        yield return new WaitForSeconds(dolls.dolls_reload - 2.5f);
        reloadEndSound.Play();
        yield return new WaitForSeconds(2.5f);
        shotsInMag = dolls.dolls_mag;
        canFire = true;
    }
    public IEnumerator FireRate()
    {
        canFire = false;
        if (shotsInMag > 1)
        {
            yield return new WaitForSeconds(dolls.dolls_firerate);
            shotsInMag -= 1;
            canFire = true;
        }
        else
        {
            StartCoroutine(Reload());
        }
    }
    public void RecieveDamage(float num)
    {
        if (healthLevel >= 0)
        {
            if ((health - num) < healthRestrictLine[healthLevel])
            {
                health = healthRestrictLine[healthLevel] - 1;
            }
            else
            {
                health -= num;
            }
        } else
        {
            health -= num;
        }
        
    }
    public void RecieveExplosiveDamage(float num)
    {
        health -= num;
    }

}