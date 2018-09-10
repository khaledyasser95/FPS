using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSHandsWeapon : MonoBehaviour {
    public AudioClip shootClip, reloadclip;
    private AudioSource audioManager;
    private GameObject muzzleFlash;

    private Animator anim;
    private string SHOOT = "Shoot";
    private string REALOD = "Reload";

	// Use this for initialization
	void Awake () {
        muzzleFlash = transform.Find("Muzzle FLash").gameObject;
        muzzleFlash.SetActive(false);

        audioManager = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
	}
	
    public void Shoot()
    {
        if (audioManager.clip != shootClip)
        {
            audioManager.clip = shootClip;
        }
        audioManager.Play();
        StartCoroutine(TurnMuzzleFlashOn());
        anim.SetTrigger(SHOOT);

    }
    IEnumerator TurnMuzzleFlashOn()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
    public void Reload()
    {
        StartCoroutine(PlayReloadSound());
        anim.SetTrigger(REALOD);
    }

    IEnumerator PlayReloadSound()
    {
        yield return new WaitForSeconds(0.8f);
        if ( audioManager.clip != reloadclip)
        {
            audioManager.clip = reloadclip;
        }
        audioManager.Play();
    }
	// Update is called once per frame
	void Update () {
		
	}
}
