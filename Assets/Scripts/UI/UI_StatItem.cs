using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

[Serializable]
public struct StatUpgradeInfo
{
    public string title;
    public int price;
    public int increase;
}
public class UI_StatItem : MonoBehaviour
{
    [SerializeField] TMP_Text TitleText;
    [SerializeField] TMP_Text ChangeText;
    [SerializeField] TMP_Text MoneyText;
    [SerializeField] Button upgradeButton;

    StatType _statType;
    StatUpgradeInfo _info;
    CharacterController player;

    Coroutine _coolTime;
    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        player = GetComponent<CharacterController>();
    }
    public void SetInfo(StatType statType, CharacterController owner)
    {
        _statType = statType;
        _info = GetUpgradeInfo(_statType);
        player = owner;
        RefreshUI();
    }
    void RefreshUI()
    {
        int value = GetStatValue(_statType);
        TitleText.text = _info.title;

        int next = GetIncreaseValue(value);
        ChangeText.text = $"{value} -> {next}";
        MoneyText.text = _info.price.ToString();
        upgradeButton.interactable = CanUpgrade();
    }
    int GetIncreaseValue(int current)
    {
        return current + _info.increase;
    }
    bool CanUpgrade()
    {
        return player.Money >= _info.price;
    }

    public void OnClickUpgradeButton()
    {
        if (_coolTime != null) return;

        if (CanUpgrade())
        {
            player.Money -= _info.price;
            int value = _info.increase;

            switch (_statType)
            {
                case StatType.MaxHp:
                    player.MaxHp += value;
                    break;
                case StatType.Attack:
                    player.Attack += value;
                    break;
                case StatType.MaxMagazine:
                    player.MaxMagazine += value;
                    break;
            }
            RefreshUI();
        }
        _coolTime = StartCoroutine(CoStartUpgradeCoolTime(0.1f));
    }
    IEnumerator CoStartUpgradeCoolTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coolTime = null;
    }

    StatUpgradeInfo GetUpgradeInfo(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHp:
                return new StatUpgradeInfo { title = "최대 체력", price = 100, increase = 10 };
            case StatType.Attack:
                return new StatUpgradeInfo { title = "공격력", price = 200, increase = 5 };
            case StatType.MaxMagazine:
                return new StatUpgradeInfo { title = "최대 탄창", price = 130, increase = 10 };
        }
        return default;
    }

    public int GetStatValue(StatType stat)
    {
        switch (stat)
        {
            case StatType.MaxHp:
                return player.MaxHp;
            case StatType.Attack:
                return player.Attack;
            case StatType.MaxMagazine:
                return player.MaxMagazine;
        }
        return 0;
    }
}
