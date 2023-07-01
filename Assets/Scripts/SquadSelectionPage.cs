using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SquadSelectionPage : MonoBehaviour
{
    public bool[] selectedSlot = new bool[8];//���slot�Ƿ��Ѿ�������
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
        {//������Ŀ����Ѿ����˵�ʱ�򣬰�������ƻص�all��������ѡ
            lastSlot.transform.GetChild(0).gameObject.SetActive(true);
            lastSlot.transform.GetChild(0).SetParent(allUnitsAnchor.transform);
            selectedSlot[slotnum] = false;//Ȼ��ѵ�ǰ���۱�Ϊû����
        }
        GameObject slot = GameObject.Find("Slot" + slotnum);//���Ȼ�ȡ��ǰ�����Ƿŵ����ſ���
        GameObject doll = EventSystem.current.currentSelectedGameObject;//Ȼ���ȡ��ǰ����
        int ID = doll.GetComponent<DollsProperty>().dolls_id;//��ȡ��ǰ��������ID
        string UID = doll.GetComponent<DollsProperty>().dolls_name;//��ȡ��ǰ��������string UID
        slot.GetComponent<SquadSlot>().spawnID = ID;//������ID�������۵�����ID����ȥ
        slot.GetComponent<SquadSlot>().spawnUID = UID;//������ID�������۵�����ID����ȥ
        doll.transform.SetParent(slot.transform);//���͵���Ӧ�Ŀ�������
        doll.SetActive(false);//Ȼ�������������ص����ⰴť�غϳ�����
        slot.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Unit" + ID + "Avatar");//�������ͼƬ����ͼƬ����ȥ
        selectedSlot[slotnum] = true;//��ǰ���۱��Ϊ�Ѿ�������
        if (slot.GetComponent<SquadSlot>().spawnID == 0)//��������ո�ѡ����ȡ��զ��
        {
            //lastSlot.transform.GetChild(0).gameObject.SetActive(true);//�Ǿ��ͻ�ȥ��
            lastSlot.transform.GetChild(0).SetParent(allUnitsAnchor.transform);
            selectedSlot[slotnum] = false;
        }
    }
    public void Slot_Clicked()//�����slot
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
                {//�Ȳ鿴���Ƿ���ڣ��Ƿ�������Ƿ���ѡ��
                    biggestNum++;
                    if (slottype == 1)//Ҫ̹��
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
                    else if (slottype == 2)//Ҫ̹�˺ͻ���
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
                    else if (slottype == 3)//Ҫ̹�ˣ����ڣ��ͷɻ�
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
                    else if (slottype == 4)//Ҫ�ɻ�
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