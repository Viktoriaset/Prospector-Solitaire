using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlayerType
{
    human,
    ai
}

[Serializable]
public class Player
{
    public PlayerType type = PlayerType.ai;
    public int playerNum;
    public BartokGame.SlotDef handSlotDef;
    public List<CardBartok> hand = new List<CardBartok>();

    public CardBartok AddCard(CardBartok card)
    {
        hand.Add(card);

        if (type == PlayerType.human)
        {
            CardBartok[] cards = hand.ToArray();
            cards = cards.OrderBy(cd => cd.rank).ToArray();

            hand = new List<CardBartok>(cards);
        }

        card.SetSortingLayerName("10");
        card.eventualSortLayer = handSlotDef.layerName;

        FanHand();
        return card;
    }

    public CardBartok RemoveCard(CardBartok card)
    {
        if (hand == null || !hand.Contains(card)) return null;
        hand.Remove(card);
        FanHand();
        return card;
    }

    public void FanHand()
    {
        float startRot = 0;
        startRot = handSlotDef.rot;
        if (hand.Count > 1)
        {
            startRot += Bartok.S.hadFanDegrees * (hand.Count - 1) / 2;
        }

        Vector3 pos;
        float rot;
        Quaternion rotQ;
        for (int i = 0; i < hand.Count; i++)
        {
            rot = startRot - Bartok.S.hadFanDegrees * i;
            rotQ = Quaternion.Euler(0, 0, rot);

            pos = Vector3.up * CardBartok.CARD_HEIGHT / 2f;

            pos = rotQ * pos;

            pos += handSlotDef.pos;
            pos.z = -0.5f * i;

            if (Bartok.S.phase != TurhPhase.idle)
            {
                hand[i].timeStart = 0;
            }

            hand[i].MoveTo(pos, rotQ);
            hand[i].state = CBState.toHand;
            hand[i].faceUp = (type == PlayerType.human);
            hand[i].eventualSortOrder = i * 4;
        }
    }

    public void TakeTurn()
    {
        Utils.tr("Player.TakeTurn()");

        if (type == PlayerType.human) return;

        Bartok.S.phase = TurhPhase.waiting;

        CardBartok card;
        List<CardBartok> validCards = new List<CardBartok>();
        foreach(CardBartok tCard in hand)
        {
            if (Bartok.S.ValidPlay(tCard))
            {
                validCards.Add(tCard);
            }
        }

        if (validCards.Count == 0)
        {
            card = AddCard(Bartok.S.Draw());
            card.callbackPlayer = this;
            return;
        }

        card = validCards[Random.Range(0, validCards.Count)];
        RemoveCard(card);
        Bartok.S.MoveToTarget(card);
        card.callbackPlayer = this;
    }

    public void CBCallback(CardBartok card)
    {
        Bartok.S.PassTurn();
    }
}
