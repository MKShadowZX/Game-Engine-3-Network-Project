using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartCountdownTimer : MonoBehaviourPun
{

    public float countdownStartValue = 3.0f;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = countdownStartValue;   
    }

    private void Update()
    {
        //If this is not materclient, then quit the function.
        if (!PhotonNetwork.IsMasterClient) return;

        if (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            photonView.RPC("UpdateTimerText", RpcTarget.All, timer);
        }
        else
        {
            photonView.RPC("StartRace", RpcTarget.All);
        }

    }

    [PunRPC]
    void StartRace()
    {
        //Call 'EnableMovement'
        GetComponent<CarMovementController>().EnableMovement();
        GameManager.instance.EnableRaceTimer();
    }

    /// <summary>
    /// RPC = Remote Procedure Call
    /// When you mark a function with 'PunRPC', you denote that this function
    /// can be called remotely from another call.
    /// </summary>
    /// <param name="time"></param>
    [PunRPC]
    void UpdateTimerText(float time)
    {
        if (time > 0.0f)
            GameManager.instance.countdownTimerText.text = Mathf.RoundToInt(time).ToString();
        else
        {
            GameManager.instance.countdownTimerText.transform.parent.gameObject.SetActive(false);
        }
            
    }

}
