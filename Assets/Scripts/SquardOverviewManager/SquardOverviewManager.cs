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

    // UI ��ťģ��
    public GameObject btnTemplate;

    // ������������� Image �ؼ�
    public GameObject sdp_DetailImage;
    // ��������������ת��ť
    public GameObject sdp_BtnPrev;
    public GameObject sdp_BtnNext;
    // �������Ľ�ɫ����
    public GameObject sdp_TxtName;

    // ���治����ʱ��Ĭ������
    public Sprite detailImageFallback;

    // ���浱ǰ��ɫ��ť����ʱ����
    private List<GameObject> itemList;

    // ���浱ǰ��ť���±꣨��ʵҲ������ itemList.IndexOf ʵ�֣���������һ�㣩
    private Dictionary<GameObject, int> btnIndexes;

    // Doll ��Ϣ�б�
    private List<DollDetailInfo> detailInfoList;

    /// <summary>
    /// ��ǰ����ҳ������
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
            // ͨ�� template �����µİ�ť����ɫͼƬ��
            GameObject dollInfo = originItemPool.transform.GetChild(i).gameObject;
            GameObject btnDollClone = Instantiate(btnTemplate, contentPane.transform, false);

            // ��ȡ������������Ϣ
            DollDetailInfo detailInfo = dollInfo.GetComponent<DollDetailInfo>();
            detailInfoList.Add(detailInfo);

            // ���ð�ť����Ϊ��Ӧ��ɫ
            btnDollClone.GetComponent<Image>().sprite = detailInfo.dollOverviewImage;
            btnDollClone.SetActive(true);

            // ��¼��ť���
            btnIndexes.Add(btnDollClone, i);
            
            // ���ð�ť�ص�����
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


    // BtnPrev �� BtnNext ��ҳ��ť�Ļص�����
    public void onPageChangeButtonClicked(int delta)
    {
        // ����ť�ص�������֧��ö�����Ͳ�����
        // ���� Unity devs��������� 14 ���������forum��������ˣ�������û��Ҫ�ӵ���˼
        // ��ˣ���ȷ�� delta ֵ����ȷ���ã�ͨ��Ӧ��Ϊ
        // -1 : ��ǰ��ҳ
        // 1  : ���ҳ
        currentPageIndex += delta;
        updateDetailPageInfo();
    }

    // �������������ʾ������
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
