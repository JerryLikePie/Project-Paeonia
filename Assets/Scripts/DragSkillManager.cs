using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSkillManager: MonoBehaviour,IDragHandler,IEndDragHandler,IBeginDragHandler
{
    MouseManager mouseManager;
    Hex tiletoSpawn;
    Transform parent;
    public Transform Anchor;
    public bool canSpawn = false;
    public bool isDragging = false;
    public IDollsSkillBehavior skill;

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
        try { skill.unit.thisUnit.skillBox.SetActive(true); } catch { }//有些dolls是没有技能范围的
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parent);
        isDragging = false;
        mouseManager.isDraggingUI = false;
        try { skill.unit.thisUnit.skillBox.SetActive(false); } catch { }//有些dolls是没有技能范围的
        transform.localPosition = Vector3.zero;
        if (!(Input.mousePosition.x < (Screen.width * 0.25f) && Input.mousePosition.y < 170))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] rayinfo;
            rayinfo = Physics.RaycastAll(ray);
            if (rayinfo != null)
            {
                for (int i = rayinfo.Length - 1; i >= 0; i--)
                {
                    GameObject targetObject = rayinfo[i].collider.transform.parent.gameObject;
                    if (targetObject.GetComponent<Hex>() != null && !skill.inCoolDown)
                    {
                        tiletoSpawn = targetObject.GetComponent<Hex>();
                        if (Vector3.Distance(tiletoSpawn.transform.position, skill.unit.transform.position) < 17.5 * skill.range)
                        {
                            skill.unit.combatBehaviour.firstTime = true;
                            skill.activateSkill(tiletoSpawn.transform);
                            skill.inCoolDown = true;
                            skill.showTime.SetActive(true);
                        }
                    }
                    parent.GetComponent<ClickAndMove>().isUp = false;
                    parent.GetComponent<ClickAndMove>().TimeToGoDown = true;
                }
            }
        }
    }
    IEnumerator WaitForTwoSec()
    {
        yield return new WaitForSeconds(2);
    }
    void Start()
    {
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        parent = transform.parent;
    }
}
