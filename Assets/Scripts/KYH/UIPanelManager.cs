using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum panelIndex
{
    AttackPanel,
    MaxHPPanel,
    MaxAmmoPanel
}

public class UIPanelManager : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    public List<IUIPanel> _uiPanels;

    private int _curretAttackPrice;
    public int CurretAttackPrice
    {
        get => _curretAttackPrice;
        set => _curretAttackPrice = value;
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        RefreshPanel();
    }

    private void Init()
    {
        
    }

    private void RefreshPanel()
    {
        
    }
}
