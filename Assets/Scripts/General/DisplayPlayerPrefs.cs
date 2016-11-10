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
        string value = "";

        switch (pref)
        {
            case Pref.Float:
                value = PlayerPrefs.GetFloat(key, 0).ToString();
                break;
            case Pref.Int:
                value = PlayerPrefs.GetInt(key, 0).ToString();
                break;
            case Pref.String:
                value = PlayerPrefs.GetString(key, "").ToString();
                break;
        }

        text.text = string.Format(text.text, value);
    }
}
