using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private MonsterSpawn _monsterSpawn;
    [SerializeField] private GameObject _bossInfoObject;
    [SerializeField] private GameObject _bossHPSliderObject;
    
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _magazineText;
    [SerializeField] private TextMeshProUGUI _bossHpText;
    
    [Header("Icons (Image Components)")]
    [SerializeField] private Image _hpIconImage;
    [SerializeField] private Image _ammoIconImage;
    
    [Header("Icon Sprites (PNG -> Sprite)")]
    [SerializeField] private Sprite _hpIconSprite;
    [SerializeField] private Sprite _ammoIconSprite;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _enemyLeftText;

    private Slider _bossHpSlider;

    private void Awake()
    {
        ApplyIconSprites();

        if (_bossHPSliderObject != null)
        {
            _bossHpSlider = _bossHPSliderObject.GetComponent<Slider>();
            _bossHpSlider.minValue = 0f;
            _bossHpSlider.maxValue = 1f;
        }
    }
    
    private void Update()
    {
        RefreshAll();
    }
    
    private void ApplyIconSprites()
    {
        if (_hpIconImage != null)   _hpIconImage.sprite = _hpIconSprite;
        if (_ammoIconImage != null) _ammoIconImage.sprite = _ammoIconSprite;

        _bossInfoObject.SetActive(false);
    }
    
    private void RefreshAll()
    {
        RefreshHpUI();
        RefreshMagazineUI();
        
        RefreshWaveUI();
        RefreshEnemyLeftUI();
        RefreshMoneyUI();
    }
    
    private void RefreshHpUI()
    {
        if (_hpText == null || _characterController == null) return;
        _hpText.text = $"{(int)_characterController.CurrentHp}";
    }
    
    private void RefreshMagazineUI()
    {
        if (_magazineText == null || _characterController == null) return;
        _magazineText.text = $"{_characterController.CurrentMagazine} / {_characterController.MaxMagazine}";
    }
    
    private void RefreshWaveUI()
    {
        _waveText.text = $"Wave : {_monsterSpawn.WaveCount}";
    }

    private void RefreshEnemyLeftUI()
    {
        _enemyLeftText.text = $"Enemy Left : {_monsterSpawn.AliveMonsterCount}";
    }
    
    private void RefreshMoneyUI()
    {
        _goldText.text = $"Money : {_characterController.Money}";
    }

    public void MonsterInfoUpdate(MonsterController monster)
    {
        if (monster.objectType == Define.ObjectType.Boss)
        {
            if (monster.monsterState != Define.MonsterState.Dead)
            {
                _bossInfoObject.SetActive(true);
                _bossHPSliderObject.GetComponent<Slider>().value = monster.Hp / monster.MaxHp;
                _bossHpText.text = $"{monster.Hp} / {monster.MaxHp}";
            }
            else
                _bossInfoObject.SetActive(false);
        }
    }
}