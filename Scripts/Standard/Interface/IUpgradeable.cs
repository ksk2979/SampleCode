using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeable 
{
    public abstract void Upgrade();
    public abstract void BatchUpgrade();
    public abstract bool VerifyCurrency(int cost, int currency);
    public abstract bool VerifyCurrency(List<int> costList, List<int> currencyList);
    public abstract bool VerifyUpperBoundReached(int targetLevel, int maxLevel);
}
