using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : State
{
    protected Enemy Enemy; // Cache enemy 'brain' component

    public EnemyState(Enemy enemy)
    {
        Enemy = enemy;
    }
}
