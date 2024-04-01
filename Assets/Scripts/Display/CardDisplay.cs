using System;
using System.Drawing;
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
        public Card Card { get; private set; }

        public void ShowCard(Card card, bool showCost = true)
        {
            Card = card;
            nameText.text = card.Name;
            costText.text = card.EnergyCost.ToString();
            costText.gameObject.SetActive(showCost);
            descriptionText.text = card.GetDescription();
        }

        private void Update()
        {
            if (IsMouseOver(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                Debug.Log("bbb");
        }

        public bool IsMouseOver(Vector2 mousePosition)
        {
            var rect = new Rect((Vector2)transform.position - CardSize / 2f, CardSize);
            var rectAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
            // rotate around rectangle center by -rectAngle
            var s = Mathf.Sin(-rectAngle);
            var c = Mathf.Cos(-rectAngle);

            // set origin to rect center
            var newPoint = mousePosition - rect.center;
            // rotate
            newPoint = new Vector2(newPoint.x * c - newPoint.y * s, newPoint.x * s + newPoint.y * c);
            // put origin back
            newPoint += rect.center;

            // check if our transformed point is in the rectangle, which is no longer
            // rotated relative to the point

            return newPoint.x >= rect.xMin && newPoint.x <= rect.xMax && newPoint.y >= rect.yMin && newPoint.y <= rect.yMax;
            return rect.Contains(Camera.main.ScreenToWorldPoint(mousePosition));
        }
    }
}