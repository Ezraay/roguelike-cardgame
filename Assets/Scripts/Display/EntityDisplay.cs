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
        private Entity _entity;
        private Rect _rect;

        private void Awake()
        {
            _rect = new Rect((Vector2)transform.position - rectSize/2f, rectSize);
        }

        private void Update()
        {
            if (_entity == null) return;
            healthText.text = $"{_entity.Health}/{_entity.MaxHealth}";
            healthBar.fillAmount = (float)_entity.Health / _entity.MaxHealth;
        }

        public void Show(Entity entity)
        {
            _entity = entity;
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