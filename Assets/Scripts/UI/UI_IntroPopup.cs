using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_IntroPopup : MonoBehaviour
{
    [Header("Pages (이미지 순서대로 넣기)")]
    [SerializeField] private GameObject[] pages;

    [Header("Click Area (이미지/버튼)")]
    [SerializeField] private Button clickButton;

    [Header("Texts(텍스트)")]
    [SerializeField] private GameObject introText1Obj;
    [SerializeField] private GameObject introText2Obj;
    [SerializeField] private GameObject introText3Obj;
    [SerializeField] private GameObject introText4Obj;

    [Header("페이지 인덱스 지정")]
    [SerializeField] private int text1PageIndex = 0;
    [SerializeField] private int text2PageIndex = 1;
    [SerializeField] private int text3PageIndex = 2;
    [SerializeField] private int text4PageIndex = 3; 


    Action _onEndCallback;

    int _selectedIndex;
    int _startIndex = 0;
    int _lastIndex; // pages 마지막 인덱스

    void Awake()
    {
        if (clickButton != null)
            clickButton.onClick.AddListener(OnClickImage);

        _selectedIndex = _startIndex;

        if (pages != null && pages.Length > 0)
        _lastIndex = pages.Length - 1;

        RefreshUI();
    }
    public void SetInfo(int startIndex, int endIndex, Action onEndCallback)
    {
        _startIndex = startIndex;
        _selectedIndex = startIndex;
        _lastIndex = endIndex;
        _onEndCallback = onEndCallback;

        RefreshUI();
    }

    void RefreshUI()
    {
        // 전부 끄기
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(false);

        introText1Obj.gameObject.SetActive(false);
        introText2Obj.gameObject.SetActive(false);
        introText3Obj.gameObject.SetActive(false);
        introText4Obj.gameObject.SetActive(false);

        // 인덱스 범위 방어
        if (_selectedIndex < 0) _selectedIndex = 0;
        if (_selectedIndex >= pages.Length) _selectedIndex = pages.Length - 1;

        for (int i = 0; i <= _selectedIndex; i++)
            pages[i].SetActive(true);

        if (_selectedIndex == text1PageIndex && introText1Obj != null) introText1Obj.gameObject.SetActive(true);
        if (_selectedIndex == text2PageIndex && introText2Obj != null) introText2Obj.gameObject.SetActive(true);
        if (_selectedIndex == text3PageIndex && introText3Obj != null) introText3Obj.gameObject.SetActive(true);
        if (_selectedIndex == text4PageIndex && introText4Obj != null) introText4Obj.gameObject.SetActive(true);
    }

    void OnClickImage()
    {
        // 끝났으면 닫는다
        if (_selectedIndex >= _lastIndex)
        {
            Close();
            _onEndCallback?.Invoke();
            return;
        }
        _selectedIndex++;
        RefreshUI();
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}