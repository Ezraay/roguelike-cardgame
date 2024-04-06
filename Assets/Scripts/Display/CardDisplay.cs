using TMPro;
using UnityEngine;

namespace Display
{
    public class CardDisplay : MonoBehaviour
    {
        public static Vector2 CardSize = new(4.5f, 6f);
        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text descriptionText;
        public Card Card { get; private set; }


        public void ShowCard(Card card, bool showCost = true)
        {
            Card = card;
            nameText.text = card.Name;
            costText.text = card.EnergyCost.ToString();
            costText.gameObject.SetActive(showCost);
            descriptionText.text = card.GetDescription();
        }

        public void SetOrder(int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }

        public bool IsMouseOver(Vector2 mousePosition)
        {
            var rect = new Rect((Vector2)transform.position - CardSize / 2f, CardSize);
            var rectAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
            var s = Mathf.Sin(-rectAngle);
            var c = Mathf.Cos(-rectAngle);

            var newPoint = mousePosition - rect.center;
            newPoint = new Vector2(newPoint.x * c - newPoint.y * s, newPoint.x * s + newPoint.y * c);
            newPoint += rect.center;

            return newPoint.x >= rect.xMin && newPoint.x <= rect.xMax && newPoint.y >= rect.yMin &&
                   newPoint.y <= rect.yMax;
        }
    }
}