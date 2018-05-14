using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameHandler : NetworkBehaviour
{

    [SyncVar]
    float timeLeft;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    // Update is called once per frame
    void Update()
    {

    }


    //SET ROUND TIME LIMIT HERE!
    public IEnumerator StartCountdown(float timeLimit = 10)
    {
        timeLeft = timeLimit;
        while (timeLeft > 0)
        {
            Debug.Log("Countdown: " + timeLeft);
            yield return new WaitForSeconds(1.0f);
            timeLeft--;
        }
    }
}
