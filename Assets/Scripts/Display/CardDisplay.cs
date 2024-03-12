using TMPro;
using UnityEngine;

namespace Display
{
    public class CardDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text descriptionText;
        public static Vector2 CardSize = new Vector2(4.5f, 6f);
        
        public void ShowCard(Card card, bool showCost = true)
        {
            nameText.text = card.Name;
            costText.text = card.EnergyCost.ToString();
            costText.gameObject.SetActive(showCost);
            descriptionText.text = card.GetDescription();
        }
    }
}