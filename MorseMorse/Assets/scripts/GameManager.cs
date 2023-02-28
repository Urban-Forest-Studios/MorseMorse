using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public Question[] questions;
    private static List<Question> _unansweredQuestions;
    public Text questiontext;
    public Text answertext;
    private Question currentQuestion;

    private void Start()
    {
        if (_unansweredQuestions == null || _unansweredQuestions.Count == 0)
        {
            _unansweredQuestions = questions.ToList<Question>();
        }

        GetRandomQuestion();
        questiontext.text = currentQuestion.question;
        answertext.text = "";
    }

    void GetRandomQuestion()
    {
        int randomQuestionIndex = UnityEngine.Random.Range(0, _unansweredQuestions.Count);
        currentQuestion = _unansweredQuestions[randomQuestionIndex];
        
        _unansweredQuestions.RemoveAt(randomQuestionIndex);
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
        answertext.text = answertext.text.Remove(answertext.text.Length - 1);
    }
    public void Submit()
    {
        if (answertext.text == currentQuestion.answer)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else Debug.Log("incorrect");
    }
}
