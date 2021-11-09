using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public int hang;
    public int lie;
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
    public GameObject FogOfWarDarken;

    Color originalColor;
    Color MaskedColor;
    Color HighlightColor;
    void Start()
    {
        if (FogOfWarDarken != null)
        {
            originalColor = FogOfWarDarken.GetComponent<Renderer>().material.color;
            MaskedColor = new Color(originalColor.r - 0.5f, originalColor.g - 0.5f, originalColor.b - 0.5f);
            HighlightColor = new Color(originalColor.r + 0.1f, originalColor.g + 0.5f, originalColor.b + 0.1f);
        }
        //if (FogOfWarDarken != null)
            //FogOfWarDarken.GetComponent<Renderer>().material.color = MaskedColor;
    }
    void Update()
    {
        ChangeTheFog();
    }
    public void ChangeTheFog()
    {
        if (FogOfWarDarken != null)
        {
            if (isInFog == 0)//0������
            {
                FogOfWarDarken.GetComponent<Renderer>().material.color = MaskedColor;
            }
            if (isInFog >= 1)//1��û����
            {
                FogOfWarDarken.GetComponent<Renderer>().material.color = originalColor;
            }
            if (isInFog == 9999)
            {
                FogOfWarDarken.GetComponent<Renderer>().material.color = HighlightColor;
            }
            if (isInFog < 0) isInFog = 0;
            if (isSpotted < 0) isSpotted = 0;
        }
    }
}
