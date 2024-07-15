using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VisualDebug
{
    public class MovableUIPanel : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        private RectTransform _panel;
        private Canvas _canvas;

        void Awake()
        {
            UpdateHandleName();

            if (_panel != null)
            {
                Transform parentTransform = transform.parent;
                while (parentTransform != null)
                {
                    _canvas = parentTransform.GetComponent<Canvas>();

                    if (_canvas != null) break;

                    parentTransform = parentTransform.parent;
                }
            }
        }

        void OnEnable()
        {
            VisualConsoleUI.OnCardChanged += UpdateHandleName;
        }

        void OnDisable()
        {
            VisualConsoleUI.OnCardChanged -= UpdateHandleName;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _panel.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            CheckIfOutOfBounds();
        }

        public void CheckIfOutOfBounds()
        {
            Camera camera = Camera.main;

            Vector3[] corners = new Vector3[4];
            Vector3[] cornersToViewport = new Vector3[4];

            _panel.GetWorldCorners(corners);

            for (int i = 0; i < corners.Length; i++)
            {
                cornersToViewport[i] = camera.ScreenToViewportPoint(corners[i]);
            }

            // Left edge is out of bounds
            if (cornersToViewport[1].x < 0)
            {
                _panel.position = new Vector3(camera.ViewportToScreenPoint(Vector3.zero).x, _panel.position.y,
                    _panel.position.z);
            }

            // Top edge is out of bounds
            if (cornersToViewport[1].y > camera.rect.height)
            {
                _panel.position = new Vector3(_panel.position.x, camera.ViewportToScreenPoint(Vector3.up).y,
                    _panel.position.z);
            }

            // Right edge is out of bounds
            if (cornersToViewport[2].x > camera.rect.width)
            {
                _panel.position = new Vector3(
                    camera.ViewportToScreenPoint(Vector3.right).x - _panel.rect.width * _canvas.scaleFactor,
                    _panel.position.y,
                    _panel.position.z);
            }

            // Bottom edge is out of bounds
            if (cornersToViewport[0].y < 0)
            {
                _panel.position = new Vector3(_panel.position.x,
                    camera.ViewportToScreenPoint(Vector3.zero).y + _panel.rect.height * _canvas.scaleFactor,
                    _panel.position.z);
            }
        }

        private void UpdateHandleName()
        {
            try
            {
                _panel = transform.parent.GetComponent<RectTransform>();
                GetComponentInChildren<TextMeshProUGUI>().text = _panel.parent.name;
            }
            catch (System.Exception)
            {
                Debug.LogError("Panel object is NULL");
            }
        }
    }
}