using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private int _startingWarFunds = 0;
    private int _currentWarFunds;

    // Start is called before the first frame update
    void Start()
    {
        _currentWarFunds = _startingWarFunds;
        UIManager.Instance.SetWarFundText(_currentWarFunds);
    }

    public void ChangeFunds(int amount, bool addFunds)
    {
        if (addFunds == true)
        {
            _currentWarFunds += amount;
        }
        else
        {
            _currentWarFunds -= amount;
        }
        UIManager.Instance.SetWarFundText(_currentWarFunds);
    }

    public bool CheckFunds(int cost)
    {
        if (_currentWarFunds >= cost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
