using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Letter", menuName = "Story System/Create New Letter")]
public class Letterscene : ScriptableObject
{
    public string finishStory;
    public Sprite photo;
    public string title;
    [TextArea(1, 20)] public string words;
    public string answer;
    public bool choice;
    public string answer2;
    public int effectEvent1;
    public int effectEvent2;

}