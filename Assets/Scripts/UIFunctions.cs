using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using GooglePlayGames;

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

    public void ShowAchievements()
    {
#if UNITY_ANDROID || UNITY_IOS
        Social.ShowAchievementsUI();
#endif
    }

    public void ShowLeaderboard()
    {
#if UNITY_ANDROID || UNITY_IOS
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_scores);
#endif
    }
}
