using GameDevHQ.Other.MonoSingletonNS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Manager.UIManagerNS
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField]
        private Text _fundsText = null;

        public static event Action<int> OnTowerButtonClick;

        public void SetWarFundText(int warFunds)
        {
            _fundsText.text = warFunds.ToString();
        }

        public void TowerButtonClick(int selectedTower)
        {
            OnTowerButtonClick?.Invoke(selectedTower);
        }
    }
}

