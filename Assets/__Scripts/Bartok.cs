using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bartok : MonoBehaviour
{
    static public Bartok S;

    [Header("Set in inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCeneter = Vector3.zero;

    [Header("Set dynamically")]
    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> dscardPile;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
        Deck.Shuffle(ref deck.cards);
    }
}
