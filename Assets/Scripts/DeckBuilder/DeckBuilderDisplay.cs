using BattleSystem;
using Display;
using UnityEngine;

public class DeckBuilderDisplay : MonoBehaviour
{
    [SerializeField] private CardLayout collectionView;
    [SerializeField] private CardLayout deckView;
    [SerializeField] private CardDisplay cardDisplayPrefab;
    private Deck _deck;

    private void Start()
    {
        var cardFactory = GlobalState.GetCardFactory();
        collectionView.Show(cardFactory.GetSearch());

        _deck = GlobalState.GetDeck();
        deckView.Show(_deck.CreatePile());
        deckView.Sort();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (deckView.IsPointOver(mousePosition))
            {
                var cardDisplay = deckView.GetCardDisplayOver(mousePosition);
                if (cardDisplay != null)
                {
                    var card = cardDisplay.Card;
                    if (_deck.RemoveCard(card))
                    {
                        deckView.RemoveCard(cardDisplay);
                        Destroy(cardDisplay.gameObject);
                    }
                }
            } else if (collectionView.IsPointOver(mousePosition))
            {
                var cardDisplay = collectionView.GetCardDisplayOver(mousePosition);
                if (cardDisplay != null)
                {
                    var card = cardDisplay.Card;
                    _deck.AddCard(card.Copy());
                    var newCardDisplay = Instantiate(cardDisplayPrefab, deckView.transform);
                    newCardDisplay.ShowCard(card);
                    deckView.AddCard(newCardDisplay);
                    deckView.Sort();
                }
            }
        }
    }

    public void BackToMainMenu()
    {
        GlobalState.SaveDeck(_deck);
        MainMenuStart.LaodMainMenu();
    }
}