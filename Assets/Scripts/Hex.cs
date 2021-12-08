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
    public int gameUtility;//��Ϸ����ʩ��1���㣬2��㣬3ռ���
    public bool haveUnit = false;
    public bool haveEnemy = false;
    public bool endGame = false;
    public int height;
    public bool blockVision;
    public int isInFog = 0;//0Ϊ��������
    public int isSpotted = 0;//0Ϊû�б������������ҷ���˵
    public float dodgeBuff,rangeBuff;
    public GameObject fogOfWarDarken;
    public GameObject hexBase;
    public bool render;

    Color originalColor;
    Color maskedColor;
    Color highlightColor;
    void Start()
    {
        if (fogOfWarDarken != null)
        {
            originalColor = fogOfWarDarken.GetComponent<Renderer>().material.color;
            maskedColor = new Color(originalColor.r - 0.5f, originalColor.g - 0.5f, originalColor.b - 0.5f);
            highlightColor = new Color(originalColor.r + 0.1f, originalColor.g + 0.5f, originalColor.b + 0.1f);
        }
        UpdateFogStatus();
    }
    void Update()
    {
        //changeTheFog();
    }
    public void GainVisual()
    {
        if (!render)
        {
            render = true;
        }
        this.isInFog++;
    }
    public void LoseVisual()
    {
        this.isInFog--;
    }
    public void EnemySpot()
    {
        this.isSpotted++;
    }
    public void EnemyLoseVisual()
    {
        this.isSpotted--;
    }
    public void UpdateFogStatus()
    {
        if (fogOfWarDarken != null)
        {
            if (isInFog == 0)//0������
            {
                fogOfWarDarken.GetComponent<Renderer>().material.color = maskedColor;
            }
            if (isInFog >= 1)//1��û����
            {
                fogOfWarDarken.GetComponent<Renderer>().material.color = originalColor;
            }
            if (isInFog == 9999)//9999�Ǹ߹�
            {
                fogOfWarDarken.GetComponent<Renderer>().material.color = highlightColor;
            }
            if (isInFog < 0) isInFog = 0;
            if (isSpotted < 0) isSpotted = 0;
        }
 
        if (render)
        {
            
            if (hexBase != null)
            {
                hexBase.GetComponent<MeshRenderer>().enabled = true;
            }
            if (fogOfWarDarken != null)
            {
                fogOfWarDarken.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        else
        {
            
            if (hexBase != null)
            {
                hexBase.GetComponent<MeshRenderer>().enabled = false;

            }
            if (fogOfWarDarken != null)
            {
                fogOfWarDarken.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
