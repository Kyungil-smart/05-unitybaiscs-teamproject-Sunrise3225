using UnityEngine;
using UnityEngine.UI;

public class UI_TitlePopup : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditButton;
    void Awake()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnClickStart);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnClickQuit);

        if (creditButton != null)
            creditButton.onClick.AddListener(OnClickCredit);
    }

    void OnClickStart()
    {
        SceneManagement.Instance.LoadGameScene();
        Destroy(gameObject); 
    }

    void OnClickQuit()
    {
        Application.Quit();
    }
    void OnClickCredit()
    {

    }
}