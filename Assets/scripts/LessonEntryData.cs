using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LessonEntryData : MonoBehaviour
{
    public Text title;
    public Text count; 
    public int index;
    
    public void Play()
    {
        GM.activeLessonIndex = index;
        Debug.Log(GM.activeLessonIndex + "in lessonEntryData");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}