using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAttackPanel : MonoBehaviour, IUIPanel
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private TextMeshProUGUI _attackIncrease;
    [SerializeField] private TextMeshProUGUI _attackPrice;
    
    private UIPanelManager _uiPanelManager;

    private void Awake()
    {
        _uiPanelManager = GetComponentInParent<UIPanelManager>();
    }
    
    public void RefreshPanelText()
    {
        _attackIncrease.text = $"{_characterController.MaxHp} -> {_characterController.MaxHp + 5}";
        _attackPrice.text = $"{_uiPanelManager.CurretAttackPrice}";
    }
}
