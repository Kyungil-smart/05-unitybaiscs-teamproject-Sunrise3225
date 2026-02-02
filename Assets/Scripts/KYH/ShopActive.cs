using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopActive : MonoBehaviour
{
    [SerializeField] private Canvas _uiShopPanel;
    [SerializeField] private Canvas _uiShopText;
    private CharacterController _player;
    private CameraController _playerCam;

    private Camera _camera;
    private bool _onShop;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        _uiShopText.transform.forward = _camera.transform.forward;
        
        if (_onShop && Input.GetKeyDown(KeyCode.Escape))
        {
            _uiShopPanel.enabled = false;
            _player.CursorLock(true);
            _playerCam._canRotate = true;
            _onShop = false;
            _player._onShopPanel = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        _player = other.GetComponent<CharacterController>();
        _playerCam = other.GetComponent<CameraController>();
        
        if (_player != null)
        {
            _uiShopPanel.enabled = true;
            _player.CursorLock(false);
            _player._onShopPanel = true;
            _playerCam._canRotate = false;
            _onShop = true;
        }
    }
    
    private void Init()
    {
        _uiShopPanel.enabled = false;
        _camera = Camera.main;
    }
}
