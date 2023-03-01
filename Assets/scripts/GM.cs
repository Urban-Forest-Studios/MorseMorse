using UnityEngine;

public class GM : MonoBehaviour
{
    public static int activeLessonIndex;
    public static GM Instance;
    
    private void Awake()
    {
        //Handles the first time run case
        if (Instance == null)
        {
            //Unity will not destroy the gameObject
            //that this script is attached to.
            //Since the gameObject will live then
            //this instance of the script lives as well.
            DontDestroyOnLoad(gameObject);
            
            //Sets Instance to this (the current running copy 
            //of the GameManger script)
            Instance = this;
        }
        else if (Instance != this)
        {
            //Handles every other time the script
            //is run. If Instance is not the first
            //occurrence of the GameManager script
            //it destroys that duplicate object.
            Destroy(gameObject);
        }
    }
    
}