using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIFunctions : MonoBehaviour
{
    public float sceneLoadDelay = 0.25f;

    public void LoadScene(int index)
    {
        StartCoroutine("LoadSceneWithDelay", index);

        TransitionImageEffect effect = Camera.main.GetComponent<TransitionImageEffect>();

        if (effect)
            effect.PlayTransition();
    }

    IEnumerator LoadSceneWithDelay(int index)
    {
        yield return new WaitForSeconds(sceneLoadDelay);

        SceneManager.LoadScene(index);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
