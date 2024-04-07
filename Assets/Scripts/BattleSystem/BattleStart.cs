using UnityEngine;
using UnityEngine.SceneManagement;

namespace BattleSystem
{
    public static class BattleStart
    {
        private const string SceneName = "Game";
    
        public static async void LoadBattleScene(Deck deck, Level level)
        {
            await SceneManager.LoadSceneAsync(SceneName);
            var game = GameObject.FindAnyObjectByType<Game>();
            game.StartBattle(GlobalState.GetCardFactory(), deck, level);
        }
    }
}