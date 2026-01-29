using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _magazineText;
    [SerializeField] private TextMeshProUGUI _lifeText;
    
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _enemyLeftText;
    [SerializeField] private TextMeshProUGUI _killCountText;

    private void RefreshHpUI()
    {
        _hpText.text = $"HP : {(int)_characterController.CurrentHp} / {_characterController.MaxHp}";
    }

}