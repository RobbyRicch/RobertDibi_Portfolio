using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEnergyTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtEnergyTimer; //sm 00s
    //[SerializeField] private int energyNumber;

    private float counterTime;
    private ulong lastTime;
    public float msToWait = 5000.0f;//5 secs
    public float timeRemaining;
    
    //private System.DateTime startTime;
    /*
    
    [SerializeField] private GameObject chestPanel;
    

   */
    private void Awake()
    {
       
    }

    DateTime currentDate;
    DateTime oldDate;
    private void Start()
    {
        counterTime = 60f;
        timeRemaining = counterTime;

        if (PlayerPrefs.HasKey("PlayerEnergyTimer"))
            timeRemaining = float.Parse(PlayerPrefs.GetString("PlayerEnergyTimer"));

        //var diffInSeconds = (dateTime1 - dateTime2).TotalSeconds;
        //  startTime = System.DateTime.UtcNow;
        //  System.TimeSpan ts = System.DateTime.UtcNow - startTime;
        // Debug.Log("JJJ" + ts.Seconds.ToString());
        if (PlayerPrefs.GetString("LastTimer") != "")
            lastTime = ulong.Parse(PlayerPrefs.GetString("LastTimer"));
        else
            lastTime = 0;
        if (PlayerPrefs.HasKey("sysString"))
        {

            //Store the current time when it starts
            currentDate = System.DateTime.Now;

            //Grab the old time from the player prefs as a long
            long temp = Convert.ToInt64(PlayerPrefs.GetString("sysString"));

            //Convert the old time from binary to a DateTime variable
            DateTime oldDate = DateTime.FromBinary(temp);
           // print("oldDate: " + oldDate);

            //Use the Subtract method and store the result as a timespan variable
            TimeSpan difference = currentDate.Subtract(oldDate);
            //print("Difference: " + difference);
            //float timeGap = (float)difference.TotalSeconds - timeRemaining;
            //print("timeGap: " + timeGap);
            //var diffDouble = difference.TotalMinutes;
            //print("diffDouble: " + diffDouble);
            //print("timeGap: " + timeGap/60f);
            double minutes = Math.Floor(difference.TotalSeconds / 60);
            double seconds = Math.Ceiling(difference.TotalSeconds % 60);
            Debug.Log("minutes:" + minutes + "|seconds:" + (seconds+timeRemaining));
            String min = ""+minutes;
            Actions.EnergyNumberUpdate(int.Parse( min));
            if(((float)seconds-timeRemaining) >0)
                timeRemaining = ((float)seconds- timeRemaining);
            else
                timeRemaining = counterTime;
            //SetTheTimer();
        }
        else
            timeRemaining = counterTime;

    }

    public void ReSetCounterTimer()
    {
        timeRemaining = counterTime;
    }
    void OnApplicationQuit()
    {
        string str = PlayerPrefs.GetString("deletekeyall");
        if (str == "1")
        {
            
            //PlayerPrefs.DeleteKey("sysString");
           // PlayerPrefs.DeleteKey("PlayerEnergyTimer");
           // PlayerPrefs.DeleteKey("LastTimer");
        }
        else
        {
            //Savee the current system time as a string in the player prefs class
            PlayerPrefs.SetString("sysString", System.DateTime.Now.ToBinary().ToString());
            //print("Saving this date to prefs: " + System.DateTime.Now);

            PlayerPrefs.SetString("PlayerEnergyTimer", "" + timeRemaining);
            //Debug.Log("PlayerEnergyTimer: " + timeRemaining.ToString());
            //now computer time
            lastTime = (ulong)DateTime.Now.Ticks;
            // Debug.Log(lastChestOpen.ToString());//(ulong)
            PlayerPrefs.SetString("LastTimer", lastTime.ToString());
            //Debug.Log(DateTime.Now.ToFileTime() + "lasttime: " +lastTime.ToString());
        }
    }

    private void OnDisable()
    {
       
        
    }

    void Update()
    {
        
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            SetTimerText();
            if (timeRemaining <= 0)
            {
                //energyNumber++;
                Actions.EnergyNumberUpdate(+1);
                timeRemaining = counterTime;
                txtEnergyTimer.text = "" + timeRemaining;
            }
        }
    }

    private void SetTimerText()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        if(seconds<10)
            txtEnergyTimer.text = minutes + ":0" + seconds;
        else
            txtEnergyTimer.text = minutes + ":" + seconds;

    }

   /* public void SetEnergyNumber(int _deEnergy)
    {
        energyNumber = _deEnergy;
    }
   */
    public void SetTheTimer()
    {
        //Set the Timer
        ulong diff = ((ulong)DateTime.Now.Ticks - lastTime);
        ulong m = diff / TimeSpan.TicksPerMillisecond; //3000 ms = 3 sec

        float secondsLeft = (float)(msToWait - m) / 1000.0f;//3000.0f-m
        
        string r = "";
        //Hours
        //r += ((int)secondsLeft / 3600).ToString() + "h ";
        secondsLeft -= ((int)secondsLeft / 3600) * 3600;
        //Minutes
        r += ((int)secondsLeft / 60).ToString("00") + "m ";
        //Seconds
        r += (secondsLeft % 60).ToString("00") + "s"; 
         Debug.Log("|"+ r);
    

    }
}
