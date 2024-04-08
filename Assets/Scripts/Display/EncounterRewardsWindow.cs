using System;
using UnityEngine;

namespace Display
{
    public class EncounterRewardsWindow : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject continueButton;
        [SerializeField] private GameObject backButton;
        public event Action OnClose;
    
        public void Show(bool finalEncounter)
        {
            // TODO Actual rewards
            content.SetActive(true);
            continueButton.SetActive(!finalEncounter);
            backButton.SetActive(finalEncounter);
        }

        public void Hide()
        {
            content.SetActive(false);
            OnClose?.Invoke();
        }

        public void BackToMainMenu()
        {
            MainMenuStart.LaodMainMenu();
        }
    }
}