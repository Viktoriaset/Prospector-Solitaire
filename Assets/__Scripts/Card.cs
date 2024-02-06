using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Set dynamically")]
    public string suit;
    public int rank;
    public Color color = Color.black;
    public string colS = "Black";

    public List<GameObject> decoGOs = new List<GameObject>();
    public List<GameObject> pipGOs = new List<GameObject>();

    public GameObject back;

    public CardDifinition def;

    public bool faceUp
    {
        get
        {
            return !back.activeSelf;
        }
        set
        {
                back.SetActive(!value);
        }
    }
}

[Serializable]
public class Decorator
{
    public string type;
    public Vector3 loc;
    public bool flip = false;
    public float scale = 1f;
}

[Serializable]
public class CardDifinition
{
    public string face;
    public int rank;
    public List<Decorator> pips = new List<Decorator>();
}   
