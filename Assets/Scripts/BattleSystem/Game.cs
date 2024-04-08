using System;
using UnityEngine;

namespace BattleSystem
{
    public class Game : MonoBehaviour
    {
        private Level _level;
        public Battle Battle { get; private set; }
        public static Camera Camera { get; private set; }
    
        public event Action<Encounter> OnStartEncounter;
        public event Action<Encounter> OnEndEncounter;


        public void StartBattle(CardFactory cardFactory, Deck deck, Level level)
        {
            Camera = Camera.main;

            var player = new Player(100, deck);
            Battle = new Battle(cardFactory, player);
            _level = level;
        
            Battle.OnStartEncounter += OnStartEncounter;
            Battle.OnEndEncounter += OnEndEncounter;
            Battle.StartEncounter(_level.CurrentEncounter);
        }

        public void StartNextEncounter()
        {
            if (_level.CanAdvance())
            {
                _level.Advance();
                Battle.StartEncounter(_level.CurrentEncounter);
            }
        }
    }
}