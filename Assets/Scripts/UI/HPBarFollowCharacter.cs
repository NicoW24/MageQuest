using TMPro;
using UnityEngine;

namespace UI.Game
{
    public class HPBarFollowCharacter : MonoBehaviour
    {
        public Transform followTarget;
        RectTransform _rect;
        RectTransform _canvasRect;
        Canvas _canvas;
        Camera _cam;

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _cam = Camera.main;

            _canvas = GetComponentInParent<Canvas>();
            _canvasRect = _canvas.GetComponent<RectTransform>();
        }
        /// <summary>
        /// Set character to follow
        /// </summary>
        public void SetFollow(Transform target)
        {
            followTarget = target;
        }
        
        void LateUpdate()
        {
            if (followTarget == null || !gameObject.activeInHierarchy)
                return;

            Vector3 worldPos = followTarget.position + Vector3.down * 0.5f;
            Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                screenPos,
                _cam,
                out Vector2 uiPos
            );
            _rect.anchoredPosition = uiPos;
        }
    }
}
