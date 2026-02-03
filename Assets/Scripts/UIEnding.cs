using UnityEngine;
using UnityEngine.UI;

public class UIEnding : MonoBehaviour
{
    private Button endQuitButton;

    void Awake()
    {
        endQuitButton = transform.Find("EndQuitButton")?.GetComponent<Button>();

        if (endQuitButton != null)
            endQuitButton.onClick.AddListener(OnClickGameQuit);
    }

    public void OnClickGameQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
