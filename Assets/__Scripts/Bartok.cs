using BartokGame;
using System.Collections.Generic;
using UnityEngine;

public enum TurhPhase
{
    idle,
    pre,
    waiting,
    post,
    gameOver
}

public class Bartok : MonoBehaviour
{
    static public Bartok S;
    static public Player CURRENT_PLAYER;

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
    public TurhPhase phase = TurhPhase.idle;

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
        foreach (BartokGame.SlotDef tSD in layout.slotDefs)
        {
            pl = new Player();
            pl.handSlotDef = tSD;
            players.Add(pl);
            pl.playerNum = tSD.player;
        }
        players[0].type = PlayerType.human;

        CardBartok tCB;
        for (int i = 0; i < numStartingCards; i++)
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
        CardBartok card = MoveToTarget(Draw());
        card.reportFinishTo = gameObject;
    }

    public CardBartok MoveToTarget(CardBartok card)
    {
        card.timeStart = 0;
        card.MoveTo(layout.discardPile.pos + Vector3.back);
        card.state = CBState.toTarget;
        card.faceUp = true;

        card.SetSortingLayerName("10");
        card.eventualSortLayer = layout.target.layerName;
        if (targetCard != null)
        {
            MoveToDiscard(targetCard);
        }

        targetCard = card;

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

    public void CBCallback(CardBartok card)
    {
        Utils.tr("Bartok:Callback()", card.name);
        StartGame();
    }

    public void StartGame()
    {
        print("Start game");
        PassTurn(1);
    }

    public void PassTurn(int num = -1)
    {
        if (num == -1)
        {
            int ndx = players.IndexOf(CURRENT_PLAYER);
            num = (ndx + 1) % 4;
        }

        int lastPlayerNum = -1;
        if (CURRENT_PLAYER != null)
        {
            lastPlayerNum = CURRENT_PLAYER.playerNum;
        }
        CURRENT_PLAYER = players[num];
        phase = TurhPhase.pre;

        //CURRENT_PLAYER.TakeTurn();
    }

    public bool ValidPlay(CardBartok card)
    {
        if (card.rank == targetCard.rank) return true;

        if (card.suit == targetCard.suit) return true;

        return false;
    }

    public CardBartok Draw()
    {
        CardBartok cb = drawPile[0];
        drawPile.RemoveAt(0);
        return cb;
    }
}
