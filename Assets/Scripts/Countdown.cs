using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.XR;
using Valve.VR;

public class Countdown : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public TextMeshProUGUI countdownText;

    float remainingTime = 3;

    public float waitTime = 3;

    public bool disabled = true;

    public bool finished = false;

    public AudioSource source;

    public AudioClip countdownTimer;
    public AudioClip countdownGunshot;

    public SpawnProjectiles game;

    public bool countdownEnabled = false;

    public bool regularTimerEnabled = false;

    private GameObject hand;

    String prevNumber = "";


    void Start()
    {
        countdownText.fontSize = 10;
        countdownText.text = "Press A to start";
        hand =  GameObject.FindGameObjectWithTag("hand");
    }

    // Update is called once per frame
    void Update()
    {
        if (!disabled)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                int time = (int)Math.Ceiling(remainingTime);
                if (countdownEnabled && time <= waitTime)
                {
                    hand.SetActive(false);
                    countdownText.text = time.ToString();
                    if (prevNumber != countdownText.text)
                    {
                        prevNumber = countdownText.text;
                        playSnap();
                    }
                }

            }
            else if (remainingTime <= 0)
            {
                if (countdownEnabled) {
                    hand.SetActive(true);
                    playCountdownGunshot();
                    prevNumber = "";
                    countdownEnabled = false;
                }
                else if (regularTimerEnabled) {
                    regularTimerEnabled = false;
                }              
                disabled = true;
                finished = true;
                countdownText.text = "";               
            }
        }
    }

    public void startCountdown()
    {
        this.countdownEnabled = true;
        countdownText.text = "";
        prevNumber = "";
        this.countdownText.fontSize = 32;
        this.countdownText.color = Color.white;
        this.remainingTime = 3;
        this.waitTime = 3;
        disabled = false;
        finished = false;
    }

    public void startRegularTimer(int time) {
        this.regularTimerEnabled = true;
        this.countdownText.fontSize = 10;
        this.remainingTime = time;
        this.waitTime = time;
        disabled = false;
        finished = false;
    }

    void playSnap()
    {
        source.clip = countdownTimer;
        source.Play();
    }

    void playCountdownGunshot()
    {
        source.clip = countdownGunshot;
        source.Play();
    }

    public void setText(String text, int fontSize)
    {
        this.countdownText.fontSize = fontSize;
        this.countdownText.text = text;
    }

    public void showLevelCompleted()
    { 
        this.countdownText.color = Color.green;
        this.countdownText.fontSize = 12;
        this.countdownText.text = "Level Completed!";
    }

    public void showLevelFailed()
    {
        this.countdownText.color = Color.red;
        this.countdownText.fontSize = 12;
        this.countdownText.text = "Level Failed! Press A to restart";
    }

    public void setTimer(int t) {
        this.remainingTime = t;
        this.waitTime = t;
    }

}
