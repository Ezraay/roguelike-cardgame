using System;
using DrawXXL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Display
{
    public class EntityDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Image healthBar;
        [SerializeField] private Vector2 rectSize;
        public Entity Entity { get; private set; }
        private Rect _rect;

        private void Awake()
        {
            _rect = new Rect((Vector2)transform.position - rectSize/2f, rectSize);
        }

        private void Update()
        {
            if (Entity == null) return;
            healthText.text = $"{Entity.Health}/{Entity.MaxHealth}";
            healthBar.fillAmount = (float)Entity.Health / Entity.MaxHealth;
        }

        public void Show(Entity entity)
        {
            Entity = entity;
        }

        public bool IsMouseOver()
        {
            return _rect.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        private void OnDrawGizmos()
        {
            DrawShapes.Rectangle(_rect);
        }
    }
}