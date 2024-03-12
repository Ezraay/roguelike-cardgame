using System;
using UnityEngine;

namespace Display
{
    public class BattleDisplay : MonoBehaviour
    {
        [SerializeField] private Game game;
        [SerializeField] private EntityDisplay entityDisplayPrefab;
        [SerializeField] private Vector2 enemyOffset;
        [SerializeField] private Transform playerDisplayParent;
        [SerializeField] private Transform enemyDisplayParent;
        [SerializeField] private HandDisplay handDisplay;
        [SerializeField] private EnergyDisplay energyDisplay;
        private CardDisplay _draggedCard;
        private int _draggedCardIndex;

        private void Start()
        {
            // Show player and enemy health
            var playerDisplay = Instantiate(entityDisplayPrefab, playerDisplayParent);
            playerDisplay.Show(game.Battle.Player.Entity);

            var halfOffset = enemyOffset * (game.Battle.Enemies.Count-1) / 2;
            for (var i = 0; i < game.Battle.Enemies.Count; i++)
            {
                var enemy = game.Battle.Enemies[i];
                var enemyDisplay = Instantiate(entityDisplayPrefab, enemyDisplayParent);
                enemyDisplay.Show(enemy);
                enemyDisplay.transform.localPosition = enemyOffset * i - halfOffset;
            }

            // TODO Show enemy intents
            
            // Show player cards
            handDisplay.Show(game.Battle.Player);
            
            // TODO Show player deck and discard
            
            // Show player energy
            energyDisplay.Show(game.Battle.Player);
            
            // TODO Allow player to play card
            
            // TODO Allow player to target card

            // TODO Show animated card actions
            
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Raycast for card
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                    _draggedCard = hit.collider.GetComponent<CardDisplay>();
                    _draggedCardIndex = handDisplay.GetIndexOf(_draggedCard);
                    handDisplay.RemoveCard(_draggedCard);
                }
            }

            if (Input.GetMouseButtonUp(0) && _draggedCard != null)
            {
                handDisplay.AddCard(_draggedCard, _draggedCardIndex);
                _draggedCard = null;
            }

            if (_draggedCard != null)
            {
                var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0;
                _draggedCard.transform.position = position;
            }
            
            if (handDisplay.IsMouseOver())
                Debug.Log("A");
        }

        public void EndTurn()
        {
            // Allow player to end turn
            game.Battle.EndTurn();
        }
    }
}