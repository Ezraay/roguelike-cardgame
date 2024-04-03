using System.Collections.Generic;
using Display;
using UnityEngine;

public class EnemyLayout : MonoBehaviour
{
    [SerializeField] private EntityDisplay entityDisplayPrefab;
    [SerializeField] private Vector2 enemyOffset;
    private readonly List<EntityDisplay> _enemyDisplays = new();

    public void Show(List<Enemy> enemies)
    {
        var halfOffset = enemyOffset * (enemies.Count - 1) / 2;
        _enemyDisplays.Clear();
        for (var i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            Vector3 position = enemyOffset * i - halfOffset;
            var enemyDisplay = Instantiate(entityDisplayPrefab, transform.position + position,
                Quaternion.identity, transform);
            _enemyDisplays.Add(enemyDisplay);

            // Destroy dead enemies
            enemy.OnDeath += () =>
            {
                Destroy(enemyDisplay.gameObject);
                _enemyDisplays.Remove(enemyDisplay);
            };

            // Show enemy
            enemyDisplay.Show(enemy);
            enemyDisplay.UpdateIntents(enemy.GetIntents());
        }
    }

    public EntityDisplay GetHoveredEnemy()
    {
        foreach (var enemyDisplay in _enemyDisplays)
            if (enemyDisplay.IsMouseOver())
                return enemyDisplay;

        return null;
    }

    public void UpdateIntents(List<Enemy> enemies)
    {
        for (var i = 0; i < enemies.Count; i++)
        {
            var enemyDisplay = _enemyDisplays[i];
            enemyDisplay.UpdateIntents(enemies[i].GetIntents());
        }
    }
}