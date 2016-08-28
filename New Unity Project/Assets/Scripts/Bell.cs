﻿using UnityEngine;
using System.Collections;

public class Bell : MonoBehaviour {

    private int curNbBellHits = 0;
    private int nbBellHits = 5;
    private Team assignedTeam;


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Orb"))
		{
            onBellHit();

            if (shouldEnableTeamPower())
                onEnableTeamPower();
            else
                UiManager.OnFreezeFrame(.1f, 1);

            ballManager.onPush (ballManager.MomentumBell + ballManager.CurrentVelocity);
		}

	}

    bool shouldEnableTeamPower()
    {
        return curNbBellHits >= nbBellHits;
    }

    void onEnableTeamPower()
    {
        Debug.Log("POWER ENABLED!");
        switch(assignedTeam.powerID)
        {
            case teamManager.powerID.stunt:
                onEnableStuntPower();
                break;
            default:
                onEnableStuntPower();
                break;
        }
      
        onResetBell();
    }

    void onEnableStuntPower()
    {
        UiManager.OnFreezeFrame(.5f, 5);
        assignedTeam.onStunt(true);
    }

    public void onAssignTeam(Team newTeam)
    {
        assignedTeam = newTeam;
    }

    void onResetBell()
    {
        curNbBellHits = 0;
    }

    public void onBellHit()
    {
        curNbBellHits++;
        if (WwiseManager.isWwiseEnabled)
            AkSoundEngine.PostEvent("STAGE_BELL", gameObject);

        GetComponent<Animator>().Play("DONG", 0, -1);
    }
}
