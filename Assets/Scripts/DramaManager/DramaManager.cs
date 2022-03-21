using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DramaManager : MonoBehaviour
{

    // unity wired
    public GameObject imgBackground;
    public GameObject imgCharactorLeft;
    public GameObject imgCharactorRight;
    public GameObject txtMain;
    public GameObject txtName;

    public DramaData currentDrama;

    private DramaState myState = DramaState.WAIT_FOR_INIT;

    public enum DramaState
    {
        WAIT_FOR_INIT = 0,
        IDLE,
        PLAYING,
        FINISHED
    }

    // Use this for initialization
    void Start()
    {
        loadDrama("DramaRes/temp.drama");
        startDrama();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void loadDrama(string dramaName)
    {
        TextAsset text = (TextAsset) Resources.Load(dramaName);
        string[] lines = text.text.Split('\r', '\n');
        DramaData data = DramaParser.parseDramaText(lines);
        Debug.Log(data);
        currentDrama = data;
        myState = DramaState.IDLE;
    }

    // 记录当前对话位置
    private int currentLineIndex;
    private int currentSectionIndex;
    void startDrama()
    {
        if (myState == DramaState.WAIT_FOR_INIT) {
            Debug.LogError("No drama data loaded!");
            return; 
        }
        // -1 表示首次执行，需要必要的初始化
        currentLineIndex = -1;
        currentSectionIndex = -1;
        myState = DramaState.PLAYING;
    }
    
    public void nextStep()
    {
        if (myState == DramaState.PLAYING)
        {
            if (currentLineIndex == -1)
            {
                // -1 表示首次执行，需要必要的初始化
                // 判断 section 是否结束，是则结束对话
                currentSectionIndex++;
                if (currentSectionIndex >= currentDrama.sections.Count)
                {
                    Debug.Log("剧情播放完毕!~");
                    myState = DramaState.FINISHED;
                    SceneManager.LoadScene("Menu");
                    return;
                }
                // 开始新 section
                DramaSectionData currentSection = currentDrama.sections[currentSectionIndex];
                setBackground(currentSection.backgroundImage);
                // todo 目前仅支持两人；1人、3+人待实现
                setCharactorLeft(currentSection.charactors[0].chImage);
                setCharactorRight(currentSection.charactors[1].chImage);
                currentLineIndex = 0;
            }

            DramaConversationData currentLine = currentDrama.sections[currentSectionIndex].conversations[currentLineIndex];
            if (currentLine.indexWho == 0)
            {
                enableCharactor(imgCharactorLeft);
                disableCharactor(imgCharactorRight);
            }
            else
            {
                enableCharactor(imgCharactorRight);
                disableCharactor(imgCharactorLeft);
            }
            string currentChName = currentDrama.sections[currentSectionIndex].charactors[currentLine.indexWho].chName;
            setNameText(currentChName);
            setConversationText(currentLine.richtext);
            currentLineIndex++;

            // 对话结束
            if (currentLineIndex >= currentDrama.sections[currentSectionIndex].conversations.Count)
            {
                currentLineIndex = -1;
            }
        }
    }

    
    #region ========= 内部便利方法 ========= 

    private void setBackground(Sprite bg)
    {
        imgBackground.GetComponent<Image>().sprite = bg;
    }

    private void setCharactorLeft(Sprite chL)
    {
        imgCharactorLeft.GetComponent<Image>().sprite = chL;
    }

    private void setCharactorRight(Sprite chR)
    {
        imgCharactorRight.GetComponent<Image>().sprite = chR;
    }

    private void enableCharactor(GameObject ch) 
    {
        ch.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }
    private void disableCharactor(GameObject ch)
    {
        ch.GetComponent<Image>().color = new Color(.3f, .3f, .3f, .9f);
    }

    private void setNameText(string text)
    {
        txtName.GetComponent<Text>().text = text;
    }

    private void setConversationText(string text)
    {
        txtMain.GetComponent<Text>().text = text;
    }
    #endregion
}