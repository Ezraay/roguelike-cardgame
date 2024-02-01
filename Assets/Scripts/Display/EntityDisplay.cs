using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Display
{
    public class EntityDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Image healthBar;
        private Entity _entity;

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
    }
}