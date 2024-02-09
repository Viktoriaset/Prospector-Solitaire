using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundResultsUI : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
        text.text = "";
    }

    private void Update()
    {
        if (Bartok.S.phase != TurhPhase.gameOver)
        {
            text.text = "";
            return;
        }

        Player cP = Bartok.CURRENT_PLAYER;
        if (cP == null || cP.type == PlayerType.human)
        {
            text.text = "";
        } else
        {
            text.text = "Player " + cP.playerNum + " won!";
        }
    }
}
