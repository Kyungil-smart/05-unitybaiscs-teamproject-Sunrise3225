using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private EnemyKillCounter _enemyKillCounter;
    
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _magazineText;
    [SerializeField] private TextMeshProUGUI _lifeText;
    [SerializeField] private TextMeshProUGUI _killCountText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _enemyLeftText;

    private void Update()
    {
        RefreshAll();
    }
    
    private void RefreshAll()
    {
        RefreshHpUI();
        RefreshMagazineUI();
        RefreshLifeUI();
        RefreshKillCountUI();
        RefreshScoreUI();
    }
    
    private void RefreshHpUI()
    {
        _hpText.text = $"HP : {(int)_characterController.CurrentHp}";
    }
    
    private void RefreshMagazineUI()
    {
        _magazineText.text = $"AMMO : {_characterController.CurrentMagazine} / {_characterController.MaxMagazine}";
    }

    private void RefreshLifeUI()
    {
        _lifeText.text = $"LIFE : {_characterController.PlayerLife}";
    }

    private void RefreshKillCountUI()
    {
        _killCountText.text = $"{_enemyKillCounter.Counter} : Kill";
    }

    private void RefreshScoreUI()
    {
        _scoreText.text = $"Score : {_scoreManager.Score}";
    }
}