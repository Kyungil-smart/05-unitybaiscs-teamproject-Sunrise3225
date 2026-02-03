using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ESPManager : MonoBehaviour
{
    private bool _isESPActive = false;
    private bool _canUseESP = true;
    private bool _UseESP = false;
    private Outline[] _monsterOutlines;

    private float _cooldown = 7f;
    private float _scanTime = 2f;
    private float _timeCount = 0f;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && TryUseESP())
        {
            ScanMonster();
            _UseESP = true;
        }

        if (_UseESP)
        {
            _timeCount += Time.deltaTime;
            if (_timeCount >= _scanTime)
            {
                ScanOff();
                _UseESP = false;
                _timeCount = 0;
            }
        }
    }

    private void ScanMonster()
    {
        _isESPActive = true;
        
        _monsterOutlines = FindObjectsOfType<Outline>();
        foreach (Outline mstOutline in _monsterOutlines)
        {
            if (mstOutline == null) continue;
            
            mstOutline.enabled = _isESPActive;
            mstOutline.OutlineMode = Outline.Mode.OutlineAll;
            mstOutline.OutlineWidth = 7f;
            mstOutline.OutlineColor = Color.red;
        }
    }

    private bool TryUseESP()
    {
        if (_canUseESP)
        {
            StartCoroutine(CooldownRoutine());
            return true;
        }
        return false;
    }

    private IEnumerator CooldownRoutine()
    {
        _canUseESP = false;
        yield return new WaitForSeconds(_cooldown);
        _canUseESP = true;
    }

    private void ScanOff()
    {
        _isESPActive = false;
        
        _monsterOutlines = FindObjectsOfType<Outline>();
        foreach (Outline mstOutline in _monsterOutlines)
        {
            if (mstOutline == null) continue;
            
            mstOutline.enabled = _isESPActive;
        }
    }
}