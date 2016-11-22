﻿using UnityEngine;
using System.Collections;

public class OrbController : MonoBehaviour
{

    public static OrbController Instance { get { return instance; } }
    static OrbController instance;

    private static Vector3 destinationAngle; // angle pushing the ball

    public static Vector3 DestinationAngle { get { return destinationAngle; } }

    private static float currentVelocity, destinationVelocity, LerpTimer; //Speed variables

    public static float velocityRatio { get { return Mathf.Clamp01(currentVelocity / MaxVelocity); } }

    public static float MaxVelocity { get { return Instance._MaxVelocity; } }

    public static float MinVelocity { get { return Instance._MinVelocity; } }

    public static float MomentumVelocity { get { return Instance._MomentumVelocity; } }

    public static float CurrentVelocity { get { return Mathf.Clamp(currentVelocity, MinVelocity, MaxVelocity); } } //Clamp & Return speed

    public static float DecreaseVelocity { get { return Instance._DecreaseVelocity; } }

    public static float MomentumBell { get { return Instance._MomentumBell; } }

    public static Color NeutralColor { get { return Instance._NeutralColor; } }
    public static Color Team1Color { get { return Instance._Team1Color; } }
    public static Color Team2Color { get { return Instance._Team2Color; } }

    private static int orbStateID = 0;
    public static int OrbStateID { get { return orbStateID; } }

    public static Transform[] OrbSpawnPoints { get { return GameController.Instance._OrbSpawnPoints; } }

    // Public variable for game designers to tweek ball velocity.
    public float _MaxVelocity, _MinVelocity, _DecreaseVelocity, _MomentumVelocity, _MomentumBell;
    static void onSetDestinationVelocity() { destinationVelocity = Mathf.Clamp(destinationVelocity, MinVelocity, MaxVelocity); }

    // Public variable for game designers to tweek ball color.
    public Color _NeutralColor, _Team1Color, _Team2Color;

    public static ParticleSystem ParticleSystemRender { get { return Instance.pSystem; } }

    private Rigidbody rBody;

    public Rigidbody RigidBody { get { return rBody; } }

    private static TeamController.teamID possessedTeam = 0;

    public static TeamController.teamID PossessedTeam { get { return possessedTeam; } }

    public TeamController.teamID _PossessedTeam;

    private static bool isPushed,isPushable = true;

    [SerializeField]
    public ParticleSystem pSystem;


    public GameObject mainOrb;




    // Use this for initialization
    void Awake()
    {
        instance = this;
        onSetComponents();
        onChangeTeamPossession(TeamController.teamID.Neutral);
        GameController.SetNextRound += onReset;
    }


    public static void shouldBallBeEnabled(bool state)
    {
        instance.gameObject.SetActive(state);
    }

    public static void onChangeTeamPossession(TeamController.teamID newTeam)
    {
        possessedTeam = newTeam;

        if (newTeam == TeamController.teamID.Neutral)
            Instance.pSystem.startColor = NeutralColor;
        else if (newTeam == TeamController.teamID.Team1)
            Instance.pSystem.startColor = Team1Color;
        else if (newTeam == TeamController.teamID.Team2)
            Instance.pSystem.startColor = Team2Color;

        Instance.pSystem.GetComponent<ParticleSystemRenderer>().material.SetColor("_EmisColor", Instance.pSystem.startColor);

    }






    private void onSetComponents()
    {
        rBody = GetComponent<Rigidbody>();
        instance = this;
    }

    private void onSetProperties()
    {
        currentVelocity = MinVelocity;
        destinationVelocity = 0;
        isPushable = true;
        rBody.velocity = Vector3.zero;
        isPushed = false;
        LerpTimer = 0f;

    }

    public static void onReset()
    {
        instance.gameObject.SetActive(true);
        instance.onSetProperties();
        instance.gameObject.transform.position = OrbSpawnPoints[Random.Range(0, OrbSpawnPoints.Length)].position;
    }


    public static void onPush(Vector3 angle, TeamController.teamID teamID)
    {

            isPushed = true;
            destinationVelocity = MaxVelocity / 3;
            onSetDestinationVelocity();
            destinationAngle = angle;
            Instance.onSetBallStage();
            onChangeTeamPossession(teamID);
        
      
    }


    public static void onPush(Vector3 angle, Player player)
    {
        if (isPushable)
        {
            isPushed = true;
            float additionalVel = player.PulledVelocity != 0 ? player.PulledVelocity : 0;
            destinationVelocity = currentVelocity + additionalVel + (MomentumVelocity * player.Owner.LeftTriggerHold.holdingButtonRatio);
            onSetDestinationVelocity();
            destinationAngle = angle;
            Instance.onSetBallStage();
            player.onSetPulledVelocity(0);
        }
    }


    public static void onPush(float destVelocity)
    {
        if (isPushable)
        {
            isPushed = true;
            destinationVelocity = destVelocity;
            onSetDestinationVelocity();
            Instance.onSetBallStage();
        }
    }

    public static void onPush(Vector3 angle, float destVelocity)
    {
        if (isPushable)
        {
            isPushed = true;
            destinationVelocity = destVelocity;
            onSetDestinationVelocity();
            destinationAngle = angle;
            Instance.onSetBallStage();
        }
    }




    void onSetBallStage()
    {
        int previousOrbID = OrbStateID;


        if (CurrentVelocity > 100)
        {
            mainOrb.SetActive(true);

            if (CurrentVelocity > 500)
                orbStateID = 3;

            else if (CurrentVelocity > 300)
                orbStateID = 2;

            else
                orbStateID = 1;


            mainOrb.GetComponent<Animator>().Play("stage" + OrbStateID.ToString());
        }
        else
        {
            orbStateID = 0;
            mainOrb.SetActive(false);
        }





        if (previousOrbID != OrbStateID)
        {
            if (previousOrbID < OrbStateID)
                WwiseManager.onPlayWWiseEvent("BALL_STATE_UP", gameObject);
            else
                WwiseManager.onPlayWWiseEvent("BALL_STATE_DOWN", gameObject);
        }

    }


    public static void onPull(Vector3 angle, float velocityApplied)
    {
        isPushed = true;
        destinationVelocity = currentVelocity + velocityApplied;
        Instance.RigidBody.velocity = (angle * -destinationVelocity);
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        onChangeVelocity();
    }


    public static void onChangeAngle(Vector3 Angle)
    {
        destinationAngle = Angle;
    }


    private void onChangeVelocity()
    {
        if (isPushed)
        {
            LerpTimer += Time.deltaTime * 3;
            if (LerpTimer >= 1)
            {
                LerpTimer = 0;
                isPushed = false;
            }


            else
            {
                currentVelocity -= DecreaseVelocity;

            }

            // Clamp max and min velocity
            currentVelocity = Mathf.Clamp(currentVelocity, _MinVelocity, _MaxVelocity);

            //rBody.velocity = rBody.velocity.normalized * currentVelocity;

        }
        currentVelocity = Mathf.Lerp(currentVelocity, destinationVelocity, Mathfx.Sinerp(0, 1, LerpTimer));
        rBody.velocity = Vector3.Lerp(rBody.velocity, destinationVelocity * destinationAngle, Mathfx.Sinerp(0, 1, LerpTimer));


    }

    public static void OnDisableOrb()
    {
        isPushable = false;
    }

    public static void OnEnableOrb()
    {
        isPushable = true;
    }

}