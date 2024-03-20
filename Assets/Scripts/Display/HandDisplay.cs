using System.Collections.Generic;
using DrawXXL;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Display
{
    public class HandDisplay : MonoBehaviour
    {
        [SerializeField] private CardDisplay cardDisplayPrefab;
        [SerializeField] private Transform cardParent;

        [SerializeField] private Vector2 cardSpacing;
        [SerializeField] private Vector2 padding;
        [SerializeField] private Vector2 cardSizeMultiplier;

        private readonly List<CardDisplay> _cardDisplays = new();
        private Player _player;
        [ShowInInspector] private Rect _size;
        private Vector2 CardSize => CardDisplay.CardSize;

        private void OnDrawGizmos()
        {
            DrawShapes.Rectangle(_size);
        }

        public void Show(Player player)
        {
            foreach (Transform child in cardParent) Destroy(child.gameObject);
            _cardDisplays.Clear();
            _player = player;

            for (var i = 0; i < player.Hand.Count; i++)
            {
                var card = player.Hand[i];
                var cardDisplay = Instantiate(cardDisplayPrefab, cardParent);
                cardDisplay.ShowCard(card);
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

            // Position each card
            for (var i = 0; i < cardCount; i++)
            {
                var cardDisplay = _cardDisplays[i];
                var position = (cardSpacing * 2 + CardSize * cardSizeMultiplier) * i - halfOffset;
                cardDisplay.transform.localPosition = position;
            }

            // Calculate size of the container
            var containerWidth = cardSpacing.x * spacingFactor + CardSize.x * cardCount;
            var containerHeight = CardSize.y;
            var containerPosition = (Vector2)transform.position - halfOffset - CardSize / 2 - padding;
            // var containerX = transform.position.x - halfOffset.x - CardSize.x / 2 - padding.x;
            // var containerY = transform.position.y - halfOffset.y - CardSize.y / 2 - padding.y;
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