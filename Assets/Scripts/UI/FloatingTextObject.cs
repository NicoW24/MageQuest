using System.Collections;
using TMPro;
using UnityEngine;

namespace UI.Game
{
    public class FloatingTextObject : MonoBehaviour
    {
        TextMeshProUGUI _text;
        [SerializeField] float _moveSpeed = 2f;
        [SerializeField] float _duration = 1f;

        RectTransform _rectTransform;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void Setup(string text, Color color)
        {
            _text.text = text;
            _text.color = color;

            StartCoroutine(AnimateText());
        }

        IEnumerator AnimateText()
        {
            float timer = 0f;
            Color originalColor = _text.color;

            while (timer < _duration)
            {
                timer += Time.deltaTime;
                float progress = timer / _duration;

                _rectTransform.anchoredPosition += Vector2.up * _moveSpeed * Time.deltaTime;

                float alpha = Mathf.Lerp(1f, 0f, progress);
                _text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}