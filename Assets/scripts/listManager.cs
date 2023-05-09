using System.IO;
using QuickType;
using UnityEngine;

public class listManager : MonoBehaviour
{
    public static TextAsset jsonFile;
    public GameObject listobjprefab;
    public GameObject scrollviewContent;
    public int index;

    public Lessons lessons;

    private string lsnJson;

    // Start is called before the first frame update
    private void Start()
    {
        //jsonFile = Resources.Load<TextAsset>("Assets/questions/lessons.json");
        lsnJson = File.ReadAllText("Assets/questions/lessons.json");
        lessons = Lessons.FromJson(lsnJson);
        foreach (var lesson in lessons.LessonsLessons)
        {
            var createdLessonItem = Instantiate(listobjprefab);
            createdLessonItem.GetComponent<LessonEntryData>().title.text = lesson.Name;
            createdLessonItem.GetComponent<LessonEntryData>().count.text = lesson.Questions.Count.ToString();
            createdLessonItem.GetComponent<LessonEntryData>().index = index;
            createdLessonItem.transform.SetParent(scrollviewContent.transform);
            createdLessonItem.transform.localScale = Vector3.one;
            index++;
        }
    }
}