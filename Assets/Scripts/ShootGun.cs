using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ShootGun : MonoBehaviour
{

    public SteamVR_Action_Boolean shootAction;


    public GameObject bullet;

    public AudioSource audioSource;

    public AudioClip gunFire;

    public AudioClip emptyFire;

    public Transform barrel;

    public ParticleSystem shootFlash;

    public float bulletSpeed = 1;

    private Animator animator;

    public SpawnProjectiles game;

    public int TotalBullets = 6;

    public int numBullets = 6;

    public SteamVR_Input_Sources hand;




    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponent<Animator>();
        this.numBullets = TotalBullets;
        shootFlash.Stop();
    }

    // Update is called once per frame
    void Update()
    {
            if (shootAction[SteamVR_Input_Sources.RightHand].stateDown)
            {
                if (game.currentLevel.levelStarted && !game.currentLevel.levelCompleted && !game.failedLevel && numBullets > 0)
                    shoot();
                else
                    playEmptyFire();
                //play empty gun noise
            }
        

    }

    void shoot()
    {
        shootFlash.Stop();
        GameObject bullet = Instantiate(this.bullet, barrel.position, barrel.rotation * Quaternion.Euler(0, 90, 0));
        bullet.GetComponent<Rigidbody>().velocity = barrel.forward * bulletSpeed;
        GameObject.Destroy(bullet, 1);
        playGunFire();
        shootFlash.Play();
        hapticPulse();
        this.numBullets -= 1;
    }

    public bool hasAnyBullets()
    {
        if (this.numBullets == 0)
        {
            return false;
        }
        return true;
    }

    public void resetBullets()
    {
        this.numBullets = TotalBullets;
    }

    void playGunFire()
    {
        audioSource.clip = gunFire;
        audioSource.PlayOneShot(gunFire);
    }

    void playEmptyFire()
    {
        audioSource.clip = emptyFire;
        audioSource.PlayOneShot(emptyFire);
    }
    void hapticPulse()
    {
        SteamVR_Actions.default_Haptic[SteamVR_Input_Sources.RightHand].Execute(0, 1, 10, 1);
    }

}
