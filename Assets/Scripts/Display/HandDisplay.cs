using System.Collections.Generic;
using DrawXXL;
using UnityEngine;

namespace Display
{
    public class HandDisplay : MonoBehaviour
    {
        [SerializeField] private CardDisplay cardDisplayPrefab;
        [SerializeField] private Transform cardParent;

        [SerializeField] private int sortingOrder;
        [SerializeField] private Vector2 cardSpacing;
        [SerializeField] private Vector2 padding;
        [SerializeField] private Vector2 cardSizeMultiplier;
        [SerializeField] private Vector2 middleCardOffset;

        [SerializeField] private float cardRotation;

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

        public void RepositionCards()
        {
            // Calculate half offset for positioning cards
            var cardCount = _cardDisplays.Count;
            var spacingFactor = 2 * (cardCount - 1);
            var cardSpacingTotal = cardSpacing * spacingFactor;
            var cardSizeTotal = CardSize * cardSizeMultiplier * (cardCount - 1);
            var halfOffset = (cardSpacingTotal + cardSizeTotal) / 2;
            var halfRotation = cardRotation * (cardCount - 1) / 2;

            // Position each card
            for (var i = 0; i < cardCount; i++)
            {
                var cardDisplay = _cardDisplays[i];
                var position = (cardSpacing * 2 + CardSize * cardSizeMultiplier) * i - halfOffset +
                               middleCardOffset * Mathf.Pow(Mathf.Abs((i) - (cardCount-1) / 2f), 2);
                cardDisplay.transform.localPosition = position;
                cardDisplay.transform.rotation = Quaternion.Euler(0, 0, cardRotation * i - halfRotation);
            }

            // Calculate size of the container
            var containerWidth = cardSpacing.x * spacingFactor + CardSize.x * cardCount;
            var containerHeight = CardSize.y;
            var containerPosition = (Vector2)transform.position - halfOffset - CardSize / 2 - padding;
            _size = new Rect(containerPosition.x, containerPosition.y, containerWidth + padding.x * 2, containerHeight + padding.y * 2);
        }

        public bool IsMouseOver()
        {
            var mouse = Input.mousePosition;
            var worldMouse = Camera.main.ScreenToWorldPoint(mouse);
            return _size.Contains(worldMouse);
        }
    }
}