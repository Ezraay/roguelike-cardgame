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
        [SerializeField] private TargettingArrow targettingArrow;
        [SerializeField] private EncounterRewardsWindow encounterRewardsWindow;
        private bool _draggedCardActive;
        private CardDisplay _draggedCard;
        private Vector2 _draggedCardStartPosition;

        private void Awake()
        {
            game.Battle.OnStartEncounter += StartEncounter;
            game.Battle.OnEndEncounter += EndEncounter;

            encounterRewardsWindow.OnClose += game.StartNextEncounter;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) StartDragging();

            if (_draggedCard == null) return;
            if (handLayout.IsMouseOver() && _draggedCardActive) // Dragged card back to hand
            {
                _draggedCardActive = false;
                _draggedCard.gameObject.SetActive(true);
            }
            else if (!handLayout.IsMouseOver() && !_draggedCardActive) // Dragged card from hand
            {
                _draggedCardActive = true;
                _draggedCard.gameObject.SetActive(_draggedCard.Card.TargetingType != TargetingType.Enemy);
            }

            var selectedEnemy = enemyLayout.GetHoveredEnemy();
            UpdateCardPosition(selectedEnemy);

            if (Input.GetMouseButtonUp(0) && _draggedCard != null) StopDragging(selectedEnemy);
        }

        private void StartDragging()
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

        private void StopDragging(EntityDisplay selectedEnemy)
        {
            if (TryUseCard(selectedEnemy?.Entity))
            {
                handLayout.RemoveCard(_draggedCard);
                Destroy(_draggedCard.gameObject);
            }
            else
            {
                _draggedCard.gameObject.SetActive(true);
            }

            _draggedCard = null;
            handLayout.RepositionCards();
            targettingArrow.Hide();
        }

        private bool TryUseCard(Entity selectedEnemy)
        {
            if (!handLayout.IsMouseOver())
            {
                var card = _draggedCard.Card;
                var target = card.TargetingType == TargetingType.Enemy
                    ? selectedEnemy
                    : game.Battle.Player.Entity;
                if (target != null && game.Battle.UseCard(card, target))
                    return true;
            }

            return false;
        }

        private void UpdateCardPosition(EntityDisplay selectedEnemy)
        {
            var mousePosition = Game.Camera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            if (_draggedCardActive && _draggedCard.Card.TargetingType == TargetingType.Enemy)
            {
                var startPosition = _draggedCardStartPosition;
                var endPosition = selectedEnemy != null
                    ? selectedEnemy.transform.position
                    : mousePosition;
                targettingArrow.Show(startPosition, endPosition);
            }
            else
            {
                targettingArrow.Hide();
            }

            _draggedCard.transform.position = mousePosition;
        }

        public void EndTurn()
        {
            // Allow player to end turn
            game.Battle.EndTurn();
            handLayout.Show(game.Battle.Player.Hand);
            enemyLayout.UpdateIntents(game.Battle.Enemies);
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
        }

        private void EndEncounter()
        {
            // Game over screen
            encounterRewardsWindow.Show();
        }
    }
}