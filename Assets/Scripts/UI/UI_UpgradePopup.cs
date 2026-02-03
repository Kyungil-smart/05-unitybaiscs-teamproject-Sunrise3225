using UnityEngine;
using static Define;

public class UI_UpgradePopup : MonoBehaviour
{
    [SerializeField] private UI_StatItem hpItem;
    [SerializeField] private UI_StatItem atackItem;
    [SerializeField] private UI_StatItem magItem;
    [SerializeField] private CharacterController player;

    CursorLockMode _prevLockMode;
    bool _prevVisible;

    private void OnEnable()
    {
        _prevLockMode = Cursor.lockState;
        _prevVisible = Cursor.visible;

        hpItem.SetInfo(StatType.MaxHp, player);
        atackItem.SetInfo(StatType.Attack, player);
        magItem.SetInfo(StatType.MaxMagazine, player);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void OnDisable()
    {
        Cursor.lockState = _prevLockMode;
        Cursor.visible = _prevVisible;
    }
    private void Start()
    {
        hpItem.SetInfo(StatType.MaxHp, player);
        atackItem.SetInfo(StatType.Attack, player);
        magItem.SetInfo(StatType.MaxMagazine, player);
    }
}
