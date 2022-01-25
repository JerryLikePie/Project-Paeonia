using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquardPageManager : MonoBehaviour
{
    // SquardPage.MainPanel.ScrollView.Viewport.Content
    public GameObject contentPane;

    // SquardManager.ItemPool
    public GameObject originItemPool;

    // UI 按钮模板
    public GameObject btnTemplate;

    // 临时数组
    private List<GameObject> itemList;

    // Start is called before the first frame update
    void Start()
    {
        itemList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // invoke when open the squard page
    public void initContentList()
    {
        for (int i = 0; i < originItemPool.transform.childCount; i++)
        {
            GameObject doll = originItemPool.transform.GetChild(i).gameObject;
            GameObject btnDollClone = Instantiate(btnTemplate, contentPane.transform, false);
            btnDollClone.GetComponent<Image>().sprite = doll.GetComponent<SpriteRenderer>().sprite;
            btnDollClone.SetActive(true);
            itemList.Add(btnDollClone);
        }
    }

    // invoke when close the squard page
    public void destroyContentList()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Destroy(itemList[i]);
        }
    }
}
