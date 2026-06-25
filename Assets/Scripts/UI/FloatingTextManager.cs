using System.Collections.Generic;
using UnityEngine;

namespace UI.Game
{
    public enum FloatingTextType
    {
        Damage,
        StatusEffect,
        GainMana
    }

    public class FloatingTextManager : MonoBehaviour
    {
        [SerializeField] FloatingTextObject _floatingTextPrefab;
        [SerializeField] Transform _floatingTextContent;
        [SerializeField] Color _damageColor = Color.red;
        [SerializeField] Color _statusEffectColor = Color.yellow;
        [SerializeField] Color _gainManaColor = Color.blue;
        Canvas _canvas;
        Camera _cam;

        List<FloatingTextObject> _listSpawnedFloatingText = new List<FloatingTextObject>();

        public static FloatingTextManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            _cam = Camera.main;
            _canvas = GetComponentInParent<Canvas>();
        }
        /// <summary>
        /// Show floating text
        /// </summary>
        public void ShowFloatingText(string message, FloatingTextType type, Transform character)
        {
            //get unused object
            FloatingTextObject unusedObject = GetUnusedFloatingText();
            if (unusedObject == null)
            {
                unusedObject = Instantiate(_floatingTextPrefab, _floatingTextContent);
                _listSpawnedFloatingText.Add(unusedObject);
            }

            //setup pos
            Vector3 worldSpawnPos = character.position + Vector3.up * 2f;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_cam, worldSpawnPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _cam, out Vector2 localCanvasPos);
            //get active floating text
            int activeCount = 0;
            for (int i = 0; i < _listSpawnedFloatingText.Count; i++)
            {
                if (_listSpawnedFloatingText[i].gameObject.activeSelf)
                {
                    activeCount++;
                }
            }
            //randomize pos according to active floating text to prevent stacking
            float offsetX = Random.Range(-40f, 40f);
            float offsetY = (activeCount * 35f) + Random.Range(-10f, 10f);
            localCanvasPos += new Vector2(offsetX, offsetY);
            unusedObject.GetComponent<RectTransform>().anchoredPosition = localCanvasPos;
            unusedObject.gameObject.SetActive(true);

            Color textColor = Color.red;
            switch (type)
            {
                case FloatingTextType.Damage:
                    unusedObject.Setup(message, _damageColor);
                    break;
                case FloatingTextType.StatusEffect:
                    unusedObject.Setup(message, _statusEffectColor);
                    break;
                case FloatingTextType.GainMana:
                    unusedObject.Setup(message, _gainManaColor);
                    break;
            }
        }
        /// <summary>
        /// Get unused floating text object
        /// </summary>
        FloatingTextObject GetUnusedFloatingText()
        {
            for (int i = 0; i < _listSpawnedFloatingText.Count; i++)
            {
                if (!_listSpawnedFloatingText[i].gameObject.activeSelf)
                {
                    return _listSpawnedFloatingText[i];
                }
            }
            return null;
        }
    }
}