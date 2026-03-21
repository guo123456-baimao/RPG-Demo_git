using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState currentState { get; private set; }

    public void Initialize(PlayerState _playerState)
    {
        currentState = _playerState;
        currentState.Enter();
    }

    public void changeState(PlayerState newStare)
    {
        currentState.Exit();
        currentState = newStare;
        newStare.Enter();
    }


}
