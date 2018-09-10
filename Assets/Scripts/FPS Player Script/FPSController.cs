using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class FPSController : NetworkBehaviour {
    private Transform firstperson_View;
    private Transform frstperson_Camera;

    private Vector3 firstperson_View_Rotation = Vector3.zero;

    public float walkSpeed = 6.75f;
    public float runSpeed = 10f;
    public float crouchSpeed = 4f;
    public float jumpSpeed = 8f;
    public float gravity = 20f;

    private float speed;
    private bool is_Moving, is_Grounded, is_Crouching;

    private float inputX, inputY;
    private float inputX_Set, inputY_Set;
    private float inputModifyFactor;

    private bool limitDiagonalSpeed = true;

    private float antiBumpFactor;
    private CharacterController charController;
    private Vector3 moveDirection = Vector3.zero;

    public LayerMask groundLayer;
    private float rayDistance;
    private float default_ControllerHeight;
    private Vector3 default_CamPos;
    private float camHeight;

    private FPSPlayerAnimation playerAnimation;
    [SerializeField]
    private WeaponManager weapon_Manager;
    private FPSWeapon current_Weapon;

    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private WeaponManager handsWeapon_Manager;
    private FPSHandsWeapon current_Hands_Weapons;


	// Use this for initialization
	void Start () {
        firstperson_View = transform.Find("FPS View").transform;
        charController = GetComponent<CharacterController>();
        speed = walkSpeed;
        is_Moving = false;
        rayDistance = charController.height * 0.5f + charController.radius;
        default_ControllerHeight = charController.height;
        default_CamPos = firstperson_View.localPosition;

        playerAnimation = GetComponent<FPSPlayerAnimation>();
        weapon_Manager.weapons[0].SetActive(true);
        current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon>();

        handsWeapon_Manager.weapons[0].SetActive(true);
        current_Hands_Weapons = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();

	}
	
	// Update is called once per frame
	void Update () {
        // IF WE ARE NOT THE LOCAL PLAER
        // WE ARE NOT RUNNING THIS CODE ON OUR OWN COMPUTER
         if (!isLocalPlayer)
        {
            return;
        }
        PlayerMovement();
        SelectWeapon();
		
	}

    private void PlayerMovement()
    {
        //Forward and Backward
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
                inputY = 1f;
            else inputY = -1f;
        }
        else inputY = 0f;

        // Left and Right
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
                inputX = -1f;
            else inputX = 1f;
        }
        else inputX = 0f;

        inputY = Mathf.Lerp(inputY, inputY_Set, Time.deltaTime * 19f);
        inputX = Mathf.Lerp(inputX, inputX_Set, Time.deltaTime * 19f);

        inputModifyFactor = Mathf.Lerp(inputModifyFactor, (inputY_Set != 0 && inputX_Set != 0 && limitDiagonalSpeed) ? 0.75f : 1.0f, Time.deltaTime * 19f);

        firstperson_View_Rotation = Vector3.Lerp(firstperson_View_Rotation, Vector3.zero, Time.deltaTime * 5f);
        firstperson_View.localEulerAngles = firstperson_View_Rotation;

        if (is_Grounded)
        {
            PlayerCrouchAndSprinting();

            moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
            moveDirection = transform.TransformDirection(moveDirection) * speed;

            PlayerJump();
        }
        moveDirection.y -= gravity * Time.deltaTime;
        is_Grounded= (charController.Move(moveDirection*Time.deltaTime) & CollisionFlags.Below) !=0 ;

        is_Moving = charController.velocity.magnitude > 0.15f;

        HandleAnimations();
    }
    void PlayerCrouchAndSprinting()
    {
        if( Input.GetKeyDown(KeyCode.C))
        {
            if (!is_Crouching)
            {
                is_Crouching = true;

            }else
            {
                if (CanGetUp())
                {
                    is_Crouching = false;
                }
            }
            StopCoroutine(MoveCameraCrouch());
            StartCoroutine(MoveCameraCrouch());

        }

        if (is_Crouching)
        {
            speed = crouchSpeed;
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = runSpeed;
            }else
            {
                speed = walkSpeed;
            }
        }
        playerAnimation.PlayerCrouch(is_Crouching);
    }
    bool CanGetUp()
    {
        RaycastHit groundHit;
        Ray groundRay = new Ray(transform.position, transform.up);
        if (Physics.SphereCast(groundRay,charController.radius +0.05f, out groundHit,rayDistance,groundLayer))
        {
            if (Vector3.Distance(transform.position,groundHit.point)< 2.3f)
            {
                return false;
            }
        }
        return true;
    }
    IEnumerator MoveCameraCrouch()
    {
        charController.height = is_Crouching ? default_ControllerHeight / 1.5f : default_ControllerHeight;
        charController.center = new Vector3(0f, charController.height / 2f, 0f);

        camHeight = is_Crouching ? default_CamPos.y/1.5f : default_CamPos.y;
        
      
        
        while(Mathf.Abs(camHeight - firstperson_View.localPosition.y) > 0.01f)
        {
            firstperson_View.localPosition = Vector3.Lerp(firstperson_View.localPosition,
                new Vector3(default_CamPos.x, camHeight, default_CamPos.z),
                Time.deltaTime * 11f);

            yield return null;
        }
    }
    void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (is_Crouching)
            {
                if (CanGetUp())
                {
                    is_Crouching = false;

                    playerAnimation.PlayerCrouch(is_Crouching);
                    StopCoroutine(MoveCameraCrouch());
                    StartCoroutine(MoveCameraCrouch());
                }
            }else
            {
                moveDirection.y = jumpSpeed;
            }
        }
    }

    void HandleAnimations()
    {
        playerAnimation.Movement(charController.velocity.magnitude);
        playerAnimation.PlayerJump(charController.velocity.y);

        if ( is_Crouching && charController.velocity.magnitude >0f)
        {
            playerAnimation.PlayerCrouchWalk(charController.velocity.magnitude);
        }

        //SHOOTING
        if(Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            if (is_Crouching)
            {
                playerAnimation.Shoot(false);
            }else
            {
                playerAnimation.Shoot(true);
            }
            current_Weapon.Shoot();
            current_Hands_Weapons.Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerAnimation.Reload_Gun();
            current_Hands_Weapons.Reload();
        }

    }

    void SelectWeapon()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if ( !handsWeapon_Manager.weapons[0].activeInHierarchy)
            {
               for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
              {
                    handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_Hands_Weapons = null;
                handsWeapon_Manager.weapons[0].SetActive(true);
                current_Hands_Weapons = handsWeapon_Manager.weapons[0].GetComponent<FPSHandsWeapon>();
                playerAnimation.ChangeController(true);
            }

            if (!weapon_Manager.weapons[0].activeInHierarchy)
            {
                for(int i =0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[0].SetActive(true);
                current_Weapon = weapon_Manager.weapons[0].GetComponent<FPSWeapon>();
                playerAnimation.ChangeController(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!handsWeapon_Manager.weapons[1].activeInHierarchy)
            {
                     for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
                    {
                     handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_Hands_Weapons = null;
                handsWeapon_Manager.weapons[1].SetActive(true);
                current_Hands_Weapons = handsWeapon_Manager.weapons[1].GetComponent<FPSHandsWeapon>();
                playerAnimation.ChangeController(false);
            }

            if (!weapon_Manager.weapons[1].activeInHierarchy)
            {
                for (int i = 0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[1].SetActive(true);
                current_Weapon = weapon_Manager.weapons[1].GetComponent<FPSWeapon>();
                playerAnimation.ChangeController(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!handsWeapon_Manager.weapons[2].activeInHierarchy)
            {
                     for (int i = 0; i < handsWeapon_Manager.weapons.Length; i++)
                   {
                     handsWeapon_Manager.weapons[i].SetActive(false);
                }
                current_Hands_Weapons = null;
                handsWeapon_Manager.weapons[2].SetActive(true);
                current_Hands_Weapons = handsWeapon_Manager.weapons[2].GetComponent<FPSHandsWeapon>();
                playerAnimation.ChangeController(false);
            }

            if (!weapon_Manager.weapons[2].activeInHierarchy)
            {
                for (int i = 0; i < weapon_Manager.weapons.Length; i++)
                {
                    weapon_Manager.weapons[i].SetActive(false);
                }
                current_Weapon = null;
                weapon_Manager.weapons[2].SetActive(true);
                current_Weapon = weapon_Manager.weapons[2].GetComponent<FPSWeapon>();
                playerAnimation.ChangeController(false);
            }
        }
    }
}//class
