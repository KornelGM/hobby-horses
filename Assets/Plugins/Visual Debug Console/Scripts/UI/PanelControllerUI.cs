using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VisualDebug
{
    public class PanelControllerUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float _minWidth = 200;
        [SerializeField] private float _maxWidth = 500;
        [SerializeField] private float _minHeight = 200;
        [SerializeField] private float _maxHeight = 500;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private MovableUIPanel _panelHandle;

        private Vector2 _sizeDelta;

        private Vector2 _currentPointerPosition;
        private Vector2 _previousPointerPosition;
        private Vector2 _minSize;
        private Vector2 _maxSize;

        private void Awake()
        {
            if (_panel == null)
            {
                Debug.LogError("Panel object to resize is NULL");
                return;
            }

            _panel = GetComponent<RectTransform>();

            SetupMinAndMaxPanelSize();
            CheckPanelSize();
        }

        private void SetupMinAndMaxPanelSize()
        {
            _minSize = new Vector2(_panel.sizeDelta.x - _panel.rect.width + _minWidth,
                _panel.sizeDelta.y - _panel.rect.height + _minHeight);

            _maxSize = new Vector2(_panel.sizeDelta.x - _panel.rect.width + _maxWidth,
                _panel.sizeDelta.y - _panel.rect.height + _maxHeight);
        }

        private void CheckPanelSize()
        {
            if (_panel.rect.width < _minWidth) _panel.sizeDelta += new Vector2(_minWidth - _panel.rect.width, 0);
            if (_panel.rect.width > _maxWidth) _panel.sizeDelta -= new Vector2(_panel.rect.width - _maxWidth, 0);

            if (_panel.rect.height < _minHeight) _panel.sizeDelta += new Vector2(0, _minHeight - _panel.rect.height);
            if (_panel.rect.height > _maxHeight) _panel.sizeDelta -= new Vector2(0, _panel.rect.height - _maxHeight);
        }

        public void OnDrag(PointerEventData data)
        {
            if (_panel == null)
            {
                return;
            }

            _sizeDelta = _panel.sizeDelta;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_panel, data.position, data.pressEventCamera,
                out _currentPointerPosition);

            Vector2 resizeValue = _currentPointerPosition - _previousPointerPosition;

            _sizeDelta += new Vector2(resizeValue.x, -resizeValue.y);

            _sizeDelta = new Vector2(
                Mathf.Clamp(_sizeDelta.x, _minSize.x, _maxSize.x),
                Mathf.Clamp(_sizeDelta.y, _minSize.y, _maxSize.y)
            );

            _panel.sizeDelta = _sizeDelta;
            _previousPointerPosition = _currentPointerPosition;
        }

        public void OnPointerDown(PointerEventData data)
        {
            MovePanelForward();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_panel, data.position, data.pressEventCamera,
                out _previousPointerPosition);
        }

        private void MovePanelForward()
        {
            transform.parent.SetAsLastSibling();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_panelHandle != null) _panelHandle.CheckIfOutOfBounds();
        }
    }
}