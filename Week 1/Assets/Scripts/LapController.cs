using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using Photon.Realtime;
//using ExitGames.Client.Photon;

public class LapController : MonoBehaviourPun
{

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if(other.gameObject.tag == "LapTrigger")
        {
            //We passed through one of the lap triggers
            //To Do:
            //Send an update to refresh the car standings.
            Debug.Log("Lap trigger crossed. Standings UI to be refreshed");
        }
        else if(other.gameObject.tag == "FinishTrigger")
        {
            Debug.Log("Car reached the finish point...");
            EndRace();
        }
    }

    /// <summary>
    /// Called via OnTriggerEnter when a car enters the finish trigger.
    /// </summary>
    void EndRace()
    {
        //Step 1: When the race ends detach the camera from the car
        //Caution: Only detach camera for your own instance's car!

        //Step 2: Disable the car movement script, so that the player controlling the car can no longer
        //control, steer, or move once it crosses the finish line.

        /**** TO BE COMPLETED BY EACH GROUP ****/
    }
}
