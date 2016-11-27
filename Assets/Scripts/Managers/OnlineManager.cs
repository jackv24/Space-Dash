using UnityEngine;
using System.Collections;
using GooglePlayGames;

public class OnlineManager : MonoBehaviour
{
    public static OnlineManager instance;

    private void Start()
    {
        if (instance)
            Destroy(gameObject);
        else
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID || UNITY_IOS
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate((bool success) =>
            {
            //Do stuff
            });
#endif
        }
    }
}
