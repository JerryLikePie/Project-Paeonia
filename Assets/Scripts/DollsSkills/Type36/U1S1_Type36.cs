using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U1S1_Type36 : IDollsSkillBehavior
{
    [SerializeField]
    AudioSource skillSound;
    [SerializeField]
    GameObject flare;
    private Transform location;//这个技能在哪里用

    public override void activateSkill(Transform location)
    {
        this.location = location;
        inCoolDown = true;
        timeStart = System.DateTime.Now.Ticks;
        skillSound.Play();
        StartCoroutine(flareLaunch());
    }

    IEnumerator flareLaunch()
    {
        yield return new WaitForSeconds(1);
        GameObject newFlare = Instantiate(flare, location.position, Quaternion.identity);
        Queue<Hex> temp = new Queue<Hex>();//一个暂时的数组
        for (int i = 0; i < allTiles.Length; i++)
        {
            if (Vector3.Distance(newFlare.transform.position, allTiles[i].transform.position) <= 17.32 * 3)
            {
                allTiles[i].isInFog += 1;
                allTiles[i].UpdateFogStatus();
                temp.Enqueue(allTiles[i]);
            }
        }
        yield return new WaitForSeconds(17);
        while(temp.Count != 0)
        {
            Hex hex = temp.Dequeue();
            hex.isInFog -= 1;
            hex.UpdateFogStatus();
        }
        Destroy(newFlare);
    }

    void Start()
    {
        cooldown.transform.localScale = new Vector3(0, 1.05f, 1f);
        showTime.SetActive(false);
    }

    void Update()
    {
        if (unit != null && unit.health <= 0)
        {
            cooldown.transform.localScale = new Vector3(1.05f, 1.05f, 1f);
            inCoolDown = true;
        }
        else
        {
            CoolDownPanel();
        }
    }
}
