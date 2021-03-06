﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PenaltyController : MonoBehaviour
{
    public static PenaltyController Instance { private set; get; }

    public Vector3 PenaltyAngle
    {

        get
        {
            if (currentBarrier == null)
                return Vector3.zero;
            else
            {
                switch (currentBarrier.direction)
                {
                    case TeamBarrier.penaltyDirection.right:
                        return OrbController.Instance.gameObject.transform.right;
                    case TeamBarrier.penaltyDirection.up:
                        return OrbController.Instance.gameObject.transform.up;
                    case TeamBarrier.penaltyDirection.down:
                        return -OrbController.Instance.gameObject.transform.up;
                    case TeamBarrier.penaltyDirection.left:
                        return -OrbController.Instance.gameObject.transform.right;
                    default:
                        return Vector3.zero;
                }
            }
        }
    }

    private TeamBarrier currentBarrier;


    public Text currentPenaltyTxt;

    public const float penaltyLength = 10f;
	[SerializeField] float velocityPenality = 25;

    public float Timer { private set; get; }

    public bool IsEnabled { private set; get; }


    // Use this for initialization
    void Awake()
    {
        Instance = this;
        GameController.SetNextRoundEvent += ResetTimer;
        ResetTimer();
    }

    public void ResetTimer()
    {
        DisableTimer();

        foreach (TeamBarrier barrier in FindObjectsOfType<TeamBarrier>())
        {
            barrier.OnDisableBarrier();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;

        currentPenaltyTxt.text = Timer.ToString("00");

        if (IsPenaltyOver())
            SetPenalty();
    }


    void DisableTimer()
    {
        IsEnabled = false;
        Timer = penaltyLength;
        Instance.enabled = false;

        if (currentBarrier != null)
        {
            currentBarrier.onHidePenaltyText();
        }
    }


    public void EnableTimer(Text txt, TeamBarrier barrier)
    {
        Instance.enabled = true;
        Timer = penaltyLength;
        currentPenaltyTxt = txt;
        currentBarrier = barrier;
    }

    bool IsPenaltyOver()
    {
        return Timer <= 0;
    }

    void SetPenalty()
    {
		OrbController.Instance.StopVelocity();
		OrbController.Instance.Push(PenaltyAngle, velocityPenality, TeamController.TeamID.Neutral);
        OrbController.Instance.DisableOrb();
        currentBarrier.onHidePenaltyText();
        DisableTimer();

    }
}
