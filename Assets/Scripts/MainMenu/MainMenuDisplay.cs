using BattleSystem;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuDisplay : MonoBehaviour
    {
        public void StartBattle()
        {
            var level = GlobalState.GetLevelFactory().GetLevel("goblin-hut");
            BattleStart.LoadBattleScene(GlobalState.GetDeck(), level);
        }

        public void LoadDeckBuilder()
        {
            DeckBuilderStart.LoadDeckBuilder();
        }
    }
}