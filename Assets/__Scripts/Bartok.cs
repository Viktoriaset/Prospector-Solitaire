using BartokGame;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bartok : MonoBehaviour
{
    static public Bartok S;

    [Header("Set in inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCeneter = Vector3.zero;
    public float hadFanDegrees = 10f;
    public int numStartingCards = 7;
    public float drawTimeStagger = 0.2f;

    [Header("Set dynamically")]
    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> discardPile;
    public List<Player> players;
    public CardBartok targetCard;

    private BartokLayout layout;
    private Transform layoutAnchor;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
        Deck.Shuffle(ref deck.cards);

        layout = GetComponent<BartokLayout>();
        layout.ReadLayout(layoutXML.text);

        drawPile = UpgradeCardList(deck.cards);
        LayoutGame();
    }

    private List<CardBartok> UpgradeCardList(List<Card> cards)
    {
        List<CardBartok> lCB = new List<CardBartok>();
        foreach (Card tCD in cards)
        {
            lCB.Add(tCD as CardBartok);
        }
        return lCB;
    }

    private void LayoutGame()
    {
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCeneter;
        }

        ArrangeDrawPile();

        Player pl;
        players = new List<Player>();
        foreach(BartokGame.SlotDef tSD in layout.slotDefs)
        {
            pl = new Player();
            pl.handSlotDef = tSD;
            players.Add(pl);
            pl.playerNum = tSD.player;
        }
        players[0].type = PlayerType.human;

        CardBartok tCB;
        for (int i = 0; i <numStartingCards; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                tCB = Draw();
                tCB.timeStart = Time.time + drawTimeStagger * (i * 4 + j);
                players[(j + 1) % 4].AddCard(tCB);
            }
        }

        Invoke("DrawFirstTarget", drawTimeStagger * (numStartingCards * 4 + 4));
    }

    public void ArrangeDrawPile()
    {
        CardBartok tCB;
        for (int i = 0; i < drawPile.Count; i++)
        {
            tCB = drawPile[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.drawPile.pos;
            tCB.faceUp = false;
            tCB.SetSortingLayerName(layout.drawPile.layerName);
            tCB.SetSortOrder(-i * 4);
            tCB.state = CBState.drawpile;
        }
    }

    public void DrawFirstTarget()
    {
        CardBartok tCB = MoveToTarget(Draw());
    }

    public CardBartok MoveToTarget(CardBartok card)
    {
        card.timeStart = 0;
        card.MoveTo(layout.discardPile.pos + Vector3.back);
        card.state = CBState.target;
        card.faceUp = true;

        card.SetSortingLayerName("10");
        card.eventualSortLayer = layout.target.layerName;
        if (targetCard != null)
        {
            MoveToDiscard(targetCard);
        }

        return card;
    }

    public CardBartok MoveToDiscard(CardBartok card)
    {
        card.state = CBState.discard;
        discardPile.Add(card);
        card.SetSortingLayerName(layout.discardPile.layerName);
        card.SetSortOrder(discardPile.Count * 4);
        card.transform.localPosition = layout.discardPile.pos + Vector3.back / 2;

        return card;
    }

    public CardBartok Draw()
    {
        CardBartok cb = drawPile[0];
        drawPile.RemoveAt(0);
        return cb;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            players[0].AddCard(Draw());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            players[1].AddCard(Draw());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            players[2].AddCard(Draw());
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            players[3].AddCard(Draw());
        }
    }
}
