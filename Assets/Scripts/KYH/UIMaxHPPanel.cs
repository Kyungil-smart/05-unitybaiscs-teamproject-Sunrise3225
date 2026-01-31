using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UIMaxHPPanel : MonoBehaviour, IUIPanel
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private TextMeshProUGUI _maxHpIncrease;
    [SerializeField] private TextMeshProUGUI _maxHpPrice;
    [SerializeField] private Button _purchaseButton;
    
    private ShopPanelManager _uiPanelManager;

    private void Awake()
    {
        _uiPanelManager = GetComponentInParent<ShopPanelManager>();
    }

    private void Start()
    {
        _purchaseButton.onClick.AddListener(MaxHpPriceButtonClick);
    }

    private void Update()
    {
        RefreshPanelText();
        ButtonInteractable();
    }
    
    public void RefreshPanelText()
    {
        _maxHpIncrease.text = $"{_characterController.MaxHp}" +
                              $"-> {_characterController.MaxHp + _uiPanelManager.IncreaseMaxHpAmount}";
        _maxHpPrice.text = $"{_uiPanelManager.MaxHpPrice}";
    }

    public void ButtonInteractable()
    {
        if (_characterController.Money >= _uiPanelManager.MaxHpPrice)
        {
            _purchaseButton.interactable = true;
        }
        else
        {
            _purchaseButton.interactable = false;
        }
    }

    private void MaxHpPriceButtonClick()
    {
        _characterController.Money -= _uiPanelManager.MaxHpPrice;
        _characterController.MaxHp += _uiPanelManager.IncreaseMaxHpAmount;
        _uiPanelManager.MaxHpPrice += _uiPanelManager.IncreaseMaxHpPrice;
    }
}