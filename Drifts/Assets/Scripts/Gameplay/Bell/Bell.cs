﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class Bell : MonoBehaviour
{

    private int curNbBellHits = 0;
    private Team assignedTeam;
    private bool isActive = true;
    private static float bellLength = 2f;


    void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            if (other.gameObject.CompareTag("Orb"))
            {
				CheckBellHit();

                OrbController.Push(transform.right,OrbController.CurrentVelocity * 1.1f);
                DisableBell();
            }
        }
    }

    void DisableBell()
    {
        isActive = false;
        Invoke("OnEnableBell", bellLength);
    }

    void OnEnableBell()
    {
        isActive = true;
    }


    void EnableTeamPower(bool state)
    {
        switch (assignedTeam.powerID)
        {
            case TeamController.powerID.stunt:
                OnEnableStuntPower(state);
                break;
            default:
                OnEnableStuntPower(state);
                break;
        }

        ResetBell();
    }



	void OnEnableStuntPower(bool state)
    {
        if (state)
        {
            UIEffectManager.OnFreezeFrame(OrbController.velocityRatio / 3);
            Invoke("onDisableStuntPower", 2f);
        }

    }

    void DisableStuntPower()
    {
        OnEnableStuntPower(false);
    }

    public void AssignTeam(Team newTeam)
    {
        assignedTeam = newTeam;
    }

    void ResetBell()
    {
        curNbBellHits = 0;
        isActive = true;

    }

    private void CheckBellHit()
    {
        //No Team, invalid
        if (OrbController.PossessedTeam == TeamController.teamID.Neutral || assignedTeam == null)
            return;


        AddScore(TeamController.onReturnOtherTeam(assignedTeam));
        PlayBellSound();
    }

    private void AddScore(Team team)
    {

        curNbBellHits++;
        team.onAddHitScore((int)OrbController.CurrentVelocity / 3);
    }

    private void PlayBellSound()
    {
        WwiseManager.onPlayWWiseEvent("STAGE_BELL", gameObject);
        GetComponent<Animator>().Play("DONG", 0, -1);
    }

}
