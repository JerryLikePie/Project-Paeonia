using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public int X;
    public int Z;
    public bool reachable = false;
    public float movecost;
    public int spawnNum;
    public int gameUtility;//游戏内设施，1蓝点，2红点，3占领点
    public bool haveUnit = false;
    public bool haveEnemy = false;
    public bool endGame = false;
    public int height;
    public bool blockVision;
    public int isInFog = 0;//0为在迷雾中
    public int isSpotted = 0;//0为没有被点亮（对于我方来说
    public float dodgeBuff,rangeBuff;
    public GameObject fogOfWarDarken;
    public MeshRenderer[] NeedsHidden;
    public bool render = true;
    bool checkupdate = false;

    Color originalColor;
    Color maskedColor;
    Color highlightColor;
    void Start()
    {
        if (fogOfWarDarken != null)
        {
            if (height >= 0)
            {
                originalColor = fogOfWarDarken.GetComponent<Renderer>().material.color;
                maskedColor = new Color(originalColor.r - 0.5f, originalColor.g - 0.5f, originalColor.b - 0.5f);
                highlightColor = new Color(originalColor.r + 0.1f, originalColor.g + 0.5f, originalColor.b + 0.1f);
            }
            
        }
        checkupdate = true;
        UpdateFogStatus();
    }
    void Update()
    {
        UpdateFogStatus();
    }
    public void GainVisual()
    {
        checkupdate = true;
        if (!render)
        {
            render = true;
        }
        this.isInFog++;
    }
    public void LoseVisual()
    {
        checkupdate = true;
        this.isInFog--;
    }
    public void EnemySpot()
    {
        checkupdate = true;
        this.isSpotted++;
    }
    public void EnemyLoseVisual()
    {
        checkupdate = true;
        this.isSpotted--;
    }
    public void UpdateFogStatus()
    {
        if (!checkupdate)
        {
            return;
        }
        if (fogOfWarDarken != null)
        {
            if (isInFog == 0)//0是有雾
            {
                fogOfWarDarken.GetComponent<Renderer>().material.color = maskedColor;
            }
            if (isInFog >= 1)//1是没有雾
            {
                fogOfWarDarken.GetComponent<Renderer>().material.color = originalColor;
            }
            if (isInFog == 9999)//9999是高光
            {
                fogOfWarDarken.GetComponent<Renderer>().material.color = highlightColor;
            }
            if (isInFog < 0) isInFog = 0;
            if (isSpotted < 0) isSpotted = 0;
        }

        if (render)
        {
            foreach (MeshRenderer item in NeedsHidden)
            {
                item.enabled = true;
            }
            if (fogOfWarDarken != null)
            {
                fogOfWarDarken.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        else
        {
            foreach (MeshRenderer item in NeedsHidden)
            {
                item.enabled = false;
            }
            if (fogOfWarDarken != null)
            {
                fogOfWarDarken.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        checkupdate = false;
    }
}
