using UnityEngine;

namespace Core.Game
{
    public class CustomizePlayerCharacter : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _hairRenderer;
        [SerializeField] SpriteRenderer _lArmRenderer;
        [SerializeField] SpriteRenderer _rArmRenderer;
        [SerializeField] SpriteRenderer _topRenderer;
        [SerializeField] SpriteRenderer _capeRenderer;

        public static CustomizePlayerCharacter Instance;

        void Awake()
        {
            if(Instance == null) 
            { 
                Instance = this;
            }
        }

        public void SetHairSprite(Sprite sprite)
        {
            _hairRenderer.sprite = sprite;
        }

        public void SetTopSprite(Sprite spriteTop, Sprite spriteLarm, Sprite spriteRarm)
        {
            _lArmRenderer.sprite = spriteLarm;
            _rArmRenderer.sprite = spriteRarm;
            _topRenderer.sprite = spriteTop;
        }

        public void SetCapeSprite(Sprite sprite)
        {
            _capeRenderer.sprite = sprite;
        }
    }
}

