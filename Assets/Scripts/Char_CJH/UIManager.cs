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
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private EnemyKillCounter _enemyKillCounter;
    [SerializeField] private MonsterSpawn _monsterSpawn;
    
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _magazineText;
    [SerializeField] private TextMeshProUGUI _killCountText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    
    [Header("Icons (Image Components)")]
    [SerializeField] private Image _hpIconImage;
    [SerializeField] private Image _ammoIconImage;
    [SerializeField] private Image _killIconImage;
    
    [Header("Icon Sprites (PNG -> Sprite)")]
    [SerializeField] private Sprite _hpIconSprite;
    [SerializeField] private Sprite _ammoIconSprite;
    [SerializeField] private Sprite _killIconSprite;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _lifeText;
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _enemyLeftText;

    private void Awake()
    {
        ApplyIconSprites();
    }
    
    private void Update()
    {
        RefreshAll();
    }
    
    private void ApplyIconSprites()
    {
        if (_hpIconImage != null)   _hpIconImage.sprite = _hpIconSprite;
        if (_ammoIconImage != null) _ammoIconImage.sprite = _ammoIconSprite;
        if (_killIconImage != null) _killIconImage.sprite = _killIconSprite;
    }
    
    private void RefreshAll()
    {
        RefreshHpUI();
        RefreshMagazineUI();
        RefreshLifeUI();
        RefreshKillCountUI();
        
        RefreshScoreUI();
        RefreshWaveUI();
        RefreshEnemyLeftUI();
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

    private void RefreshLifeUI()
    {
        _lifeText.text = $"LIFE : {_characterController.PlayerLife}";
    }

    private void RefreshKillCountUI()
    {
        if (_killCountText == null || _enemyKillCounter == null) return;
        _killCountText.text = $"{_enemyKillCounter.Counter}";
    }

    private void RefreshScoreUI()
    {
        _scoreText.text = $"Score : {_scoreManager.Score}";
    }
    
    private void RefreshWaveUI()
    {
        _waveText.text = $"Wave : {_monsterSpawn.WaveCount}";
    }

    private void RefreshEnemyLeftUI()
    {
        _enemyLeftText.text = $"Enemy Left : {_monsterSpawn.AliveMonsterCount}";
    }
}