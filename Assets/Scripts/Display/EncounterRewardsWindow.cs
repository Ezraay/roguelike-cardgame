using System;
using UnityEngine;

public class EncounterRewardsWindow : MonoBehaviour
{
    [SerializeField] private GameObject content;
    public event Action OnClose;
    
    public void Show()
    {
        // TODO Actual rewards
        content.SetActive(true);
    }

    public void Hide()
    {
        content.SetActive(false);
        OnClose?.Invoke();
    }
}