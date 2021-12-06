using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utilities;

public class EnemyCombat : MonoBehaviour
{
    public EnemyProperty enemy;
    public float health, dodge, rangeBuff;
    public Slider healthBar;
    float percentageHealth;
    Gradient healthGradient;
    public int hang;
    public int lie;
    public GameObject map;
    public GameObject dollsList;
    DollsCombat dolls = new DollsCombat(), setDolls;
    bool canFire = true;
    public Queue<Hex> targetHex;
    public Queue<int> moveWaitTime;
    public Queue<Hex> deScanTheMap;
    int nextWaitTime;
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
    public int moveSpeedWaitTime;
    Transform supportTargetCord;
    public Transform artyTarget;

    // Start is called before the first frame update
    void Start()
    {
        healthGradient = new Gradient();
        //InvokeRepeating("ScanMap", 1f, 1f);
        //Invoke("DeScanMap", 1f);
        //deScanTheMap;
        SetHealthGradient(healthGradient);
        enemy = transform.GetComponent<EnemyProperty>();
        health = enemy.enemy_max_hp;
        dodge = enemy.enemy_dodge;
        shotsInMag = enemy.enemy_mag;
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
        if (canMove)
        {
            UpdateRoute();
            if (destination != transform.position)
            {
                MoveToDestination();
            }
            else if (firstTime)
            {
                //DeScanMap();
                firstTime = false;
                scanMap();
            }

        }
        if (gameObject.activeSelf)
        {
            UpdateHealthBar();
            FogOfWar();
            switch(enemy.enemy_type)
            {
                case 1:
                    GroundCheckDolls();
                    break;
                case 2:
                    SupportCheckDolls();
                    break;
            }
            
        }
    }

    const int RSTATE_NODE = 0;
    const int RSTATE_MOVE = 1;
    const int RSTATE_WAIT = 2;
    const int RSTATE_END  = 3;
    int routeState = RSTATE_NODE;
    float timeWaitStart;
    float timeToWait;
    Hex nextTarget = null;

