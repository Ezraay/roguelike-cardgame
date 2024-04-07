using System;
using UnityEngine;

namespace Display
{
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
}