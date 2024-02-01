using System;
using UnityEngine;

namespace Display
{
    public class BattleDisplay : MonoBehaviour
    {
        [SerializeField] private Game game;
        [SerializeField] private EntityDisplay entityDisplayPrefab;
        [SerializeField] private Transform playerDisplayParent;
        [SerializeField] private Transform enemyDisplayParent;

        private void Start()
        {
            // TODO Show player and enemy health
            var playerDisplay = Instantiate(entityDisplayPrefab, playerDisplayParent);
            playerDisplay.Show(game.Battle.Player.Entity);

            foreach (var enemy in game.Battle.Enemies)
            {
                var enemyDisplay = Instantiate(entityDisplayPrefab, enemyDisplayParent);
                enemyDisplay.Show(enemy);
            }
            // TODO Show enemy intents
            // TODO Show player cards
            // TODO Show player deck and discard
            // TODO Show player energy
            // TODO Allow player to play card
            // TODO Allow player to target card
            // TODO Allow player to end turn
            // TODO Show animated card actions
        }
    }
}