using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


public class SquadSelectionPage : MonoBehaviour
{
    [SerializeField] private GameObject spaceAnchor;
    [SerializeField] private GameObject unitAnchor;
    [SerializeField] private DollsPoolManager poolManager;
    [SerializeField] public Queue<Transform> tempBinInUse = new Queue<Transform>();
    [SerializeField] private bool[] slotLoaded;
    [SerializeField] private Button[] slots;
    int slotNum = 0;

    [System.Serializable]
    public class SquadSave
    {
        public int[] dollsID;
    }

    public SquadSave squadSave;

    private void Start()
    {
        LoadSquad();
    }

    public void LoadDolls()
    {
        DiscardDolls();
        int j = 0;
        int i = 1;
        if (slotLoaded[slotNum])
            i = 0;
        int howMany = unitAnchor.transform.childCount;
        while (unitAnchor.transform.childCount > i)
        {
            Transform temp;
            temp = unitAnchor.transform.GetChild(i);
            temp.SetParent(spaceAnchor.transform.GetChild(j));
            temp.localPosition = Vector3.zero;
            tempBinInUse.Enqueue(temp);
            if (temp.gameObject.activeSelf)
                j++;
        }
    }

    public void DiscardDolls()
    {
        while (tempBinInUse.Count!= 0)
        {
            Transform temp = tempBinInUse.Dequeue();
            temp.SetParent(unitAnchor.transform);
            temp.localPosition = Vector3.zero;
        }
    }

    public void SetSlotNum(int num)
    {
        slotNum = num;
    }

    public void SelectedDolls(int dollsNum)
    {
        int previous = poolManager.dollsInputted[slotNum];
        poolManager.InputDolls(slotNum, dollsNum);
        if (dollsNum != 0)
        {
            if (slotLoaded[slotNum])
            {
                SwitchActive(previous, true);
            }
            SwitchActive(dollsNum, false);
            slotLoaded[slotNum] = true;
        }
        else
        {
            SwitchActive(previous, true);
            slotLoaded[slotNum] = false;
        }
        slots[slotNum].image.sprite = poolManager.dolls[dollsNum].banner;
    }

    void SwitchActive(int dollsNum, bool state)
    {
        Transform temp = null;
        for (int i = 0; i < spaceAnchor.transform.childCount; i++)
        {
            temp = spaceAnchor.transform.GetChild(i).transform.Find("Doll" + dollsNum);
            if (temp != null)
                temp.gameObject.SetActive(state);
        }
        temp = unitAnchor.transform.Find("Doll" + dollsNum);
        if (temp != null)
            temp.gameObject.SetActive(state);
    }
    
    public void SelectedDone()
    {
        squadSave.dollsID = poolManager.dollsInputted;
        SaveSquad();
    }

    void SaveSquad()
    {
        string datapath = Application.persistentDataPath;

        var serializer = new XmlSerializer(typeof(SquadSave));
        var stream = new FileStream(datapath + "/squadSetUp.save", FileMode.Create);
        serializer.Serialize(stream, squadSave);

        stream.Close();
        Debug.Log("已保存");
    }

    public void LoadSquad()
    {
        string datapath = Application.persistentDataPath;
        
        if (File.Exists(datapath + "/squadSetUp.save"))
        {
            var serializer = new XmlSerializer(typeof(SquadSave));
            var stream = new FileStream(datapath + "/squadSetUp.save", FileMode.Open);

            squadSave = serializer.Deserialize(stream) as SquadSave;
            stream.Close();

            int j = 0;
            foreach(int i in squadSave.dollsID)
            {
                SetSlotNum(j);
                SelectedDolls(i);
                j++;
            }

            Debug.Log("加载完毕");
        }
    }
}
