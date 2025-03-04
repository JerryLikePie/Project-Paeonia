using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryPlay : MonoBehaviour
{
    /*
     * Story Play
     * 剧情演出的相关函数
     */
    [SerializeField] private Canvas storyPanel;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource storyMusic;
    public GameObject logPanel;
    public Text logText;
    public ClickAndMove loadingPage;
    [SerializeField] private Image character, background;
    [SerializeField] private Sprite defaultBackGround;
    private Sprite lastSprite;
    [SerializeField] private Text textBox, nameBox;
    [SerializeField] private Button[] responses;
    [SerializeField] private Text[] answers;
    private string endingStage;
    private string alsoFinish;
    [SerializeField] int choice;
    [SerializeField] int index, length;
    [SerializeField] bool isDragging, haveToChoose, isInLogMode, isClickingUI, textIsGoing, imageUpdating;
    private Vector3 mouseDownPos1, mouseDownPos2;
    Storyscene scene;
    float textDelay = 0.015f;

    private void Start() {
        Debug.Log("Starting Story Panel..");
        if (scene == null)
        {
            Debug.Log("There is no story..");
            unloadStory();
        }
        haveToChoose = false;
    }

    public void loadStory(string storyName) {
        // 根据输入的名称，加载一个剧情
        Debug.Log(storyName);
        if (!string.IsNullOrEmpty(storyName))
        {
            Debug.Log("Enabling Story Panel..");
            storyPanel.gameObject.SetActive(true);
            storyPanel.enabled = true;
            scene = (Storyscene)Resources.Load("Stories/" + storyName);
            Debug.Log("已加载");
            length = scene.story.Length;
            index = 0;
            choice = 0;
            haveToChoose = false;
            character.color = new Color(1, 1, 1, 0);
            endingStage = scene.endingStage;
            // if there is a stage to skip along with this scene
            // skip it.
            alsoFinish = scene.alsoFinish;
            if (alsoFinish != null)
            {
                PlayerPrefs.SetInt(alsoFinish, 4);
                alsoFinish = null;
            }
            changeBGM(true);
            updateStory();
        }
    }

    void changeBGM(bool goingIntoStory)
    {
        if (goingIntoStory)
        {
            backgroundMusic.Stop();
            backgroundMusic.volume = 0f;
            storyMusic.volume = 1f;
        }
        else
        {
            backgroundMusic.volume = 1f;
            backgroundMusic.Play();
            storyMusic.volume = 0f;
            storyMusic.Stop();
            storyMusic.clip = null;
        }
    }

    public void skipStory()
    {
        isClickingUI = true;
        unloadStory();
    }

    public void openLog()
    {
        isInLogMode = true;
        logPanel.GetComponent<ClickAndMove>().Going_Up();
    }

    public void closeLog()
    {
        isInLogMode = false;
        logPanel.GetComponent<ClickAndMove>().Going_Down();
        isClickingUI = true;
    }

    public void setNewTextDelay(float num)
    {
        textDelay = num;
    }

    void record(int type, string name, string sentence)
    {
        // 剧情log，相当于录进历史记录。
        switch (type)
        {
            case 1: //人说的话
                logText.text = logText.text + name + ": " + "「" + sentence + "」\n";
                break;
            case 2: //旁白
                logText.text = logText.text + sentence + "\n";
                break;
            case 3: //你的选择
                logText.text = logText.text + "【" + sentence + "】\n";
                break;
            default:
                break;
        }
        
    }

    void unloadStory()
    {
        // 结束后初始化一切设定
        Debug.Log("Unloading Story");
        logText.text = " ";
        
        length = 0;
        index = 0;
        choice = 0;
        lastSprite = null;
        isDragging = false;
        haveToChoose = false;
        isInLogMode = false;
        isClickingUI = false;
        textIsGoing = false;
        imageUpdating = false;
        character.color = new Color(1, 1, 1, 0);
        scene = null;
        // If there is a stage to be loaded, load the stage.
        // the stage is linked directly using a button. So onClick does the job.
        if (endingStage != null)
        {
            // there is a stage
            if (PlayerPrefs.GetInt(endingStage, 0) == 0)
            {
                // the stage has not been done
                loadingPage.Going_Up();
                PlayerPrefs.SetString("Stage_You_Should_Load", endingStage);
                endingStage = null;
                StartCoroutine(DelayEnterGame());
            }
            else
            {
                changeBGM(false);
                StartCoroutine(DelayEnd());
            }
            endingStage = null;
        } 
        else
        {
            changeBGM(false);
            StartCoroutine(DelayEnd());
        }
        
        
    }

    void updateStory() {
        // 翻页相关
        if (scene == null)
        {
            Debug.Log("There is no story loaded, stopping update.");
            return;
        }
        if (scene.story[index].isChoice) {
            haveToChoose = true;
        }
        //updateButton();
        if (choice != 0 && scene.story[index].branchNum == 0) {
            choice = 0;
        }
        if (choice != scene.story[index].branchNum) {
            
            index++;
            updateStory();
        } else {
            haveToChoose = scene.story[index].isChoice;
            if (scene.story[index].backgroundMusic != null)
            {
                storyMusic.clip = scene.story[index].backgroundMusic;
                if (!storyMusic.isPlaying)
                {
                    storyMusic.volume = 1.0f;
                    storyMusic.Play();
                }
            }
            if (scene.story[index].startMute)
            {
                StartCoroutine(musicFadeOut(storyMusic));
            }
            if (lastSprite != scene.story[index].background)
            {
                imageUpdating = true;
            }
            if (scene.story[index].background == null)
            {
                if (imageUpdating)
                {
                    StartCoroutine(backgroundFadeChange(background, defaultBackGround));
                } else
                {
                    background.sprite = defaultBackGround;
                }
            } else
            {
                if (imageUpdating)
                {
                    StartCoroutine(backgroundFadeChange(background, scene.story[index].background));
                }
                else
                {
                    background.sprite = scene.story[index].background;
                }
            }
            lastSprite = scene.story[index].background;
            if (scene.story[index].character == null)
            {
                StartCoroutine(characterImageChange(true, character));
            } else
            {
                character.sprite = scene.story[index].character;
                StartCoroutine(characterImageChange(false, character));
            }
            if (string.IsNullOrEmpty(scene.story[index].name))
            {
                nameBox.text = " ";
                record(2, " ", scene.story[index].words);
            } else
            {
                nameBox.text = scene.story[index].name;
                record(1, scene.story[index].name, scene.story[index].words);
            }
            StartCoroutine(showText(scene.story[index].words, textBox));
            index++;
        }
    }

    public void respondWith(int num)
    {
        isClickingUI = true;
        choice = num;
        haveToChoose = false;
        record(3, " ", answers[num - 1].text);
        for (int j = 0; j < 3; j++)
        {
            responses[j].gameObject.SetActive(false);
        }
        updateStory();
    }

    void updateButton()
    {
        if (scene != null)
        { 
            for (int j = 0; j < 3; j++)
            {
                if ( j < scene.story[index - 1].response.Length)
                {
                    if (!string.IsNullOrEmpty(scene.story[index - 1].response[j]))
                    {
                        responses[j].gameObject.SetActive(true);
                        answers[j].text = scene.story[index - 1].response[j];
                    }
                    else
                    {
                        responses[j].gameObject.SetActive(false);
                    }
                }
                else
                {
                    responses[j].gameObject.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            isDragging = false;
            mouseDownPos1 = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (Vector3.Distance(mouseDownPos1, Input.mousePosition) > 10f)
            {
                isDragging = true;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            if (!isDragging && !haveToChoose && !isInLogMode && !isClickingUI && !textIsGoing && !imageUpdating)
            {
                if (index >= length)
                {
                    unloadStory();
                }
                updateStory();
            }
            if (isClickingUI)
            {
                isClickingUI = false;
            }
           
        }
    }

    IEnumerator showText(string fullText, Text output)
    {
        // 一个一个字输出到文本框
        string currentText;
        textIsGoing = true;
        for (int k = 0; k <= fullText.Length; k++)
        {
            currentText = fullText.Substring(0, k);
            output.text = currentText;
            yield return new WaitForSeconds(textDelay);
        }
        for (int k = 0; k <= 10; k++)
        {
            yield return new WaitForSeconds(textDelay);
        }
        updateButton();
        textIsGoing = false;
    }

    IEnumerator backgroundFadeChange(Image background, Sprite changeTo)
    {
        // 更改剧情背景图像
        Debug.Log("变化中");
        for (float i = 1; i >= 0; i -= 4f * Time.deltaTime)
        {
            // set color with i as alpha
            background.color = new Color(i, i, i, 1);
            yield return null;
        }
        background.color = new Color(0, 0, 0, 1);
        background.sprite = changeTo;
        for (float i = 0; i <= 1; i += 4f * Time.deltaTime)
        {
            // set color with i as alpha
            background.color = new Color(i, i, i, 1);
            yield return null;
        }
        background.color = new Color(1, 1, 1, 1);
        imageUpdating = false;
    }

    IEnumerator musicFadeOut(AudioSource music)
    {
        // 音乐关闭
        for (float i = 1; i >= 0; i -= 0.5f * Time.deltaTime)
        {
            // set color with i as alpha
            music.volume = i;
            yield return null;
        }
        music.Stop();
        
    }

    IEnumerator characterImageChange(bool fadeout, Image character)
    {
        // 更改人物图像
        if (fadeout)
        {
            for (float i = character.color.a; i >= 0; i -= 0.1f)
            {
                // set color with i as alpha
                character.color = new Color(1, 1, 1, i);
                yield return new WaitForSeconds(0.02f);
            }
            character.color = new Color(1, 1, 1, 0);
        }
        else // else it would be fading in
        {
            for (float i = character.color.a; i <= 1; i += 0.1f)
            {
                // set color with i as alpha
                character.color = new Color(1, 1, 1, i);
                yield return new WaitForSeconds(0.02f);
            }
            character.color = new Color(1, 1, 1, 1);
        }
    }

    IEnumerator DelayEnterGame()
    {
        yield return new WaitForSeconds(0.5f); // Wait 1 seconds

        SceneManager.LoadScene("Game1");
    }

    IEnumerator DelayEnd()
    {
        yield return new WaitForSeconds(0.5f); // Wait 1 seconds
        loadingPage.Going_Down();
        storyPanel.enabled = false;
        storyPanel.gameObject.SetActive(false);
    }
}

