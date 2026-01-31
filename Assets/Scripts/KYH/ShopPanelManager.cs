using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum panelIndex
{
    AttackPanel,
    MaxHPPanel,
    MaxAmmoPanel
}

public class ShopPanelManager : MonoBehaviour
{
    [Header("공격력")]
    public int IncreaseAttackDamageAmount;
    public int IncreaseAttackPrice;
    
    [Header("최대 체력")]
    public int IncreaseMaxHpAmount;
    public int IncreaseMaxHpPrice;
    
    [Header("최대 탄창")]
    public int IncreaseMaxAmmoAmount;
    public int IncreaseMaxAmmoPrice;

    private int _attackPrice;
    public int AttackPrice
    {
        get => _attackPrice;
        set => _attackPrice = value;
    }

    private int _maxHpPrice;
    public int MaxHpPrice
    {
        get => _maxHpPrice;
        set => _maxHpPrice = value;
    }
    
    private int _maxAmmoPrice;
    public int MaxAmmoPrice
    {
        get => _maxAmmoPrice;
        set => _maxAmmoPrice = value;
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _attackPrice = 2000;
        _maxHpPrice = 2000;
        _maxAmmoPrice = 2000;
    }

}
