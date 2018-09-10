using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerAnimation : MonoBehaviour {
    private Animator anim;

    private string MOVE = "Move";
    private string VELOCITY_Y = "VelocityY";
    private string CROUCH = "Crouch";
    private string CROUCH_WALK = "CrouchWalk";

    private string STAND_SHOOT = "Standshoot";
    private string Crouch_shoot = "Crouchshoot";
    private string Reload = "Reload";

    public RuntimeAnimatorController animController_Pistol, animCotroller_MachinGun;


    private void Awake()
    { 
            anim = GetComponent<Animator>();
      
    }
    public void Movement(float magnitude)
    {
        anim.SetFloat(MOVE, magnitude);
    }

    public void PlayerJump(float velocity)
    {
        anim.SetFloat(VELOCITY_Y, velocity);
    }
    public void PlayerCrouch ( bool isCrouching)
    {
        anim.SetBool(CROUCH, isCrouching);
    }
    public void PlayerCrouchWalk(float magnitude)
    {
        anim.SetFloat(CROUCH_WALK, magnitude);
    }
    public void Shoot(bool isStanding)
    {
        if (isStanding)
        {
            anim.SetTrigger(STAND_SHOOT);

        }
        else
        {
            anim.SetTrigger(Crouch_shoot);
        }
    }

    public void Reload_Gun()
    {
        anim.SetTrigger(Reload);
    }

    public void ChangeController(bool isPistol)
    {
        if (isPistol)
        {
            anim.runtimeAnimatorController = animController_Pistol;
        }
        else
            {
            anim.runtimeAnimatorController = animCotroller_MachinGun;

        }
    }



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
