using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSShootingControl : MonoBehaviour {

    private Camera mainCam;
    private float fireRate = 15f;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private GameObject concrete_Impact;


	// Use this for initialization
	void Start () {
        mainCam = Camera.main;	
	}
	
	// Update is called once per frame
	void Update () {
        Shoot();
	}
    void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            RaycastHit hit;
            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit))
            {
                Instantiate(concrete_Impact, hit.point, Quaternion.LookRotation(hit.normal));
               // print("WE HIT " + hit.collider.gameObject.name);
               // print("THE POSITION OF THE GAME OBJECT IS " + hit.transform.position);
               // print("The POINT WHERE THE HIT HAPPENED IS " + hit.point);
            }
        }
    }
}//class
