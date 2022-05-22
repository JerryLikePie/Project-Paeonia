using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class DragSpawnItem: MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    MouseManager mouseManager;
    DragSpawnManager spawnManager;
    Hex tiletoSpawn;
    Transform parent;
    public Transform Anchor;
    public bool isDragging = false;
    public Vector3 snapBack;
    public GameObject spawn;


    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(Anchor);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 parentPosition = new Vector2(transform.parent.localPosition.x, transform.parent.localPosition.y);
        //将屏幕拖拽适配到所有屏幕上面去的万能代码，请刻入DNA：
        transform.localPosition = new Vector2((eventData.position.x - Screen.width / 2f) * (1920f / Screen.width) * ((float)Screen.width / Screen.height / (1920f / 1080f)), (eventData.position.y - Screen.height / 2f) * (1080f / Screen.height));
        mouseManager.isDraggingUI = true;
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parent);
        isDragging = false;
        mouseManager.isDraggingUI = false;
        transform.localPosition = snapBack;
        if (!(Input.mousePosition.x < (Screen.width * 0.5f) && Input.mousePosition.y < 170))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] rayinfo;
            rayinfo = Physics.RaycastAll(ray);
            if (rayinfo != null)
            {
                for (int i = rayinfo.Length - 1; i >= 0; i--)
                {
                    GameObject targetObject = rayinfo[i].collider.transform.parent.gameObject;
                    if (targetObject.GetComponent<Hex>() != null)
                    {
                        tiletoSpawn = targetObject.GetComponent<Hex>();
                        if (!tiletoSpawn.haveUnit)
                        {
                            spawnDoll(tiletoSpawn);
                        }
                        Debug.Log(tiletoSpawn.name);
                        
                    }
                }
            }
        }
    }
    void spawnDoll(Hex tiletoSpawn)
    {
        if (spawn != null)
        {
            GameObject spawnedUnit = Instantiate(spawn, tiletoSpawn.transform.position, Quaternion.identity);
            Debug.Log("在" + tiletoSpawn.name + "生成了" + spawn.name);
            spawnedUnit.GetComponent<Unit>().hang = tiletoSpawn.X;
            spawnedUnit.GetComponent<Unit>().lie = tiletoSpawn.Z;
            spawnedUnit.GetComponent<DollsCombat>().allEnemy = spawnManager.enemyList;
            spawnedUnit.GetComponent<DollsCombat>().allDolls = spawnManager.playerList;
            spawnedUnit.GetComponent<DollsCombat>().thisUnit = spawnedUnit.GetComponent<Unit>();
            spawnedUnit.GetComponent<DollsCombat>().map = spawnManager.map;

            //spawnedUnit.GetComponent<DollsCombat>().FogOfWar();
            //tiletoSpawn.haveUnit = true;
            //spawnedUnit.transform.parent = spawnManager.playerList.transform;
            //spawnManager.Skills[slots[i].spawnID].transform.SetParent(spawnManager.SkillSlot[tempCounter].transform);
            //tempCounter++;
            //spawnManager.Skills[slots[i].spawnID].transform.localPosition = Vector3.zero;
            //IDollsSkillBehavior skill1 = spawnManager.Skills[slots[i].spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>();
            //skill1.unit = spawnedUnit.GetComponent<DollsCombat>();
            ////Skills[slots[i].spawnID].transform.GetChild(0).GetComponentInChildren<IDollsSkillBehavior>().mapList = this.gameObject;
            //skill1.loadMap();
            //if (skill1.secondSkill != null)
            //{
            //    skill1.secondSkill.unit = spawnedUnit.GetComponent<DollsCombat>();
            //    skill1.secondSkill.loadMap();
            //    if (skill1.secondSkill.secondSkill != null)
            //    {
            //        skill1.secondSkill.secondSkill.unit = spawnedUnit.GetComponent<DollsCombat>();
            //        skill1.secondSkill.secondSkill.loadMap();
            //    }
            //}
            //spawnedUnit.GetComponent<DollsCombat>().CheckStatus();
        }
    }
    void Start()
    {
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        spawnManager = GameObject.Find("DragSpawnManager").GetComponent<DragSpawnManager>();
        parent = transform.parent;
    }

    
}
