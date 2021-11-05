using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    public EnemyProperty enemy;
    public float health,dodge,rangeBuff;
    public Slider healthBar;
    float percentageHealth;
    public Gradient FullHealthNoHealth;
    public int hang;
    public int lie;
    public GameObject map;
    public GameObject dollsList;
    DollsCombat dolls = new DollsCombat(), setDolls;
    bool canFire = true;
    public Hex targetHex;
    public GameObject toHideTheEnemy;
    public int height = 1;
    Vector3 destination;

    public UnitEntity[] enemyEntities;
    public int crewNum;
    public float[] healthRestrictLine = { 0, 0, 0, 0, 0, 0 };
    public int healthLevel;
    bool firstTime;

    public int shotsInMag;
    public GameObject bullet;
    BulletManager shot = new BulletManager();
    bool isFiring = false;
    public bool canMove;
    int counter;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ScanMap", 1f, 1f);
        InvokeRepeating("DeScanMap", 1.5f, 1f);
        enemy = transform.GetComponent<EnemyProperty>();
        health = enemy.enemy_max_hp;
        dodge = enemy.enemy_dodge;
        shotsInMag = enemy.enemy_mag;
        if (canMove)
            Invoke("Move", Random.Range(1f, 30f));
        ScanMap();
        healthLevel = crewNum - 1;
        destination = transform.position;
        for (int i = 0; i <= crewNum; i++)
        {
            healthRestrictLine[i] = health * ((float)i / (float)crewNum);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveToDestination();
        if (gameObject.activeSelf)
        {
            UpdateHealthBar();
            FogOfWar();
            GroundCheckDolls();
        }
    }
    void Move()
    {
        float lastDistance = 9999f;
        Hex closestOne = map.transform.Find("Map" + hang + "_" + lie).transform.GetComponent<Hex>(); ;
        int[] change_hang = { 0, 1, 1, 0, -1, -1 };
        int[] change_lie = { 1, 1, 0, -1 , 0, 1,
                       1, 0, -1, -1, - 1, 0 };//分奇偶层，当前坐标的“六个可能的下个坐标”
        for (int i = 0; i < 6; i++)
        {
            Hex nextHex;
            if (hang % 2 == 0)
            {
                nextHex = map.transform.Find("Map" + (hang + change_hang[i]) + "_" + (lie + change_lie[i])).transform.GetComponent<Hex>();
            }
            else
            {
                nextHex = map.transform.Find("Map" + (hang + change_hang[i]) + "_" + (lie + change_lie[i + 6])).transform.GetComponent<Hex>();
            }
            float distance = Find_Distance(nextHex.gameObject, targetHex.gameObject);
            if (distance <= lastDistance && !nextHex.haveEnemy && !nextHex.haveUnit && nextHex.reachable)
            {
                lastDistance = distance;
                closestOne = nextHex;
            }
        }
        map.transform.Find("Map" + hang + "_" + lie).transform.GetComponent<Hex>().haveEnemy = false;
        if (gameObject.activeSelf)
        {
            DeScanMap();
            closestOne.haveEnemy = true;
            destination = closestOne.transform.position;
            firstTime = false;
            hang = closestOne.hang; lie = closestOne.lie; height = closestOne.height;
            dodge = enemy.enemy_dodge + closestOne.dodgeBuff;
            rangeBuff = closestOne.rangeBuff;
            Invoke("Move", enemy.enemy_moveTime + Random.Range(-6f, 10f));
        }
            
    }
    void MoveToDestination()
    {
        Vector3 direction = destination - transform.position;
        Vector3 velocity = direction.normalized * 2;
        velocity = Vector3.ClampMagnitude(velocity, direction.magnitude);
        transform.Translate(velocity);
        if (direction.magnitude < 1 && firstTime)
        {
            firstTime = false;
            ScanMap();
        }
    }
    public void GroundCheckDolls()
    {
        try
        {
            for (int i = 0; i <= dollsList.transform.childCount - 1; i++)
            {
                dolls = dollsList.transform.GetChild(i).GetComponent<DollsCombat>();
                if (bullet != null && Find_Distance(transform.gameObject, dolls.gameObject) <= 17.32 * (enemy.enemy_range + rangeBuff) && dolls.dolls.dolls_type != 3)
                {
                    if (canFire && dolls.beingSpotted == true && dolls.gameObject.activeSelf)
                    {
                        counter = 0;
                        setDolls = dolls;
                        for (int j = 0; j < crewNum; j++)
                            Invoke("Attack", Random.Range(0f, 0.5f));
                        StartCoroutine(FireRate());
                    }
                }
            }
        }
        catch
        {
        }
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
    public void Attack ()
    {
        enemy.enemy_visible = true;
        toHideTheEnemy.SetActive(true);
        StartCoroutine(SetInActiveAfterFire());
        GameObject bulletThatWasShot = Instantiate(bullet, enemyEntities[counter].transform.position, Quaternion.identity);
        bulletThatWasShot.SetActive(true);
        bulletThatWasShot.transform.LookAt(setDolls.transform);
        shot = bulletThatWasShot.GetComponent<BulletManager>();
        shot.speed = enemy.enemy_shell_speed;
        shot.WhereTheShotWillGo = setDolls.transform.position;
        shot.damage = 0;//先设为0
        shot.damageIndicate = "hit";
        float randomPen = enemy.enemy_penetration + Random.Range(-1.5f, 1.5f);
        if (randomPen >= setDolls.dolls.dolls_armor_front)
        {
            shot.damage = (enemy.enemy_sts_attack * enemy.enemy_damage_multiplier) * Random.Range(0.95f, 1.05f);
            //判定，如果可以击穿那就把伤害加上去，不然的话这发炮弹就是0伤害
            shot.damageIndicate = shot.damage.ToString("F0");
            if (Random.Range(0, 100) < setDolls.dolls.dolls_dodge + setDolls.dodgeBuff - enemy.enemy_accuracy)
            {
                shot.damage = 0;
                //判定，被闪避了那就miss
                shot.damageIndicate = "miss";
            }
        }
        shot.sender = enemyEntities[counter].gameObject;
        shot.dollsList = dollsList;
        shot.whoShotMe = "enemy";
        shot.firstImpact = true;
        counter++;
    }
    IEnumerator SetInActiveAfterFire()
    {
        yield return new WaitForSeconds(1f);
        isFiring = true;
        yield return new WaitForSeconds(2.5f);
        try {
            isFiring = false;
        } catch { }
        
    }
    void WithDrawl()
    {
        map.GetComponent<MapCreate>().Score.killedEnemy += 1;
        map.transform.Find("Map" + hang + "_" + lie).GetComponent<Hex>().haveEnemy = false;
        DeScanMap();
        transform.gameObject.SetActive(false);
        transform.GetComponent<EnemyCombat>().enabled = false;
        Destroy(gameObject);
    }
    void UpdateHealthBar()
    {
        percentageHealth = health / enemy.enemy_max_hp;
        if (health < healthRestrictLine[healthLevel])
        {
            health = healthRestrictLine[healthLevel];
            enemyEntities[healthLevel].gameObject.SetActive(false);
            crewNum -= 1;
            healthLevel -= 1;
        }
        healthBar.value = Mathf.Lerp(healthBar.value, percentageHealth, 20f * Time.deltaTime);
        healthBar.fillRect.GetComponent<Image>().color = FullHealthNoHealth.Evaluate(percentageHealth);
        if (health <= 0)
        {
            map.transform.Find("Map" + hang + "_" + lie).GetComponent<Hex>().haveEnemy = false;
            WithDrawl();
        }
    }
    void FogOfWar()
    {
        Hex hex = map.transform.Find("Map" + hang + "_" + lie).GetComponent<Hex>();
        if (hex.isInFog <= 0 && isFiring == false)
        {
            enemy.enemy_visible = false;
            toHideTheEnemy.SetActive(false);
        }
        else
        {
            enemy.enemy_visible = true  ;
            toHideTheEnemy.SetActive(true);
        }
    }
    public float Find_Distance(GameObject x, GameObject y)
    {
        float dist = Vector3.Distance(x.transform.position, y.transform.position);
        return dist;
    }
    void DeScanMap()
    {
        Hex NextTile;
        for (int i = 0; i <= map.transform.childCount - 1; i++)
        {
            try
            {
                NextTile = map.transform.GetChild(i).GetComponent<Hex>();
                if (Find_Distance(gameObject, NextTile.gameObject) <= 17.5 * enemy.enemy_range)
                {
                    if (!BeingBlocked(gameObject, NextTile.gameObject))
                    {
                        NextTile.isSpotted -= 1;
                    }
                }
            }
            catch
            {
                continue;
            }
        }
    }
    void ScanMap()
    {
        Hex NextTile;
        for (int i = 0; i <= map.transform.childCount - 1; i++)
        {
            try
            {
                NextTile = map.transform.GetChild(i).GetComponent<Hex>();
                if (Find_Distance(gameObject, NextTile.gameObject) <= 17.5 * enemy.enemy_range)
                {
                    if (!BeingBlocked(gameObject, NextTile.gameObject))
                    {
                        NextTile.isSpotted += 1;
                    }
                }
            }
            catch
            {
                continue;
            }
        }
    }
    bool BeingBlocked(GameObject x, GameObject y)
    {
        bool blocked = false;
        if (Find_Distance(x, y) < 17.5)
        {
            return blocked;
        }
        float lastDistance = 9999f;
        int myHeight = height;
        Hex closestOne = null;
        int[] change_hang = { 0, 1, 1, 0, -1, -1 };
        int[] change_lie = { 1, 1, 0, -1 , 0, 1,
                       1, 0, -1, -1, - 1, 0 };//分奇偶层，当前坐标的“六个可能的下个坐标”
        int newHang = hang, newLie = lie;
        while (true)
        {
            lastDistance = 9999f;
            for (int i = 0; i < 6; i++)
            {
                Hex hex;
                if (newHang % 2 == 0)
                {
                    hex = map.transform.Find("Map" + (newHang + change_hang[i]) + "_" + (newLie + change_lie[i])).transform.GetComponent<Hex>();
                }
                else
                {
                    hex = map.transform.Find("Map" + (newHang + change_hang[i]) + "_" + (newLie + change_lie[i + 6])).transform.GetComponent<Hex>();
                }
                float distance = Find_Distance(hex.gameObject, y);
                if (distance <= lastDistance)
                {
                    lastDistance = distance;
                    closestOne = hex;
                }
            }
            newHang = closestOne.hang;
            newLie = closestOne.lie;
            if (closestOne.height == height && closestOne.blockVision)
            {
                blocked = true;//如果相同的话，那就看当前格子是不是可以遮挡视线的（树林）
                break;
            }
            else if (closestOne.height > height)
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
    
}
