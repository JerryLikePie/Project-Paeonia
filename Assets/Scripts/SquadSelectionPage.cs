using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SquadSelectionPage : MonoBehaviour
{
    public bool[] selectedSlot = new bool[8];//这个slot是否已经有人了
    public GameObject lastSlot, allUnitsAnchor, squadSelection, SpacesAnchor;
    public int slotnum;
    void Start()
    {
        gameObject.SetActive(true);
        slotnum = 0;
        for (int a = 0; a < 7; a++)
        {
            selectedSlot[a] = false;
        }
    }
    public void Selected_Done()
    {
        if (selectedSlot[slotnum] == true)
        {//当点击的卡槽已经有人的时候，把这个人移回到all里面来备选
            lastSlot.transform.GetChild(0).gameObject.SetActive(true);
            lastSlot.transform.GetChild(0).SetParent(allUnitsAnchor.transform);
            selectedSlot[slotnum] = false;//然后把当前卡槽标为没有人
        }
        GameObject slot = GameObject.Find("Slot" + slotnum);//首先获取当前人物是放到几号卡槽
        GameObject doll = EventSystem.current.currentSelectedGameObject;//然后获取当前人物
        int ID = doll.GetComponent<DollsProperty>().dolls_id;//获取当前这个人物的ID
        string UID = doll.GetComponent<DollsProperty>().dolls_name;//获取当前这个人物的string UID
        slot.GetComponent<SquadSlot>().spawnID = ID;//把人物ID给到卡槽的生成ID上面去
        slot.GetComponent<SquadSlot>().spawnUID = UID;//把人物ID给到卡槽的生成ID上面去
        doll.transform.SetParent(slot.transform);//给送到对应的卡槽里面
        doll.SetActive(false);//然后把这个人物隐藏掉避免按钮重合出问题
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Unit" + ID + "Avatar");//把人物的图片给到图片上面去
        selectedSlot[slotnum] = true;//当前卡槽标记为已经有人了
        if (slot.GetComponent<SquadSlot>().spawnID == 0)//但是如果刚刚选的是取消咋办
        {
            //lastSlot.transform.GetChild(0).gameObject.SetActive(true);//那就送回去嘛
            lastSlot.transform.GetChild(0).SetParent(allUnitsAnchor.transform);
            selectedSlot[slotnum] = false;
        }
    }
    public void Slot_Clicked()//点击了slot
    {
        lastSlot = EventSystem.current.currentSelectedGameObject;
        slotnum = lastSlot.GetComponent<SquadSlot>().slotNum;
        int slottype = lastSlot.GetComponent<SquadSlot>().slotType;
        int i = 1;
        int biggestNum = 0;
        for (int n = 1; n <= 25; n++)
        {
            GameObject currentSlot = SpacesAnchor.transform.Find("Space" + i).gameObject;
            try
            {
                GameObject checkDoll;
                if (selectedSlot[slotnum] == true)
                {
                    checkDoll = allUnitsAnchor.transform.Find("Doll" + (n - 1)).gameObject;
                    checkDoll.SetActive(false);
                }
                else
                {
                    checkDoll = allUnitsAnchor.transform.Find("Doll" + n).gameObject;
                    checkDoll.SetActive(false);
                    allUnitsAnchor.transform.Find("Doll0").gameObject.SetActive(false);
                }
                if (checkDoll.GetComponent<DollsProperty>().dolls_unlocked == true)
                {//先查看：是否存在，是否解锁，是否已选择
                    biggestNum++;
                    if (slottype == 1)//要坦克
                    {
                        allUnitsAnchor.GetComponent<RectTransform>().sizeDelta = new Vector2(8 * 200, 400);
                        if (checkDoll.GetComponent<DollsProperty>().dolls_type == 0 || checkDoll.GetComponent<DollsProperty>().dolls_type == 1)
                        {
                            Vector2 parentPosition = new Vector2(checkDoll.transform.parent.localPosition.x, checkDoll.transform.parent.localPosition.y);
                            checkDoll.transform.localPosition = new Vector2(currentSlot.transform.localPosition.x, currentSlot.transform.localPosition.y);
                            i++;
                            checkDoll.SetActive(true);
                        }
                    }
                    else if (slottype == 2)//要坦克和火炮
                    {
                        allUnitsAnchor.GetComponent<RectTransform>().sizeDelta = new Vector2(11 * 200, 400);
                        if (checkDoll.GetComponent<DollsProperty>().dolls_type == 0 || checkDoll.GetComponent<DollsProperty>().dolls_type == 1 || checkDoll.GetComponent<DollsProperty>().dolls_type == 2)
                        {
                            Vector2 parentPosition = new Vector2(checkDoll.transform.parent.localPosition.x, checkDoll.transform.parent.localPosition.y);
                            checkDoll.transform.localPosition = new Vector2(currentSlot.transform.localPosition.x, currentSlot.transform.localPosition.y);
                            i++;
                            checkDoll.SetActive(true);
                        }
                    }
                    else if (slottype == 3)//要坦克，火炮，和飞机
                    {
                        allUnitsAnchor.GetComponent<RectTransform>().sizeDelta = new Vector2(21 * 200, 400);
                        if (checkDoll.GetComponent<DollsProperty>().dolls_type == 0 || checkDoll.GetComponent<DollsProperty>().dolls_type == 1 || checkDoll.GetComponent<DollsProperty>().dolls_type == 2 || checkDoll.GetComponent<DollsProperty>().dolls_type == 3)
                        {
                            Vector2 parentPosition = new Vector2(checkDoll.transform.parent.localPosition.x, checkDoll.transform.parent.localPosition.y);
                            checkDoll.transform.localPosition = new Vector2(currentSlot.transform.localPosition.x, currentSlot.transform.localPosition.y);
                            i++;
                            checkDoll.SetActive(true);
                        }
                    }
                    else if (slottype == 4)//要飞机
                    {
                        allUnitsAnchor.GetComponent<RectTransform>().sizeDelta = new Vector2(10 * 200, 400);
                        if (checkDoll.GetComponent<DollsProperty>().dolls_type == 0 || checkDoll.GetComponent<DollsProperty>().dolls_type == 3)
                        {
                            Vector2 parentPosition = new Vector2(checkDoll.transform.parent.localPosition.x, checkDoll.transform.parent.localPosition.y);
                            checkDoll.transform.localPosition = new Vector2(currentSlot.transform.localPosition.x, currentSlot.transform.localPosition.y);
                            i++;
                            checkDoll.SetActive(true);
                        }
                    }
                }
            }
            catch
            {
                continue;
            }
        }
        i = 1;
    }
}