using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
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

        if (Bartok.CURRENT_PLAYER == null) return;
        if (Bartok.CURRENT_PLAYER.type == PlayerType.human)
        {
            text.text = "You Won!";
        } else
        {
            text.text = "Game Over!";
        }
    }
}
