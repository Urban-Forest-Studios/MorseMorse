using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuickType;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private int i;

    public bool isGrid;
    public bool audioPlaying = false;
    public int questionCount;
    public Text questiontext;
    public Text answertext;
    public Text countText;
    private Lessons lessons;
    public List<QuestionElement> questions;
    public List<QuestionElement> answeredQuestions;
    public GameObject completeScreen;
    public GameObject correct;
    public GameObject incorrect;
    public GameObject grid;
    public GameObject type;
    private QuestionElement currentQuestion;
    public GameObject btn;
    public List<GameObject> btns;
    private List<QuestionElement> gridQuestions = new();
    private string lsnJson;
    public Color transparent;
    public Color defaultcolor;


    private readonly Dictionary<char, string> morse = new()
    {
        { 'A', ".-" },
        { 'B', "-..." },
        { 'C', "-.-." },
        { 'D', "-.." },
        { 'E', "." },
        { 'F', "..-." },
        { 'G', "--." },
        { 'H', "...." },
        { 'I', ".." },
        { 'J', ".---" },
        { 'K', "-.-" },
        { 'L', ".-.." },
        { 'M', "--" },
        { 'N', "-." },
        { 'O', "---" },
        { 'P', ".--." },
        { 'Q', "--.-" },
        { 'R', ".-." },
        { 'S', "..." },
        { 'T', "-" },
        { 'U', "..-" },
        { 'V', "...-" },
        { 'W', ".--" },
        { 'X', "-..-" },
        { 'Y', "-.--" },
        { 'Z', "--.." },
        { '0', "-----" },
        { '1', ".----" },
        { '2', "..---" },
        { '3', "...--" },
        { '4', "....-" },
        { '5', "....." },
        { '6', "-...." },
        { '7', "--..." },
        { '8', "---.." },
        { '9', "----." },
        { ' ', "/" },
        { ',', "--..--" },
        { '?', "..--.." },
        { ':', "---..." },
        { '-', "-....-" },
        { '\"', ".-..-." },
        { '(', "-.--." },
        { '=', "-...-" },
        { '.', ".-.-.-" },
        { ';', "-..-." },
        { '\'', ".----." },
        { ')', "-.--.-" },
        { '+', ".-.-." },
        { '@', ".--.-." },
        { '!', "---." }
    };

    private void Start()
    {
        i = GM.activeLessonIndex;
        lsnJson = File.ReadAllText("Assets/questions/lessons.json");
        lessons = Lessons.FromJson(lsnJson);
        questions = lessons.LessonsLessons[i].Questions;

        questionCount = questions.Count;
        countText.text = questionCount.ToString();
        answeredQuestions = new List<QuestionElement>();
        if (lessons.LessonsLessons[i].Type == "grid")
        {
            isGrid = true;
            grid.SetActive(true);
            type.SetActive(false);
            GetGridQuestions();
        }
        else
        {
            isGrid = false;
            grid.SetActive(false);
            type.SetActive(true);
            GetRandomQuestion();
        }

        questiontext.text = currentQuestion.String;
        answertext.text = "";
    }


    private void GetRandomQuestion()
    {
        var randomQuestionIndex = Random.Range(0, questions.Count);
        while (answeredQuestions.Contains(questions[randomQuestionIndex]))
        {
            randomQuestionIndex = Random.Range(0, questions.Count);
        }
        currentQuestion = questions[randomQuestionIndex];
        PlayMorseCode(ToMorse(currentQuestion.String));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void GetGridQuestions()
    {
        /*lsnJson = File.ReadAllText("Assets/questions/lessons.json");
        lessons = Lessons.FromJson(lsnJson);
        questions = lessons.LessonsLessons[i].Questions;*/
        int randomQuestionIndex = Random.Range(0, questions.Count);
        var index = 0;
        currentQuestion = questions[randomQuestionIndex];
        if (answeredQuestions.Contains(currentQuestion))
        {
            while (answeredQuestions.Contains(currentQuestion))
            {
                currentQuestion = questions[randomQuestionIndex];
            }
        }
        gridQuestions.Add(currentQuestion);
        while (gridQuestions.Count < 4)
        {
            randomQuestionIndex = Random.Range(0, questions.Count);
            var candidate = questions[randomQuestionIndex];
            if (!gridQuestions.Contains(candidate) && !currentQuestion.Equals(candidate))
            {
                gridQuestions.Add(candidate);
            }

        }
        gridQuestions.Shuffle();

        index = 0;
        var correctAnswerIndex = gridQuestions.IndexOf(currentQuestion);
        foreach (var createdButtonItem in btns)
        {
            createdButtonItem.GetComponent<ButtonEntryData>().choice.text = ToMorse(gridQuestions[index].String);
            if (index == correctAnswerIndex)
            {
                createdButtonItem.GetComponent<ButtonEntryData>().answer = true;
            }
            else
            {
                createdButtonItem.GetComponent<ButtonEntryData>().answer = false;
            }
            index++;
        }
        PlayMorseCode(ToMorse(currentQuestion.String));
        gridQuestions.Clear();

    }

    public void AddDot()
    {
        answertext.text += ".";
    }

    public void AddDash()
    {
        answertext.text += "-";
    }

    public void AddSpace()
    {
        answertext.text += " ";
    }

    public void Backspace()
    {
        if (answertext.text.Length > 0) answertext.text = answertext.text.Remove(answertext.text.Length - 1);
    }

    public void Correct()
    {
        gridQuestions.Clear();
        correct.SetActive(false);
        foreach (var b in GameObject.FindGameObjectsWithTag("dynamicButton"))
        {
            b.GetComponent<Image>().color = transparent;
        }

        questionCount--;
        countText.text = questionCount.ToString();
        if (questionCount != 0)
        {
            correct.SetActive(true);
            answeredQuestions.Add(currentQuestion);
            answertext.text = "";
            StartCoroutine(NextQuestion(1));
        }
        else if (questionCount <= 0)
        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

    public void Incorrect()
    {

        gridQuestions.Clear();
        incorrect.SetActive(false);
        foreach (var b in GameObject.FindGameObjectsWithTag("dynamicButton"))
        {
            b.GetComponent<Image>().color = transparent;
        }

        countText.text = questionCount.ToString();
        incorrect.SetActive(true);
        answertext.text = "";
        StartCoroutine(NextQuestion(1));
    }

    public void Submit()
    {
        incorrect.SetActive(false);
        correct.SetActive(false);
        var answer = ToMorse(currentQuestion.String);
        answertext.text += " ";
        if (answertext.text == answer)
        {
            Correct();
        }
        else
        {
            Incorrect();
        }

        Backspace();
    }

    private IEnumerator NextQuestion(float time)
    {
        yield return new WaitForSeconds(time);
        gridQuestions.Clear();
        if (isGrid) GetGridQuestions();
        else GetRandomQuestion();
        questiontext.text = currentQuestion.String;
    }

    public string ToMorse(string input)
    {
        var output = "";
        input = input.ToUpper();
        var x = input.ToCharArray();
        for (var i = 0; i < x.Length; i++)
        {
            output += morse[x[i]];
            output += " ";
        }

        return output;
    }

    public void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    //MORSE CODE PLAYER
    public float dotFrequency = 550.0f; // Frequency of a dot in Hz
    public float dashFrequency = 550.0f; // Frequency of a dash in Hz
    public float sampleRate = 44100.0f; // Audio sample rate in Hz
    public float dotDuration = 0.1f; // Duration of a dot in seconds
    public float dashDuration = 0.3f; // Duration of a dash in seconds
    public float silenceDuration = 0.1f; // Duration of the silence between dots and dashes in seconds
    public float cspaceSilenceDuration = 0.3f; // Duration of the silence between characters in seconds
    public AudioSource source;

    public void PlayMorseCode(string morseCode)
    {
        StartCoroutine(PlayMorseCodeCoroutine(morseCode));
    }

    IEnumerator PlayMorseCodeCoroutine(string morseCode)
    {
        audioPlaying = true;
        int numSamplesDot = Mathf.RoundToInt(dotDuration * sampleRate);
        int numSamplesDash = Mathf.RoundToInt(dashDuration * sampleRate);
        int numSamplesSilence = Mathf.RoundToInt(silenceDuration * sampleRate);


        foreach (char c in morseCode)
        {
            if (c == '.')
            {
                PlayAudio(numSamplesDot);
                yield return new WaitForSeconds(dotDuration);
            }
            else if (c == '-')
            {
                PlayAudio(numSamplesDash);
                yield return new WaitForSeconds(dashDuration);
            }
            else if (c == ' ')
            {
                yield return new WaitForSeconds(cspaceSilenceDuration);
            }
            else if (c == '/')
            {
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(silenceDuration);
        }

        audioPlaying = false;
    }

    void PlayAudio(int smpl)
    {
        int sampleFreq = 44000;
        float frequency = 550;

        float[] samples = new float[smpl];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = Mathf.Sin(Mathf.PI * 2 * i * frequency / sampleFreq);
        }

        AudioClip ac = AudioClip.Create("Test", samples.Length, 1, sampleFreq, false);
        ac.SetData(samples, 0);
        gameObject.GetComponent<AudioSource>().clip = ac;
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void RepeatAudioPrompt()
    {
        if(!audioPlaying) PlayMorseCode(ToMorse(currentQuestion.String));
    }
}

public static class Extensions
{
    private static System.Random rand = new System.Random();
 
    public static void Shuffle<T>(this IList<T> values)
    {
        for (int i = values.Count - 1; i > 0; i--) {
            int k = rand.Next(i + 1);
            (values[k], values[i]) = (values[i], values[k]);
        }
    }
}