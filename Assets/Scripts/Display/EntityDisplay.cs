using System;
using System.Collections.Generic;
using DrawXXL;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Display
{
    public class EntityDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text blockText;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image blockBar;
        [SerializeField] private Vector2 rectSize;
        [SerializeField] private CardDisplay cardDisplayPrefab;
        [SerializeField] private CardLayout intentDisplay;
        
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
            
            blockText.text = Entity.Block > 0 ? $"{Entity.Block}" : "";
            blockBar.fillAmount = (float)Entity.Block / Entity.MaxHealth;
        }

        public void UpdateIntents(List<Card> intents)
        {
            intentDisplay.Show(intents);
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