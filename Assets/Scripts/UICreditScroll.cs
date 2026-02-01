using UnityEngine;

public class UICreditScroll : MonoBehaviour
{
    public float speed = 50f;
    public float stopY = 500f; // 멈출 y 위치 (조정 필요)
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        // 멈출 위치까지 올라가면 멈춤
        if (rect.anchoredPosition.y < stopY)
        {
            rect.anchoredPosition += Vector2.up * speed * Time.deltaTime;
        }
    }
}