using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private CardBlueprint[] cardBlueprints;
    public Battle Battle { get; private set; }
    public static Camera Camera { get; private set; }


    private void Awake()
    {
        Camera = Camera.main;
        var cardFactory = new CardFactory(cardBlueprints);
        var player = new Player(100, new List<Card>
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
        Battle = new Battle(cardFactory, player);
        Battle.Start();
    }
}