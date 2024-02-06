using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prospector : MonoBehaviour
{
    static public Prospector S;

    [Header("Set in inspector")]
    public TextAsset deckXML;

    [Header("Set dynamically")]
    public Deck deck;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
    }
}
