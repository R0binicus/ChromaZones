using Unity.VisualScripting;
using UnityEngine;

public class ResetRegionLinkData
{
    public bool _isPlayer = false;
    public Enemy _enemy = null; 

    public ResetRegionLinkData(bool IsPlayer)
    {
        _isPlayer = IsPlayer;
    }

    public ResetRegionLinkData(Enemy Enemy)
    {
        _enemy = Enemy;
    }
}
