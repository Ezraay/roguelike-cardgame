using UnityEngine;

namespace Display
{
    public class TargetingArrow : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        private void Start()
        {
            Hide();
        }

        public void Hide()
        {
            lineRenderer.gameObject.SetActive(false);
        }

        public void Show(Vector2 from, Vector2 to)
        {
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);
            lineRenderer.gameObject.SetActive(true);
        }
    }
}