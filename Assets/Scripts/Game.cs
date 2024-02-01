using UnityEngine;

public class Game : MonoBehaviour
{
    public Battle Battle { get; private set; }

    private void Awake()
    {
        Battle = new Battle();
        Battle.Start();
    }
}