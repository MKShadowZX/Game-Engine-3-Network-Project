using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStandingUIItem : MonoBehaviour
{
    public Text playerNameText;
    public Text playerRankText;
    public Text playerTimeText;

   public void UpdateInfo(string plName, int plRank, string plTime, bool isPlayerYou)
    {
        playerNameText.text = plName;
        playerRankText.text = plRank.ToString();
        playerTimeText.text = plTime;

        if (isPlayerYou)
        {
            playerNameText.color = Color.red;
            playerRankText.color = Color.red;
            playerTimeText.color = Color.red;
        }
        //TO - DO:: remember to test the game and update/add code here to fix the problems you will identify

    }

}
