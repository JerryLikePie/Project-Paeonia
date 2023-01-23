using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class storyNode
{
    public Sprite background;
    public Sprite character;
    public AudioClip backgroundMusic;
    public string name;
    [TextArea] public string words;
    public bool isChoice;
    public int branchNum;
    public string[] response = new string[3];
}

[CreateAssetMenu(fileName = "New Story", menuName = "Story System/Create New Story")]
public class Storyscene : ScriptableObject
{
    public storyNode[] story;
}
