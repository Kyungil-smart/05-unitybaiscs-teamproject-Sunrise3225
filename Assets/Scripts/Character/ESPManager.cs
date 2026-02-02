using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ESPManager : MonoBehaviour
{
    private bool _isESPActive = false;
    
    private Outline[] _monsterOutlines;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ScanMonster();
        }
    }

    private void ScanMonster()
    {
        _isESPActive = !_isESPActive;
        
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
}
