using System.Collections;
using System.Collections.Generic;
using System.IO;
using QuickType;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public bool isGrid;
    public bool audioPlaying;
    public int questionCount;
    public Text questiontext;
    public Text answertext;
    public Text countText;
    public GameObject completeScreen;
    public GameObject correct;
    public GameObject incorrect;
    public GameObject grid;
    public GameObject type;
    public GameObject btn;
    public List<GameObject> btns;
    public Color transparent;
    public Color defaultcolor;


    //MORSE CODE PLAYER
    public float frequency = 550.0f; // Frequency of a dot in Hz
    public float sampleRate = 44100.0f; // Audio sample rate in Hz
    public float dotDuration = 0.1f; // Duration of a dot in seconds
    public float dashDuration = 0.3f; // Duration of a dash in seconds
    public float silenceDuration = 0.1f; // Duration of the silence between dots and dashes in seconds
    public float cspaceSilenceDuration = 0.3f; // Duration of the silence between characters in seconds
    public AudioSource source;


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

    public List<QuestionElement> answeredQuestions;
    private QuestionElement currentQuestion;
    private readonly List<QuestionElement> gridQuestions = new();
    private int i;
    private Lessons lessons;
    private string lsnJson;
    public List<QuestionElement> questions;

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
        var randomQuestionIndex = UnityEngine.Random.Range(0, questions.Count);
        while (answeredQuestions.Contains(questions[randomQuestionIndex]))
            randomQuestionIndex = UnityEngine.Random.Range(0, questions.Count);
        currentQuestion = questions[randomQuestionIndex];
        PlayMorseCode(ToMorse(currentQuestion.String));
    }

    private void GetGridQuestions()
    {
        var randomQuestionIndex = UnityEngine.Random.Range(0, questions.Count);
        var index = 0;
        var tries = 0;
        currentQuestion = questions[randomQuestionIndex];
        if (answeredQuestions.Contains(currentQuestion))
            //OFFENDING LOOP
            while (answeredQuestions.Contains(currentQuestion))
            {
                randomQuestionIndex = UnityEngine.Random.Range(0, questions.Count);
                currentQuestion = questions[randomQuestionIndex];
            }

        gridQuestions.Add(currentQuestion);
        tries = 0;
        while (index < 3)
        {
            randomQuestionIndex = UnityEngine.Random.Range(0, questions.Count);
            var candidate = questions[randomQuestionIndex];
            if (!gridQuestions.Contains(candidate) && !currentQuestion.Equals(candidate))
            {
                gridQuestions.Add(candidate);
                index++;
            }
        }

        gridQuestions.Shuffle();


        index = 0;
        var correctAnswerIndex = gridQuestions.IndexOf(currentQuestion);
        foreach (var createdButtonItem in btns)
        {
            createdButtonItem.GetComponent<ButtonEntryData>().choice.text = ToMorse(gridQuestions[index].String);
            createdButtonItem.GetComponent<ButtonEntryData>().answer = index == correctAnswerIndex;
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
            b.GetComponent<Image>().color = transparent;

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
            b.GetComponent<Image>().color = transparent;

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
            Correct();
        else
            Incorrect();

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

    public void PlayMorseCode(string morseCode)
    {
        StartCoroutine(PlayMorseCodeCoroutine(morseCode));
    }

    private IEnumerator PlayMorseCodeCoroutine(string morseCode)
    {
        audioPlaying = true;
        var numSamplesDot = Mathf.RoundToInt(dotDuration * sampleRate);
        var numSamplesDash = Mathf.RoundToInt(dashDuration * sampleRate);
        var numSamplesSilence = Mathf.RoundToInt(silenceDuration * sampleRate);


        foreach (var c in morseCode)
        {
            switch (c)
            {
                case '.':
                    PlayAudio(numSamplesDot);
                    yield return new WaitForSeconds(dotDuration);
                    break;
                case '-':
                    PlayAudio(numSamplesDash);
                    yield return new WaitForSeconds(dashDuration);
                    break;
                case ' ':
                    yield return new WaitForSeconds(cspaceSilenceDuration);
                    break;
                case '/':
                    yield return new WaitForSeconds(0.5f);
                    break;
            }

            yield return new WaitForSeconds(silenceDuration);
        }

        audioPlaying = false;
    }

    private void PlayAudio(int smpl)
    {
        var sampleFreq = 44000;

        var samples = new float[smpl];
        for (var i = 0; i < samples.Length; i++) samples[i] = Mathf.Sin(Mathf.PI * 2 * i * frequency / sampleFreq);

        var ac = AudioClip.Create("Test", samples.Length, 1, sampleFreq, false);
        ac.SetData(samples, 0);
        gameObject.GetComponent<AudioSource>().clip = ac;
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void RepeatAudioPrompt()
    {
        if (!audioPlaying) PlayMorseCode(ToMorse(currentQuestion.String));
    }
}

public static class Extensions
{
    private static readonly Random rand = new();

    public static void Shuffle<T>(this IList<T> values)
    {
        for (var i = values.Count - 1; i > 0; i--)
        {
            var k = rand.Next(i + 1);
            (values[k], values[i]) = (values[i], values[k]);
        }
    }
}