using BattleSystem;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuDisplay : MonoBehaviour
    {
        public void StartBattle()
        {
            var level = new Level(new Encounter(new Encounter())); // TODO Move to level selection
            BattleStart.LoadBattleScene(GlobalState.GetDeck(), level);
        }

        public void LoadDeckBuilder()
        {
            DeckBuilderStart.LoadDeckBuilder();
        }
    }
}