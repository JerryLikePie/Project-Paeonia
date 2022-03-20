using Assets.Scripts.SquardOverviewManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquardOverviewManager : MonoBehaviour
{
    // SquardPage.MainPanel.ScrollView.Viewport.Content
    public GameObject contentPane;

    // SquardManager.ItemPool
    public GameObject originItemPool;

    // UI 按钮模板
    public GameObject btnTemplate;

    // 详情界面的立绘的 Image 控件
    public GameObject sdp_DetailImage;
    // 详情界面的左右跳转按钮
    public GameObject sdp_BtnPrev;
    public GameObject sdp_BtnNext;
    // 详情界面的角色名称
    public GameObject sdp_TxtName;

    // 立绘不存在时的默认立绘
    public Sprite detailImageFallback;

    // 保存当前角色按钮的临时数组
    private List<GameObject> itemList;

    // 保存当前按钮的下标（其实也可以用 itemList.IndexOf 实现，但这样快一点）
    private Dictionary<GameObject, int> btnIndexes;

    // Doll 信息列表
    private List<DollDetailInfo> detailInfoList;

    /// <summary>
    /// 当前立绘页面的序号
    /// </summary>
    private int currentPageIndex = -1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // invoke when open the squard page
    public void initContentList()
    {
        itemList = new List<GameObject>();
        btnIndexes = new Dictionary<GameObject, int>();
        detailInfoList = new List<DollDetailInfo>();
       
        for (int i = 0; i < originItemPool.transform.childCount; i++)
        {
            // 通过 template 创建新的按钮（角色图片）
            GameObject dollInfo = originItemPool.transform.GetChild(i).gameObject;
            GameObject btnDollClone = Instantiate(btnTemplate, contentPane.transform, false);

            // 获取并保存详情信息
            DollDetailInfo detailInfo = dollInfo.GetComponent<DollDetailInfo>();
            detailInfoList.Add(detailInfo);

            // 设置按钮背景为对应角色
            btnDollClone.GetComponent<Image>().sprite = detailInfo.dollOverviewImage;
            btnDollClone.SetActive(true);

            // 记录按钮编号
            btnIndexes.Add(btnDollClone, i);
            
            // 设置按钮回调函数
            btnDollClone.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                // currentPageIndex =  itemList.IndexOf(btnDollClone);
                currentPageIndex = btnIndexes[btnDollClone];
                updateDetailPageInfo();
            });
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


    // BtnPrev 和 BtnNext 翻页按钮的回调函数
    public void onPageChangeButtonClicked(int delta)
    {
        // “按钮回调函数不支持枚举类型参数”
        // 懒狗 Unity devs，这个问题 14 年就有人在forum上提出来了，但就是没有要加的意思
        // 因此，请确保 delta 值的正确设置，通常应该为
        // -1 : 向前翻页
        // 1  : 向后翻页
        currentPageIndex += delta;
        updateDetailPageInfo();
    }

    // 更新立绘界面显示的内容
    private void updateDetailPageInfo()
    {
        if (currentPageIndex >= 0)
        {
            DollDetailInfo detailInfo = detailInfoList[currentPageIndex];
            sdp_DetailImage.GetComponent<Image>().sprite = detailInfo.dollDetailImage != null ? detailInfo.dollDetailImage : detailImageFallback;
            // sdp_BtnPrev.SetActive(currentPageIndex > 0);
            // sdp_BtnNext.SetActive(currentPageIndex < detailInfoList.Count - 1);
            sdp_TxtName.GetComponent<Text>().text = detailInfo.dollName;
        }
    }
}
