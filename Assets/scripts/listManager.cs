using System.Collections;
using System.Collections.Generic;
using System.IO;
using QuickType;
using UnityEngine;

public class listManager : MonoBehaviour
{
    public GameObject listobjprefab;
    public static TextAsset jsonFile;
    public GameObject scrollviewContent;
    public int index = 0;

    public Lessons lessons;

    private string lsnJson;
    // Start is called before the first frame update
    void Start()
    {
        //jsonFile = Resources.Load<TextAsset>("Assets/questions/lessons.json");
        lsnJson = File.ReadAllText("Assets/questions/lessons.json");
        lessons = Lessons.FromJson(lsnJson);
        foreach (var lesson in lessons.LessonsLessons)
        {
            GameObject createdLessonItem = Instantiate(listobjprefab);
            createdLessonItem.GetComponent<LessonEntryData>().title.text = lesson.Name;
            createdLessonItem.GetComponent<LessonEntryData>().count.text = lesson.Questions.Count.ToString();
            createdLessonItem.GetComponent<LessonEntryData>().index = index;
            createdLessonItem.transform.SetParent(scrollviewContent.transform);
            createdLessonItem.transform.localScale = Vector3.one;
            index++;
        }
    }
    
    
    
}

