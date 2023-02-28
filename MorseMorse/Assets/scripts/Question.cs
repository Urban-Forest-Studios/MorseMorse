using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Question
{
    public QuestionTypes type = new QuestionTypes();
    public AudioClip audio;
    public string question;
    public string[] answers;
    public string answer;
}
public enum QuestionTypes
{
    GridSelect,
    Listen,
    Read
};
