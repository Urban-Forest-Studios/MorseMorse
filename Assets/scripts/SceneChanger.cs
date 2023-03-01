using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public bool changeScene;

    // Update is called once per frame
    private void Update()
    {
        if (changeScene) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}