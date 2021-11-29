using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DollsCombat : MonoBehaviour
{

    // Dolls不同的战斗行为
    public IDollsCombatBehaviour combatBehaviour;
    [HideInInspector] public DollsProperty dolls;
    [HideInInspector] public float health, accurancyBuff, rangeBuff, dodgeBuff;

    public Slider healthBar;
    float percentageHealth;
    public Gradient FullHealthNoHealth;

    [HideInInspector] public GameObject enemiesList, dollsList;
    [HideInInspector] public EnemyCombat setEnemy;
    [HideInInspector] public GameObject map;
    [HideInInspector] public bool beingSpotted = false;

    [HideInInspector] public Transform supportTargetCord;
    [HideInInspector] public Queue<Hex> ToCancelFog = new Queue<Hex>();
    public List<EnemyCombat> enemy;
    public UnitEntity[] dollsEntities;

    public int crewNum;
    Hex NextTile;
    public GameObject bullet;
    public AudioSource Fire, Fire2, FireBase, ReloadStart, ReloadEnd, SkillSound;
    public int shotsInMag;
    BulletManager shot = new BulletManager();
    public GameObject LineofSight;

    [HideInInspector] public Vector3 planeVelocity;
    [HideInInspector] public float[] healthRestrictLine = {0,0,0,0,0,0};
    [HideInInspector] public Unit thisUnit;
    
    [HideInInspector] public bool canFire = true;
    bool firstTime;
    
    [HideInInspector] public int counter, healthLevel, height, ReloadStartTime;
    public float resetTime;
    float newMaxHealth;

    void Start()
    {
        counter = 0;
        for (int i = 0; i < enemiesList.transform.childCount; i++)
        {
            enemy.Add(enemiesList.transform.GetChild(i).GetComponent<EnemyCombat>());
        }
        dolls = transform.GetComponent<DollsProperty>();
        thisUnit = transform.GetComponent<Unit>();
        crewNum = PlayerPrefs.GetInt(dolls.dolls_id + "_crewNum", 1);
        checkCrewNumber();
        shotsInMag = dolls.dolls_mag;
        healthLevel = crewNum - 1;
    }
    void checkCrewNumber()
    {
        int maxCrewNum = dolls.dolls_ammount;
        float crewPercentage = crewNum / (float)maxCrewNum;
        newMaxHealth = dolls.dolls_max_hp * crewPercentage;
        health = newMaxHealth;
        for (int i = 0; i <= crewNum; i++)
        {
            healthRestrictLine[i] = health * ((float)i / (float)crewNum);
        }
        for (int j = crewNum; j <dolls.dolls_ammount; j++)
        {
            dollsEntities[j].gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            updateHealthBar();
            combatBehaviour.CheckEnemy(this);
        }
    }
    void FireBullet()
    {
        if (setEnemy != null && setEnemy.gameObject.activeSelf == true)
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
            shot.enemyList = enemiesList;
            shot.dollsList = dollsList;
            shot.whoShotMe = "player";
            shot.firstImpact = true;
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
        shot.enemyList = enemiesList;
        shot.dollsList = dollsList;
        shot.whoShotMe = "hitAll";
        shot.firstImpact = true;
    }
    void ThrowBomb()
    {
        GameObject bulletThatWasShot = Instantiate(bullet, dollsEntities[counter].transform.position, Quaternion.identity);
        bulletThatWasShot.SetActive(true);
        shot = bulletThatWasShot.GetComponent<BulletManager>();
        shot.speed = 0;
        shot.WhereTheShotWillGo = transform.position;
        shot.shotType = dolls.dolls_ammo_type;
        shot.damage = (dolls.dolls_ats_attack * dolls.dolls_damage_multiplier) * Random.Range(0.95f, 1.05f);
        float randomPen = dolls.dolls_penetration + Random.Range(-1f, 5f);
        shot.penetration = randomPen;
        shot.sender = dollsEntities[counter].gameObject;
        counter++;
        shot.enemyList = enemiesList;
        shot.dollsList = dollsList;
        shot.whoShotMe = "hitAll";
        shot.firstImpact = true;
        shot.GetComponent<Rigidbody>().velocity = planeVelocity * 70f;
    }

    public void Attack()
    {
        StartCoroutine(SetInactiveAfterFire());
        for (int i = 1; i <= crewNum; i++)
        {
            Debug.Log(i + " -> " + crewNum);
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
                Invoke("ThrowBomb", Random.Range(0f,0.1f));
            }
        }
        counter = 0;
    }
    void resetCord()
    {
        supportTargetCord = null;
    }
    void WithDrawl()
    {
        map.GetComponent<MapCreate>().Score.killedDolls += 1;
        transform.GetComponent<DollsCombat>().enabled = false;
        transform.gameObject.SetActive(false);
        map.transform.Find("Map" + thisUnit.hang + "_" + thisUnit.lie).GetComponent<Hex>().haveUnit = false;
    }
    void updateHealthBar()
    {
        percentageHealth = health / newMaxHealth;
        healthBar.fillRect.GetComponent<Image>().color = FullHealthNoHealth.Evaluate(percentageHealth);
        if (health <= dolls.dolls_max_hp)
        {
            health = health + 0.1f;//我们为所有dolls都购买了维修套件和灭火器
        }
        healthBar.value = Mathf.Lerp(healthBar.value, percentageHealth, 100f * Time.deltaTime);
        if (health < healthRestrictLine[healthLevel])
        {
            health = healthRestrictLine[healthLevel];
            dollsEntities[healthLevel].gameObject.SetActive(false);
            crewNum -= 1;
            healthLevel -= 1;
        }
        if (health <= 0)
        {
            WithDrawl();
        }
    }
    public float Find_Distance(GameObject x, GameObject y)
    {
        float dist = Vector3.Distance(x.transform.position, y.transform.position);
        return dist;
    }
    public bool Check_Blocked(GameObject x, GameObject y)//x是玩家，y是目标
    {
        bool blocked = false;
        if (Find_Distance(x,y) < 17.5)
        {
            return blocked;
        }
        float lastDistance = 9999f;
        int myHeight = height;
        Hex closestOne = null;
        int[] change_hang = { 0, 1, 1, 0, -1, -1 };
        int[] change_lie = { 1, 1, 0, -1 , 0, 1,
                       1, 0, -1, -1, - 1, 0 };//分奇偶层，当前坐标的“六个可能的下个坐标”
        int hang = x.GetComponent<Unit>().hang, lie = x.GetComponent<Unit>().lie;
        while (true)
        {
            lastDistance = 9999f;
            for (int i = 0; i < 6; i++)
            {
                Hex hex;
                if (hang % 2 == 0)
                {
                    hex = map.transform.Find("Map" + (hang + change_hang[i]) + "_" + (lie + change_lie[i])).transform.GetComponent<Hex>();
                }
                else
                {
                    hex = map.transform.Find("Map" + (hang + change_hang[i]) + "_" + (lie + change_lie[i+6])).transform.GetComponent<Hex>();
                }
                float distance = Find_Distance(hex.gameObject, y);
                if (distance <= lastDistance)
                {
                    lastDistance = distance;
                    closestOne = hex;
                }
            }
            hang = closestOne.hang;
            lie = closestOne.lie;
            if (closestOne.height == height && closestOne.blockVision)
            {
                blocked = true;//如果相同的话，那就看当前格子是不是可以遮挡视线的（树林）
                break;
            }
            if (closestOne.height > height)
            {
                blocked = true;//如果遇到了高地，那就是说明不能打
                break;
            }
            if (lastDistance <= 1)
            {
                break;
            }
        }
        return blocked;
    }
    public void CheckStatus()
    {
        Hex hex = map.transform.Find("Map" + thisUnit.hang + "_" + thisUnit.lie).GetComponent<Hex>();
        if (hex.isSpotted <= 0)
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
        int j = 0;
        for (int i = 0; i <= map.transform.childCount - 1; i++)
        {
            try
            {
                NextTile = map.transform.GetChild(i).GetComponent<Hex>();
                if (Find_Distance(gameObject, NextTile.gameObject) <= 17.8 * (dolls.dolls_view_range + rangeBuff))
                {
                    if (!Check_Blocked(gameObject, NextTile.gameObject))
                    {
                        NextTile.isInFog += 1;
                        ToCancelFog.Enqueue(NextTile);
                        j += 1;
                    }
                }
                NextTile.ChangeTheFog();
            }
            catch
            {
                continue;
            }
        }
    }
    public void DeFogOfWar()
    {
        while (ToCancelFog.Count != 0)
        {
            Hex hex = ToCancelFog.Dequeue();
            hex.isInFog -= 1;
            hex.ChangeTheFog();
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
        catch { }
    }
    IEnumerator Reload()
    {
        ReloadStart.Play();
        yield return new WaitForSeconds(dolls.dolls_reload - 2.5f);
        ReloadEnd.Play();
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
}
