using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance { get; private set; }

    [Header("Scene Name")]
    [SerializeField] private string gameSceneName;
    [SerializeField] private string shopSceneName;

    [Header("UI Popup")]
    [SerializeField] private UI_IntroPopup introPopupPrefab;
    [SerializeField] private UI_TitlePopup titlePopupPrefab;
    void Awake()
    {
        if (Instance != null &&  Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (titlePopupPrefab != null)
            titlePopupPrefab = Instantiate(titlePopupPrefab, null);
    }
    public void LoadGameScene()
    {
        UI_IntroPopup popup = Instantiate(introPopupPrefab, null);
        popup.gameObject.SetActive(true);

        popup.SetInfo(0, 3, () =>
        {
            if (!string.IsNullOrEmpty(gameSceneName))
                SceneManager.LoadScene(gameSceneName);
        });
    }
    public void LoadShopScene()
    {
        if (!string.IsNullOrEmpty(shopSceneName))
            SceneManager.LoadScene(shopSceneName);
    }

    public void SceneLoad(string sceneName) => SceneManager.LoadScene(sceneName);
}
