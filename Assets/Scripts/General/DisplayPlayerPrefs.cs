using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayPlayerPrefs : MonoBehaviour
{
    public enum Pref
    {
        Float,
        Int,
        String
    }
    public Pref pref;

    public string key;

    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Start()
    {
        switch (pref)
        {
            case Pref.Float:
                text.text = string.Format(text.text, PlayerPrefs.GetFloat(key, 0));
                break;
            case Pref.Int:
                text.text = string.Format(text.text, PlayerPrefs.GetInt(key, 0));
                break;
            case Pref.String:
                text.text = string.Format(text.text, PlayerPrefs.GetString(key, ""));
                break;
        }
    }
}
