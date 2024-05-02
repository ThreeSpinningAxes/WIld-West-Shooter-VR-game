using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class SpawnProjectiles : MonoBehaviour
{
    int levelNumber = 0;

    public SteamVR_Action_Boolean startOrRestartAction;

    public int totalLevels = 3;

    public GameObject levelOneProjectilePrefab;

    public GameObject levelTwoProjectilePrefab;

    public GameObject levelThreeProjectilePrefab;

    public ShootGun gunController;

    public Level currentLevel;

    public List<Level> levels;

    public bool gameStarted = false;

    public bool finishedGame = false;

    public bool failedLevel = false;

    public Countdown countdown;


    private Transform originalCameraTransform;

    public GameObject playerCamera;

    XRInputSubsystem subsystem;

    

    public class Level
    {
        public int totalProjectiles;

        public int depth;

        //for bounding box
        int x1 = -8;

        int x2 = 0;

        int minDepth = 10;

        int groundLevel = -10;

        int minDropHeight = 20;

        int maxDropHeight = 12;

        public Transform location;

        public List<GameObject> projectiles;

        public GameObject projectile;

        public bool levelStarted = false;

        public bool levelCompleted = false;



        public Level(GameObject projectile, int totalProjectiles, int depth, int maxDropHeight)
        {
            this.projectile = projectile;
            this.totalProjectiles = totalProjectiles;
            this.projectiles = new List<GameObject>();
            this.depth = minDepth + depth;
            this.maxDropHeight += maxDropHeight;
        }

        Vector3 getRandomSpawnLocation()
        {
            return new Vector3(Random.Range(x1, x2 + 1), Random.Range(minDropHeight, maxDropHeight + 1), depth);
        }

        public void initProjectiles()
        {
            for (int i = 0; i < totalProjectiles; i++)
            {
                if (i == totalProjectiles - 1)
                    this.projectiles.Add(Instantiate(this.projectile, this.getRandomSpawnLocation(), Quaternion.Euler(-90, 0, 0)));
            }
        }

        public bool hitGround()
        {
            for (int i = 0; i < this.projectiles.Count; i++)
            {
                float distToGround = this.projectiles[i].GetComponent<Collider>().bounds.extents.y;
                if (Physics.Raycast(projectiles[i].transform.position, Vector3.down, distToGround + 0.1f))
                {
                    return true;
                }
            }
            return false;
        }


        public void stopLevel()
        {
            this.levelStarted = false;
            this.levelCompleted = false;
            foreach (GameObject proj in projectiles)
            {
                Destroy(proj);
            }
            this.projectiles = new List<GameObject>();
        }

        public void startLevel()
        {
            initProjectiles();
            Debug.Log(this.projectiles.Count);
            this.levelStarted = true;
        }

    }
    // Start is called before the first frame update


    void Start()
    {
        Level levelOne = new Level(levelOneProjectilePrefab, 1, 0, 3);
        Level levelTwo = new Level(levelTwoProjectilePrefab, 2, 0, 3);
        Level levelThree = new Level(levelThreeProjectilePrefab, 3, 0, 3);
        this.levels = new List<Level> { levelOne, levelTwo, levelThree };
        this.currentLevel = levelOne;
        this.originalCameraTransform = playerCamera.transform; 
        this.subsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
    }

    bool levelLost()
    {
        if (!this.gunController.hasAnyBullets() || this.currentLevel.hitGround())
            return true;
        return false;
    }

    void restartGame()
    {
        gameStarted = true;
        this.currentLevel.stopLevel();
        this.levelNumber = 0;
        this.currentLevel = levels[0];
        this.currentLevel.stopLevel();
        this.countdown.startCountdown();
        this.gunController.resetBullets();
        this.resetCamera();
    }

    public void projectileHit(GameObject gameObject)
    {
        Debug.Log("Removing " + gameObject.name);
        this.currentLevel.projectiles.Remove(gameObject);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //start of fresh game
        if (!gameStarted && !this.currentLevel.levelStarted)
        {
            if (startOrRestartAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                gameStarted = true;
                countdown.startCountdown();
            }
        }
        else if (this.failedLevel)
        {
            if (startOrRestartAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                failedLevel = false;
                restartGame();
            }
        }
        else if (this.finishedGame)
        {
            if (startOrRestartAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                this.finishedGame = false;
                restartGame();
            }
        }
        else
        {
            if (countdown.finished == true && !this.currentLevel.levelStarted)
            {
                this.currentLevel.startLevel();
                this.countdown.finished = false;
            }

            if (this.currentLevel.levelStarted == true)
            {

                if (this.currentLevel.projectiles.Count == 0)
                {
                    Debug.Log("asd");
                    this.currentLevel.levelCompleted = true;
                }
                else if (levelLost())
                {
                    this.failedLevel = true;
                    this.countdown.showLevelFailed();
                }

                // if (this.currentLevel.failedLevel()) {
                //     this.countdown.showLevelFailed();
                // }
            }

            if (this.currentLevel.levelCompleted)
            {

                if (this.levelNumber == this.totalLevels - 1)
                {
                    countdown.setText("You have completed the game!\nPress A to restart.", 8);
                    this.finishedGame = true;
                }
                else
                {
                    if (!this.countdown.regularTimerEnabled && !this.countdown.finished)
                    {
                        this.countdown.showLevelCompleted();
                        this.countdown.startRegularTimer(2);
                    }
                    else if (this.countdown.disabled && this.countdown.finished)
                    {
                        Debug.Log("start timer again");
                        this.currentLevel.stopLevel();
                        this.levelNumber += 1;
                        this.currentLevel = this.levels[this.levelNumber];
                        this.gunController.resetBullets();
                        this.resetCamera();
                       
                        this.countdown.startCountdown();
                    }


                }
            }

        }
    }

    private void resetCamera() {
        //this.playerCamera.camera.Reset();
        this.playerCamera.transform.position = this.originalCameraTransform.position;
        this.playerCamera.transform.rotation = this.originalCameraTransform.rotation; 
        subsystem.TryRecenter();
        
        
    }
    }


