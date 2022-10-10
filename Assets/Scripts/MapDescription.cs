using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MapDescription : MonoBehaviour
{
    [SerializeField] private string myName;
    [SerializeField] private string areaIn;
    [SerializeField] public int livePercent;
    [SerializeField] public int airDom;

    [SerializeField] private Text thisName;
    [SerializeField] private Text areaName;
    [SerializeField] private Text areaDescription;

    // �󶨵ĵ�ͼ�ļ���
    [SerializeField] private string mapFile;
    // ����������Ҫ�ڵ�ͼ�г�ʼ��������Ҳ����д�����Ȼ��ͨ��һ�� struct/class ���� MapMangaer

    public void ChangeDescription()
    {
        thisName.text = myName;
        areaName.text = areaIn;
        areaDescription.text = "���Ի���"+livePercent+"%\n�ƿ�Ȩ��"+airDom+"% ";
        PlayerPrefs.SetString("Stage_You_Should_Load", mapFile);
    }
}
