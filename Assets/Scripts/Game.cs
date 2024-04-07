using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private CardBlueprint[] cardBlueprints;
    public Battle Battle { get; private set; }
    public static Camera Camera { get; private set; }
    private Level _level;


    private void Awake()
    {
        Camera = Camera.main;

        var cardFactory = new CardFactory(cardBlueprints);
        var deck = new Deck(new List<Card>
        {
            cardFactory.CreateCard("strike"),
            cardFactory.CreateCard("strike"),
            cardFactory.CreateCard("strike"),
            cardFactory.CreateCard("strike"),
            cardFactory.CreateCard("strike"),
            cardFactory.CreateCard("defend"),
            cardFactory.CreateCard("defend"),
            cardFactory.CreateCard("defend"),
            cardFactory.CreateCard("defend"),
            cardFactory.CreateCard("defend")
        });
        var player = new Player(100, deck);
        Battle = new Battle(cardFactory, player);
        _level = new Level(new Encounter(new Encounter()));
    }

    private void Start()
    {
        Battle.StartEncounter(_level.CurrentEncounter);
    }

    public void StartNextEncounter()
    {
        if (_level.CanAdvance())
        {
            _level.Advance();
            Battle.StartEncounter(_level.CurrentEncounter);
        }
    }
}