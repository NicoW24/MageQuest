using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class CustomizeCharacterPanelManager : MonoBehaviour
    {
        [SerializeField] List<Sprite> _listSpriteHair = new List<Sprite>();
        [SerializeField] List<Sprite> _listSpriteTop = new List<Sprite>();//must have the same count as l and r arm
        [SerializeField] List<Sprite> _listSpriteLeftArmTop = new List<Sprite>();
        [SerializeField] List<Sprite> _listSpriteRightArmTop = new List<Sprite>();
        [SerializeField] List<Sprite> _listSpriteCape = new List<Sprite>();

        [SerializeField] SpriteRenderer _playerTopPreviewRenderer;
        [SerializeField] SpriteRenderer _playerLeftArmPreviewRenderer;
        [SerializeField] SpriteRenderer _playerHairPreviewRenderer;
        [SerializeField] SpriteRenderer _playerRightArmPreviewRenderer;
        [SerializeField] SpriteRenderer _playerCapePreviewRenderer;

        int _playerHairIndex = -1;
        int _playerTopIndex = -1;
        int _playerCapeIndex = -1;

        public void PauseCharacter()
        {
            BattleManager.Instance.GetPlayerStat().GetController().PauseCharacter();
        }
        public void ResumeCharacter()
        {
            BattleManager.Instance.GetPlayerStat().GetController().ResumeCharacter();
        }

        public void OnCloseCustomizePanel()
        {
            ResumeCharacter();

            //after costumize play intro cutscene
            TimelineManager.Instance.PlayIntroTimeline();
        }

        /// <summary>
        /// Change hair player character
        /// </summary>
        public void NextHair()
        {
            _playerHairIndex = (_playerHairIndex + 1) % _listSpriteHair.Count;
            _playerHairPreviewRenderer.sprite = _listSpriteHair[_playerHairIndex];

            CustomizePlayerCharacter.Instance.SetHairSprite(_listSpriteHair[_playerHairIndex]);
        }
        /// <summary>
        /// Change top player character
        /// </summary>
        public void NextTop()
        {
            _playerTopIndex = (_playerTopIndex + 1) % _listSpriteTop.Count;

            _playerLeftArmPreviewRenderer.sprite = _listSpriteLeftArmTop[_playerTopIndex];
            _playerRightArmPreviewRenderer.sprite = _listSpriteRightArmTop[_playerTopIndex];
            _playerTopPreviewRenderer.sprite = _listSpriteTop[_playerTopIndex];

            CustomizePlayerCharacter.Instance.SetTopSprite(_listSpriteTop[_playerTopIndex], _listSpriteLeftArmTop[_playerTopIndex], _listSpriteRightArmTop[_playerTopIndex]);
        }
        /// <summary>
        /// Change cape player character
        /// </summary>
        public void NextCape()
        {
            _playerCapeIndex = (_playerCapeIndex + 1) % _listSpriteCape.Count;
            _playerCapePreviewRenderer.sprite = _listSpriteCape[_playerCapeIndex];

            CustomizePlayerCharacter.Instance.SetCapeSprite(_listSpriteCape[_playerCapeIndex]);
        }
    }
}

