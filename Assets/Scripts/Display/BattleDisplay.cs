using DrawXXL;
using Effects;
using UnityEngine;

namespace Display
{
    public class BattleDisplay : MonoBehaviour
    {
        [SerializeField] private Game game;
        [SerializeField] private EntityDisplay entityDisplayPrefab;
        [SerializeField] private Transform playerDisplayParent;
        [SerializeField] private EnemyLayout enemyLayout;
        [SerializeField] private CardLayout handLayout;
        [SerializeField] private EnergyDisplay energyDisplay;
        private bool _draggedCardActive;
        private CardDisplay _draggedCard;
        private Vector2 _draggedCardStartPosition;

        private void Awake()
        {
            game.Battle.OnStartEncounter += StartEncounter;
        }

        private void StartEncounter()
        {
            // Show player and enemy health
            var playerDisplay = Instantiate(entityDisplayPrefab, playerDisplayParent);
            playerDisplay.Show(game.Battle.Player.Entity);
            enemyLayout.Show(game.Battle.Enemies);


            // Show player cards
            handLayout.Show(game.Battle.Player.Hand);

            // TODO Show player deck and discard

            // Show player energy
            energyDisplay.Show(game.Battle.Player);

            // TODO Show animated card actions


            // TODO Game over screen
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Raycast for card
                var ray = Game.Camera.ScreenPointToRay(Input.mousePosition);
                var hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                    _draggedCard = hit.collider.GetComponent<CardDisplay>();
                    _draggedCardStartPosition = _draggedCard.transform.position;
                    _draggedCardActive = false;
                }
            }

            if (_draggedCard == null) return;
            if (handLayout.IsMouseOver() && _draggedCardActive) // Dragged card back to hand
            {
                _draggedCardActive = false;
                _draggedCard.gameObject.SetActive(true);
            }
            else if (!handLayout.IsMouseOver() && !_draggedCardActive) // Dragged card from hand
            {
                _draggedCardActive = true;
                if (_draggedCard.Card.TargetingType == TargetingType.Enemy)
                    _draggedCard.gameObject.SetActive(false);
            }

            var selectedEnemy = enemyLayout.GetHoveredEnemy();

            if (_draggedCardActive && _draggedCard.Card.TargetingType == TargetingType.Enemy)
            {
                var startPosition = _draggedCardStartPosition;
                var endPosition = selectedEnemy != null
                    ? selectedEnemy.transform.position
                    : Game.Camera.ScreenToWorldPoint(Input.mousePosition);
                DrawBasics2D.Vector(startPosition, endPosition);
            }

            var position = Game.Camera.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            _draggedCard.transform.position = position;

            if (Input.GetMouseButtonUp(0) && _draggedCard != null)
            {
                var returnCard = true;
                if (!handLayout.IsMouseOver())
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
                    _draggedCard.gameObject.SetActive(true);
                }
                else
                {
                    handLayout.RemoveCard(_draggedCard);
                    Destroy(_draggedCard.gameObject);
                }

                _draggedCard = null;
                handLayout.RepositionCards();
            }
        }

        public void EndTurn()
        {
            // Allow player to end turn
            game.Battle.EndTurn();
            handLayout.Show(game.Battle.Player.Hand);
            enemyLayout.UpdateIntents(game.Battle.Enemies);
        }
    }
}