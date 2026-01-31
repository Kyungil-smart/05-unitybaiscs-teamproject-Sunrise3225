using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGoldText : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private TextMeshProUGUI _playerGoldText;

    private void Update()
    {
        RefreshGoldText();
    }

    private void RefreshGoldText()
    {
        _playerGoldText.text = $"GOLD : {_characterController.Money}";
    }
}
