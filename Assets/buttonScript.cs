using UnityEngine;
using UnityEngine.SceneManagement;
public class buttonScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void skipScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

    }

}