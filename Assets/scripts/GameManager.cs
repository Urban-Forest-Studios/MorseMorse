using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuickType;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private int i;

    public bool isGrid;
    public int questionCount;
    public Text questiontext;
    public Text answertext;
    public Text countText;
    private Lessons lessons;
    public List<QuestionElement> questions;
    public List<QuestionElement> _unansweredQuestions;
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
        { ' ', " " },
        { ',', "--..--" },
        { '?', "..--.." },
        { ':', "---..." },
        { '-', "-....-" },
        { '\"', ".-..-." },
        { '(', "-.--." },
        { '=', "-...-" },
        { '.', ".-.-.-" },
        { ';', "-..-." },
        { '\'', ".----."},
        { ')', "-.--.-" },
        { '+', ".-.-." },
        { '@', ".--.-." }
    }; 

    private void Start()
    {
        i = GM.activeLessonIndex;
        lsnJson = File.ReadAllText("Assets/questions/lessons.json");
        lessons = Lessons.FromJson(lsnJson);
        questions = lessons.LessonsLessons[i].Questions;

        questionCount = questions.Count;
        countText.text = questionCount.ToString();
        if (_unansweredQuestions == null || _unansweredQuestions.Count == 0) _unansweredQuestions = questions;
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
        var randomQuestionIndex = Random.Range(0, _unansweredQuestions.Count);
        currentQuestion = _unansweredQuestions[randomQuestionIndex];
    }

    private void GetGridQuestions()
    {
        if (btns.Count > 0)
        {
            foreach (var b in btns)
            {
                Destroy(b);
            }
        }
        var randomQuestionIndex = Random.Range(0, _unansweredQuestions.Count);
        var index = 0;
        while (index < 4)
        {
            randomQuestionIndex = Random.Range(0, _unansweredQuestions.Count);
            currentQuestion = _unansweredQuestions[randomQuestionIndex];
            Debug.Log(currentQuestion.String);
            if (gridQuestions.Contains(currentQuestion) || gridQuestions == null)
            {
                return;
            }
            gridQuestions.Add(currentQuestion);
            index++;
        }

        index = 0;
        var correctAnswerIndex = Random.Range(0, 3);
        /*GameObject createdButtonItem = Instantiate(btn);
        createdButtonItem.transform.SetParent(grid.transform);*/
        while (index < 4)
        {
            GameObject createdButtonItem = Instantiate(btn);
            createdButtonItem.GetComponent<ButtonEntryData>().choice.text = ToMorse(gridQuestions[index].String);
            createdButtonItem.transform.SetParent(grid.transform);
            btns.Add(createdButtonItem);
            if (index != correctAnswerIndex)
            {
                createdButtonItem.GetComponent<ButtonEntryData>().answer = false;
            }
            else
            {
                createdButtonItem.GetComponent<ButtonEntryData>().answer = true;
                currentQuestion = gridQuestions[correctAnswerIndex];
            }

            index++;

        }

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
        correct.SetActive(false);
        foreach (var b in btns)
        {
            b.SetActive(false);
        }
        questionCount--;
        countText.text = questionCount.ToString();
        if (questionCount != 0)
        {
            correct.SetActive(true);
            _unansweredQuestions.RemoveAt(questions.IndexOf(currentQuestion));
            answertext.text = "";
            StartCoroutine(NextQuestion(1));
        }
        else
        {
            completeScreen.SetActive(true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
    
    public void Incorrect()
    {
        incorrect.SetActive(false);
        foreach (var b in btns)
        {
            b.SetActive(false);
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
        Debug.Log(answer);
        Debug.Log(answertext.text);
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
        if(isGrid) GetGridQuestions();
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
}