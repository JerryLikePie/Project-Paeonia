using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSpawnManager: MonoBehaviour,IDragHandler,IEndDragHandler,IBeginDragHandler
{
    MouseManager mouseManager;
    Hex tiletoSpawn;
    Unit unit;
    //GameObject spawnedUnit;
    Transform parent;
    public Transform Anchor;
    public bool canSpawn = false;
    public bool isDragging = false;
    Unit1Skill Skill;
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(Anchor);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 parentPosition = new Vector2(transform.parent.localPosition.x, transform.parent.localPosition.y);
        //将屏幕拖拽适配到所有屏幕上面去的万能代码，请刻入DNA：
        //transform.localPosition = new Vector2(eventData.position.x * (1920f / Screen.width) * ((float)Screen.width / Screen.height / (1920f / 1080f)) - Screen.width / 2f, eventData.position.y * (1080f / Screen.height) - Screen.height / 2f);
        transform.localPosition = new Vector2((eventData.position.x - Screen.width / 2f) * (1920f / Screen.width) * ((float)Screen.width / Screen.height / (1920f / 1080f)), (eventData.position.y - Screen.height / 2f) * (1080f / Screen.height));
        mouseManager.isDraggingUI = true;
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parent);
        isDragging = false;
        mouseManager.isDraggingUI = false;
        transform.localPosition = Vector3.zero;
        if (Input.mousePosition.x < (Screen.width * 0.25f) && Input.mousePosition.y < 170)
        {
            Debug.Log("在限制区域内："+Input.mousePosition);
        }
        else
        {
            Debug.Log("不在：" + Input.mousePosition);
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
                        Skill.whereToSkill = tiletoSpawn.transform;
                        Skill.Can_Spawn();
                        parent.GetComponent<ClickAndMove>().isUp = false;
                        parent.GetComponent<ClickAndMove>().TimeToGoDown = true;
                        StartCoroutine(WaitForTwoSec());
                    }
                }
            }
        }
    }
    IEnumerator WaitForTwoSec()
    {
        yield return new WaitForSeconds(2);
        Skill.CannotSpawn();
    }
    // Start is called before the first frame update
    void Start()
    {
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        parent = transform.parent;
        Skill = transform.GetComponent<Unit1Skill>();
    }
}
