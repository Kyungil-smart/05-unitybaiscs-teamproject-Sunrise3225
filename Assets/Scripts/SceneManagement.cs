using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public SceneManagement Instance { get; private set; }

    private int _sceneIndex;
    private int _currentSceneIndex;

    void Awake()
    {
        if (Instance != null &&  Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _sceneIndex = _currentSceneIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneLoad();
        }
    }
    
    public void SceneLoad()
    {
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        _sceneIndex = _currentSceneIndex;
        SceneManager.LoadScene(_sceneIndex);
    }
}
