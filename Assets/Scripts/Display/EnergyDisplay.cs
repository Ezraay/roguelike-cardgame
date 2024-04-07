using BattleSystem;
using TMPro;
using UnityEngine;

namespace Display
{
    public class EnergyDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text energyText;
        private Player _player;

        private void Update()
        {
            if (_player == null) return;
            energyText.text = $"Energy: {_player.Energy} / {_player.EnergyPerTurn}";
        }

        public void Show(Player player)
        {
            _player = player;
        }
    }
}