using System.Collections.Generic;
using DrawXXL;
using Effects;
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
        private bool _draggedCardActive;
        private CardDisplay _draggedCard;
        private List<EntityDisplay> _enemyDisplays = new();
        private int _draggedCardIndex;

        private void Start()
        {
            // Show player and enemy health
            var playerDisplay = Instantiate(entityDisplayPrefab, playerDisplayParent);
            playerDisplay.Show(game.Battle.Player.Entity);

            var halfOffset = enemyOffset * (game.Battle.Enemies.Count - 1) / 2;
            _enemyDisplays.Clear();
            for (var i = 0; i < game.Battle.Enemies.Count; i++)
            {
                var enemy = game.Battle.Enemies[i];
                Vector3 position = enemyOffset * i - halfOffset;
                var enemyDisplay = Instantiate(entityDisplayPrefab, enemyDisplayParent.position + position,
                    Quaternion.identity, enemyDisplayParent);
                enemyDisplay.Show(enemy);
                _enemyDisplays.Add(enemyDisplay);

                enemy.OnDeath += () =>
                {
                    Destroy(enemyDisplay.gameObject);
                    _enemyDisplays.Remove(enemyDisplay);
                };
            }

            // TODO Show enemy intents

            // Show player cards
            handDisplay.Show(game.Battle.Player);

            // TODO Show player deck and discard

            // Show player energy
            energyDisplay.Show(game.Battle.Player);

            // TODO Show animated card actions

            // TODO Destroy dead enemies

            // TODO Game over screen
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
                    _draggedCardActive = false;
                }
            }

            if (_draggedCard != null)
            {
                if (handDisplay.IsMouseOver() && _draggedCardActive)
                {
                    _draggedCardActive = false;
                    handDisplay.AddCard(_draggedCard, _draggedCardIndex);
                    _draggedCard.gameObject.SetActive(true);
                }
                else if (!handDisplay.IsMouseOver() && !_draggedCardActive)
                {
                    _draggedCardActive = true;
                    handDisplay.RemoveCard(_draggedCard);

                    if (_draggedCard.Card.TargetingType == TargetingType.Enemy)
                        _draggedCard.gameObject.SetActive(false);
                }

                EntityDisplay selectedEnemy = null;
                if (_draggedCard.Card.TargetingType == TargetingType.Enemy)
                    foreach (var enemyDisplay in _enemyDisplays)
                        if (enemyDisplay.IsMouseOver())
                        {
                            selectedEnemy = enemyDisplay;
                            break;
                        }

                if (_draggedCardActive && _draggedCard.Card.TargetingType == TargetingType.Enemy)
                {
                    if (selectedEnemy != null)
                        DrawBasics2D.Vector(handDisplay.transform.position, selectedEnemy.transform.position);
                    else
                        DrawBasics2D.Vector(handDisplay.transform.position,
                            Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }

                var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0;
                _draggedCard.transform.position = position;

                if (Input.GetMouseButtonUp(0) && _draggedCard != null)
                {
                    var returnCard = true;
                    if (!handDisplay.IsMouseOver())
                    {
                        var card = _draggedCard.Card;
                        if (card.TargetingType == TargetingType.Enemy)
                        {
                            if (selectedEnemy != null && game.Battle.UseCard(card, selectedEnemy.Entity))
                                returnCard = false;
                        }
                        else if (game.Battle.UseCard(card, game.Battle.Player.Entity))
                        {
                            returnCard = false;
                        }
                    }


                    if (returnCard)
                    {
                        if (_draggedCardActive)
                            handDisplay.AddCard(_draggedCard, _draggedCardIndex);
                        _draggedCard.gameObject.SetActive(true);
                    }
                    else
                    {
                        Destroy(_draggedCard.gameObject);
                    }

                    _draggedCard = null;
                    handDisplay.RepositionCards();
                }
            }
        }

        public void EndTurn()
        {
            // Allow player to end turn
            game.Battle.EndTurn();
            handDisplay.Show(game.Battle.Player);
        }
    }
}