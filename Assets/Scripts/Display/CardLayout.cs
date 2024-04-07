using System.Collections.Generic;
using BattleSystem;
using DrawXXL;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Display
{
    public class CardLayout : MonoBehaviour
    {
        [SerializeField] private CardDisplay cardDisplayPrefab;
        [SerializeField] private Transform cardParent;

        [OnValueChanged("RepositionCards"), SerializeField] private int sortingOrder;
        [OnValueChanged("RepositionCards"), SerializeField] private Vector2 cardSpacing = new(0.15f, 0);
        [OnValueChanged("RepositionCards"), SerializeField] private Vector2 padding = new(2, 0);
        [OnValueChanged("RepositionCards"), SerializeField] private Vector2 middleCardOffset = new(0, -0.1f);
        [OnValueChanged("RepositionCards"), SerializeField] private float cardRotation = -2;
        [OnValueChanged("RepositionCards"), SerializeField] private bool wrapCards;
        [OnValueChanged("RepositionCards"), SerializeField, ShowIf("wrapCards")] private int columnCount = 3;

        private readonly List<CardDisplay> _cardDisplays = new();
        private Rect _size;
        private Vector2 CardSize => CardDisplay.CardSize;

        private void OnDrawGizmos()
        {
            DrawShapes.Rectangle(_size);
        }

        public void Show(List<Card> cards)
        {
            foreach (Transform child in cardParent) Destroy(child.gameObject);
            _cardDisplays.Clear();

            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var cardDisplay = Instantiate(cardDisplayPrefab, cardParent);
                cardDisplay.ShowCard(card);
                cardDisplay.SetOrder(sortingOrder);
                _cardDisplays.Add(cardDisplay);
            }

            RepositionCards();
        }

        public int GetIndexOf(CardDisplay draggedCard)
        {
            return _cardDisplays.IndexOf(draggedCard);
        }

        public void AddCard(CardDisplay card, int index)
        {
            _cardDisplays.Insert(index, card);
            card.transform.SetParent(cardParent);
            RepositionCards();
        }

        public void RemoveCard(CardDisplay card)
        {
            _cardDisplays.Remove(card);
            card.transform.SetParent(transform.parent);
            RepositionCards();
        }

        [Button]
        public void RepositionCards()
        {
            // Calculate half offset for positioning cards
            var cardCount = _cardDisplays.Count;
            var columns = wrapCards ? columnCount : cardCount;
            var rows = Mathf.Ceil((float)cardCount / columns);
            var gridSize = new Vector2(columns, rows);
            var spacingFactor = 2 * (gridSize - Vector2.one);
            var halfRotation = cardRotation * (columnCount - 1) / 2;

            // Position each card
            for (var row = 0; row < rows; row++)
            {
                var cardSpacingTotal = cardSpacing * spacingFactor;
                var cardsThisRow = Mathf.Min(columns, cardCount - row * columns);
                var cardSizeTotal = CardSize * (new Vector2(cardsThisRow, gridSize.y) - Vector2.one);
                var halfOffset = (cardSpacingTotal + cardSizeTotal) / 2;
                for (var column = 0; column < columns; column++)
                {
                    var cardIndex = column + row * columns;
                    if (cardIndex >= cardCount) break;
                    var cardDisplay = _cardDisplays[cardIndex];
                    var cardPosition = new Vector2(column, rows - row - 1);
                    var position = (cardSpacing * 2 + CardSize) * cardPosition
                                   - halfOffset
                                   + middleCardOffset * Mathf.Pow(Mathf.Abs(column - (cardsThisRow - 1) / 2f), 2);
                    cardDisplay.transform.localPosition = position;
                    cardDisplay.transform.rotation = Quaternion.Euler(0, 0, cardRotation * column - halfRotation);
                    cardDisplay.SetOrder(cardIndex);
                }
            }

            // Calculate size of the container
            var size = cardSpacing * spacingFactor + CardSize * gridSize + padding * 2;
            var containerPosition = (Vector2)transform.position - size / 2;
            _size = new Rect(containerPosition.x, containerPosition.y, size.x, size.y);
        }

        public bool IsMouseOver()
        {
            var mouse = Input.mousePosition;
            var worldMouse = Game.Camera.ScreenToWorldPoint(mouse);
            return _size.Contains(worldMouse);
        }
    }
}