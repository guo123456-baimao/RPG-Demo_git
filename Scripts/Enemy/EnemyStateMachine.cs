using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine                       //µÐÈË-×´Ì¬»ú
{
    public EnemyState currentState { get; private set; }
    public void Initialize(EnemyState _startState)
    {
        currentState = _startState;
        currentState.Enter();
    }
    public void ChangeState(EnemyState _newState)
    {
        currentState.Exit();
        currentState = _newState;
        _newState.Enter();
    }
}
