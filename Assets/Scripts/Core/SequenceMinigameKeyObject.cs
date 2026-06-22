using UnityEngine;
using UnityEngine.UI;

namespace Core.Game
{
    public class SequenceMinigameKeyObject : MonoBehaviour
    {
        [SerializeField] Sprite _defaultKeyBG;
        [SerializeField] Sprite _keyCorrectBG;
        [SerializeField] Image _backgroundKey;
        [SerializeField] Image _keyImage;

        /// <summary>
        /// Set key object image
        /// </summary>
        public void SetData(Sprite keySprite)
        {
            _backgroundKey.sprite = _defaultKeyBG;
            _keyImage.sprite = keySprite;
            _keyImage.color = Color.white;

            gameObject.SetActive(true);
        }
        /// <summary>
        /// Set key image color and sprite to correct
        /// </summary>
        public void KeyCorrect()
        {
            _keyImage.color = Color.gray;
            _backgroundKey.sprite = _keyCorrectBG;
        }
    }
}
