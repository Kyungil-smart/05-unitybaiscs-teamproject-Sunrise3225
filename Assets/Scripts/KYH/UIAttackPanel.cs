using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAttackPanel : MonoBehaviour, IUIPanel
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private TextMeshProUGUI _attackIncrease;
    [SerializeField] private TextMeshProUGUI _attackPrice;
    [SerializeField] private Button _purchaseButton;
    
    private ShopPanelManager _uiPanelManager;

    private void Awake()
    {
        _uiPanelManager = GetComponentInParent<ShopPanelManager>();
    }

    private void Start()
    {
        _purchaseButton.onClick.AddListener(AttackPriceButtonClick);
    }

    private void Update()
    {
        RefreshPanelText();
        ButtonInteractable();
    }
    
    public void RefreshPanelText()
    {
        _attackIncrease.text = $"{_characterController.AttackDamage}" +
                               $"-> {_characterController.AttackDamage + _uiPanelManager.IncreaseAttackDamageAmount}";
        _attackPrice.text = $"{_uiPanelManager.AttackPrice}";
    }

    public void ButtonInteractable()
    {
        if (_characterController.Money >= _uiPanelManager.AttackPrice)
        {
            _purchaseButton.interactable = true;
        }
        else
        {
            _purchaseButton.interactable = false;
        }
    }

    public void AttackPriceButtonClick()
    {
        _characterController.Money -= _uiPanelManager.AttackPrice;
        _characterController.AttackDamage += _uiPanelManager.IncreaseAttackDamageAmount;
        _uiPanelManager.AttackPrice += _uiPanelManager.IncreaseAttackPrice;
    }
}