    void UpdateRoute()
    {
        if (routeState == RSTATE_NODE)
        {
            // ·�����
            if (targetHex.Count > 0)
            {
                nextTarget = targetHex.Dequeue();
                routeState = RSTATE_MOVE;
            }
            else
            {
                routeState = RSTATE_END;
            }
        }
        else if (routeState == RSTATE_MOVE) {
            // ������һ��Ҫȥ�ĸ���
            // �ȴ� moveSpeedWaitTime
            float minDistance = 9999f;
            Hex currentHex = map.transform.Find("Map" + hang + "_" + lie).transform.GetComponent<Hex>();

            int[] change_hang = { 0, 1, 1, 0, -1, -1 };
            int[] change_lie = { 1, 1, 0, -1 , 0, 1,
                       1, 0, -1, -1, - 1, 0 };//����ż�㣬��ǰ����ġ��������ܵ��¸����ꡱ
            for (int i = 0; i < 6; i++)
            {
                Hex nextHex;
                // try catch ��ֹ��ͼ�±�Խ��
                try
                {
                    if (hang % 2 == 0)
                    {
                        nextHex = map.transform.Find("Map" + (hang + change_hang[i]) + "_" + (lie + change_lie[i])).transform.GetComponent<Hex>();
                    }
                    else
                    {
                        nextHex = map.transform.Find("Map" + (hang + change_hang[i]) + "_" + (lie + change_lie[i + 6])).transform.GetComponent<Hex>();
                    }
                    float distance = FindDistance(nextHex.gameObject, nextTarget.gameObject);
                    if (distance <= minDistance && !nextHex.haveEnemy && !nextHex.haveUnit && nextHex.reachable)
                    {
                        minDistance = distance;
                        currentHex = nextHex;
                    }
                }
                catch { }
            }
            // �����ƶ���ȥ���޸�destination
            if (gameObject.activeSelf)
            {
                descanMap();
                //Debug.Log(this + "(" + hang + "," + lie + ")->(" + currentHex.hang + "," + currentHex.lie + ")\tTarget:" + nextTarget);
                map.transform.Find("Map" + hang + "_" + lie).transform.GetComponent<Hex>().haveEnemy = false;
                
                destination = currentHex.transform.position;
                // transform.position = currentHex.transform.position;
                currentHex.haveEnemy = true;
                firstTime = true;
                hang = currentHex.hang; lie = currentHex.lie; height = currentHex.height;
                dodge = enemy.enemy_dodge + currentHex.dodgeBuff;
                rangeBuff = currentHex.rangeBuff;
                
                // �ȴ��ƶ�����������ǵ���·�����ϣ���ͣ��·���滮��ʱ��
                float randomMoveTime = (moveSpeedWaitTime) * currentHex.movecost + Random.Range(-2f, 5f);
                timeToWait = (currentHex == nextTarget) ? moveWaitTime.Dequeue() : randomMoveTime;
                timeWaitStart = Time.time;
                routeState = RSTATE_WAIT;   
            }
        }
        else if (routeState == RSTATE_WAIT)
        {
            if (Time.time - timeWaitStart >= timeToWait)
            {
                // ��ʱĬ�϶����Ѿ���λ���Ҿ�ֹ
                Hex currentHex = map.transform.Find("Map" + hang + "_" + lie).transform.GetComponent<Hex>();
                // ����·���ڵ㣬����·����·��
                // δ��������ƶ�
                routeState = (currentHex == nextTarget) ? RSTATE_NODE : RSTATE_MOVE;
            }
        }
    }
    void MoveToDestination()
    {
        Vector3 direction = destination - transform.position;
        Vector3 velocity = direction.normalized * 0.5f;
        velocity = Vector3.ClampMagnitude(velocity, direction.magnitude);
        transform.Translate(velocity);
    }
    public void Attack ()
    {
        switch (enemy.enemy_type)
        {
            case 1:
                fireBullet();
                break;
            case 2:
                fireHowitzer();
                break;
        }
    }
    void fireBullet()
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
        shot.damage = 0;//����Ϊ0
        shot.damageIndicate = "hit";
        float randomPen = enemy.enemy_penetration + Random.Range(-1.5f, 1.5f);
        if (randomPen >= setDolls.dolls.dolls_armor_front)
        {
            shot.damage = (enemy.enemy_sts_attack * enemy.enemy_damage_multiplier) * Random.Range(0.95f, 1.05f);
            //�ж���������Ի����ǾͰ��˺�����ȥ����Ȼ�Ļ��ⷢ�ڵ�����0�˺�
            shot.damageIndicate = shot.damage.ToString("F0");
            if (Random.Range(0, 100) < setDolls.dolls.dolls_dodge + setDolls.dodgeBuff - enemy.enemy_accuracy)
            {
                shot.damage = 0;
                //�ж������������Ǿ�miss
                shot.damageIndicate = "miss";
            }
        }
        shot.sender = enemyEntities[counter].gameObject;
        shot.dollsList = dollsList;
        shot.whoShotMe = "enemy";
        shot.firstImpact = true;
        counter++;
    }
    void fireHowitzer()
    {
        Debug.Log("Fire");
        GameObject bulletThatWasShot = Instantiate(bullet, enemyEntities[counter].transform.position, Quaternion.identity);
        bulletThatWasShot.SetActive(true);
        bulletThatWasShot.transform.LookAt(supportTargetCord);
        shot = bulletThatWasShot.GetComponent<BulletManager>();
        shot.speed = enemy.enemy_shell_speed;
        shot.WhereTheShotWillGo = supportTargetCord.position;
        shot.shotType = enemy.enemy_ammo_type;
        shot.damage = (enemy.enemy_sts_attack * enemy.enemy_damage_multiplier) * Random.Range(0.9f, 1.1f);
        shot.damageIndicate = shot.damage.ToString("F0");
        float randomPen = enemy.enemy_penetration + Random.Range(-10f, 10f);
        shot.penetration = randomPen;
        shot.sender = enemyEntities[counter].gameObject;
        counter++;
        shot.dollsList = dollsList;
        shot.whoShotMe = "enemy";
        shot.firstImpact = true;
    }
    public void GroundCheckDolls()
    {
        try
        {
            for (int i = 0; i <= dollsList.transform.childCount - 1; i++)
            {
                dolls = dollsList.transform.GetChild(i).GetComponent<DollsCombat>();
                if (bullet != null && FindDistance(transform.gameObject, dolls.gameObject) <= 17.5 * (enemy.enemy_range + rangeBuff) && dolls.dolls.dolls_type != 3)
                {
                    if (canFire && dolls.beingSpotted == true && dolls.gameObject.activeSelf)
                    {
                        counter = 0;
                        setDolls = dolls;
                        for (int j = 0; j < crewNum; j++)
                            Invoke("Attack", Random.Range(0f, 0.49f));
                        StartCoroutine(FireRate());
                    }
                }
            }
        }
        catch
        {
        }
    }
    public void SupportCheckDolls()
    {
        try
        {
            if (canFire)
            {
                GameObject maybeTarget = GameObject.FindWithTag("ArtilleryReference");
                supportTargetCord = maybeTarget.transform;
                counter = 0;
                for (int j = 0; j < crewNum; j++)
                    Invoke("Attack", Random.Range(0f, 3f));
                StartCoroutine(FireRate());
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
        descanMap();
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
        healthBar.fillRect.GetComponent<Image>().color = healthGradient.Evaluate(percentageHealth);
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
    void descanMap()
    {
        while (deScanTheMap.Count != 0)
        {
            deScanTheMap.Peek().EnemyLoseVisual();
            deScanTheMap.Dequeue().UpdateFogStatus();
        }
    }
    void scanMap()
    {
        Hex NextTile;
        for (int i = 0; i <= map.transform.childCount - 1; i++)
        {
            try
            {
                NextTile = map.transform.GetChild(i).GetComponent<Hex>();
                if (FindDistance(gameObject, NextTile.gameObject) <= 17.5 * (enemy.enemy_range + rangeBuff))
                {
                    if (!BeingBlocked(gameObject, NextTile.gameObject))
                    {
                        NextTile.isSpotted += 1;
                        deScanTheMap.Enqueue(NextTile);
                    }
                }
            }
            catch
            {
                continue;
            }
        }
        //Debug.Log("һ��֮�󣬶�������" + deScanTheMap.Count + "���ؿ�");
    }
    bool BeingBlocked(GameObject x, GameObject y)
    {
        bool blocked = false;
        if (FindDistance(x, y) < 17.5)
        {
            return blocked;
        }
        else
        {
            try
            {
                if (y.GetComponent<Hex>().blockVision)
                {
                    return true;
                }
            }
            catch
            {
                // do nothing
            }
        }

        float lastDistance = 9999f;
        Hex closestOne = null;
        int[] change_hang = { 0, 1, 1, 0, -1, -1 };
        int[] change_lie = { 1, 1, 0, -1 , 0, 1,
                       1, 0, -1, -1, - 1, 0 };//����ż�㣬��ǰ����ġ��������ܵ��¸����ꡱ
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
                float distance = FindDistance(hex.gameObject, y);
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
                blocked = true;//�����ͬ�Ļ����ǾͿ���ǰ�����ǲ��ǿ����ڵ����ߵģ����֣�
                break;
            }
            else if (closestOne.height > height)
            {
                blocked = true;//��������˸ߵأ��Ǿ���˵�����ܴ�
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
