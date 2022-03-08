using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DramaData
{
    public DramaData()
    {
        sections = new List<DramaSectionData>();
    }
    public string chaptorName;
    public List<DramaSectionData> sections;
}

public class DramaSectionData
{
    public DramaSectionData()
    {
        charactors = new List<DramaCharactorData>();
        conversations = new List<DramaConversationData>();
    }
    public string sectionName;
    public Sprite backgroundImage;
    public List<DramaCharactorData> charactors;
    public List<DramaConversationData> conversations;
}

public struct DramaCharactorData
{
    public string chName;
    public string chAbbr;
    public Sprite chImage;
}

public struct DramaConversationData
{
    public DramaConversationData(int indexWho, string richtext)
    {
        this.indexWho = indexWho;
        this.richtext = richtext;
    }
    public int indexWho;
    public string richtext;
}