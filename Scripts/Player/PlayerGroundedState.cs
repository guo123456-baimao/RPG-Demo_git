using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.R)&&player.skill.blackhole.blackholeUnlocked)
        {
            if (player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.CreatePopUpText("cooldown");
                return;
            }
            stateMachine.changeState(player.blackHole);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)&&HasNoSword()&&player.skill.sword.swordUnlocked)
        {
            stateMachine.changeState(player.aimSword);
        }
        if (Input.GetKeyDown(KeyCode.Q) && player.skill.parry.parryUnlocked&&player.skill.parry.CanUseSkill())
        {
            stateMachine.changeState(player.counterAttack);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            stateMachine.changeState(player.primaryAttack);
        }
        if (!player.IsGroundDetected())
        {
            stateMachine.changeState(player.airState);
        }

        if (Input.GetKeyDown(KeyCode.Space)&&player.IsGroundDetected())
        {
            stateMachine.changeState(player.jumpState);
        }
    }


    public bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
