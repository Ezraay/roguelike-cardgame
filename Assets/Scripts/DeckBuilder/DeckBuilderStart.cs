using UnityEngine.SceneManagement;

public static class DeckBuilderStart 
{
    private const string SceneName = "Deck Builder";
    
    public static void LoadDeckBuilder()
    {
        SceneManager.LoadScene(SceneName);
    }
}