using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UIMaxAmmoPanel : MonoBehaviour, IUIPanel
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private TextMeshProUGUI _maxAmmoIncrease;
    [SerializeField] private TextMeshProUGUI _maxAmmoPrice;
    [SerializeField] private Button _purchaseButton;
    
    private ShopPanelManager _uiPanelManager;

    private void Awake()
    {
        _uiPanelManager = GetComponentInParent<ShopPanelManager>();
    }

    private void Start()
    {
        _purchaseButton.onClick.AddListener(MaxAmmoPriceButtonClick);
    }

    private void Update()
    {
        RefreshPanelText();
        ButtonInteractable();
    }
    
    public void RefreshPanelText()
    {
        _maxAmmoIncrease.text = $"{_characterController.MaxMagazine} " +
                                $"-> {_characterController.MaxMagazine + _uiPanelManager.IncreaseMaxAmmoAmount}";
        _maxAmmoPrice.text = $"{_uiPanelManager.MaxAmmoPrice}";
    }

    public void ButtonInteractable()
    {
        if (_characterController.Money >= _uiPanelManager.MaxAmmoPrice)
        {
            _purchaseButton.interactable = true;
        }
        else
        {
            _purchaseButton.interactable = false;
        }
    }

    public void MaxAmmoPriceButtonClick()
    {
        _characterController.Money -= _uiPanelManager.MaxAmmoPrice;
        _characterController.MaxMagazine += _uiPanelManager.IncreaseMaxAmmoAmount;
        _uiPanelManager.MaxAmmoPrice += _uiPanelManager.IncreaseMaxAmmoPrice;
    }
}