using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIRetry : MonoBehaviour
{
    private Button retryButton;
    private Button quitButton;

    void Awake()
    {
        retryButton = transform.Find("RetryButton")?.GetComponent<Button>();
        quitButton = transform.Find("QuitButton")?.GetComponent<Button>();

        if (retryButton != null)
            retryButton.onClick.AddListener(OnClickRetryGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnClickGameQuit);
    }

    public void OnClickRetryGame()
    {
        SceneManager.LoadScene(1);
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
