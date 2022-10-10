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

    // 绑定的地图文件名
    [SerializeField] private string mapFile;
    // 后续其他需要在地图中初始化的属性也可以写在这里，然后通过一个 struct/class 传给 MapMangaer

    public void ChangeDescription()
    {
        thisName.text = myName;
        areaName.text = areaIn;
        areaDescription.text = "活性化："+livePercent+"%\n制空权："+airDom+"% ";
        PlayerPrefs.SetString("Stage_You_Should_Load", mapFile);
    }
}
