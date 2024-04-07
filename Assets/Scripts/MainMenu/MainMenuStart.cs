using UnityEngine.SceneManagement;

public static class MainMenuStart
{
    private const string SceneName = "Main Menu";

    public static void LaodMainMenu()
    {
        SceneManager.LoadScene(SceneName);
    }
}