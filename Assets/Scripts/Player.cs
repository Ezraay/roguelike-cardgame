using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public readonly Entity Entity;
    public readonly List<Card> Deck = new();
    public readonly List<Card> Discard = new();
    public readonly List<Card> Hand = new();

    public Player(int health, List<Card> deck)
    {
        Entity = new Entity(health);
        Deck.AddRange(deck);
        Shuffle(Deck);
    }

    public int Energy { get; private set; }
    public int EnergyPerTurn { get; } = 3;

    public void StartTurn()
    {
        Entity.OnStartTurn();
        Energy = EnergyPerTurn;

        // Draw 5
        for (var i = 0; i < 5; i++) DrawCard();
    }

    public void EndTurn()
    {
        // Discard hand
        Discard.AddRange(Hand);
        Hand.Clear();
    }

    public void DrawCard()
    {
        if (Deck.Count == 0)
        {
            // Shuffle discard into deck
            Deck.AddRange(Discard);
            Shuffle(Deck);
            Discard.Clear();
        }

        if (Deck.Count == 0)
            // No cards to draw
            return;

        var card = Deck[0];
        Deck.RemoveAt(0);
        Hand.Add(card);
    }

    public void UseEnergy(int energy)
    {
        Energy = Mathf.Max(0, Energy - energy);
    }

    private void Shuffle(List<Card> cards)
    {
        // Fisher-Yates shuffle
        for (var i = cards.Count - 1; i > 0; i--)
        {
            var j = Random.Range(0, i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }
}