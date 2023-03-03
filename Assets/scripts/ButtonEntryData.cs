using UnityEngine;
using UnityEngine.UI;

public class ButtonEntryData : MonoBehaviour
{
    public GameManager gameManager;
    public Text choice;
    public bool answer;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("gamemanager").GetComponent<GameManager>();
    }

    public void ClickMe()
    {
        if (answer) gameManager.Correct();
        else gameManager.Incorrect();
    }
}