using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI.Game;

namespace Core.Game
{
    public class BattleGUIManager : MonoBehaviour
    {
        [SerializeField] Slider _PlayerHPBar;
        [SerializeField] Slider _PlayerManaBar;
        [SerializeField] Slider _EnemyHPBar;
        [SerializeField] TextMeshProUGUI _turnText;

        [SerializeField] Image _stunIndicatorEnemy;
        [SerializeField] Image _stunIndicatorPlayer;

        [SerializeField] GameObject _burnIndicatorEnemy;
        [SerializeField] GameObject _burnIndicatorPlayer;

        CharacterStat _player, _enemy;

        [Header("Header Color")]
        [SerializeField] Image _headerBG;
        [SerializeField] Color _playerTurnColor;
        [SerializeField] Color _EnemyTurnColor;
        [SerializeField] Color _WinTurnColor;

        public static BattleGUIManager Instance;

        void Awake()
        {
            if(Instance == null) 
            {
                Instance = this;
            }
        }
        /// <summary>
        /// Setup battle gui
        /// </summary>
        public void SetupBattleGUI(CharacterStat player, CharacterStat enemy)
        {
            _player = player;
            _enemy = enemy;

            //reset stun indicator
            DisableAllEffectIndicator();

            _PlayerHPBar.maxValue = _player.GetMaxHP();
            _EnemyHPBar.maxValue = _enemy.GetMaxHP();
            _PlayerManaBar.maxValue = _player.GetMaxMana();

            //activate hp bar follow
            HPBarFollowCharacter hpBarFollowPlayer = _PlayerHPBar.GetComponentInParent<HPBarFollowCharacter>();
            hpBarFollowPlayer.SetFollow(player.transform);
            HPBarFollowCharacter hpBarFollowEnemy = _EnemyHPBar.GetComponent<HPBarFollowCharacter>();
            hpBarFollowEnemy.SetFollow(enemy.transform);

            UpdateHPUI();
            UpdateManaUI();

            //close skill
            PlayerSkillActionPanelManager.Instance.ClosePanel();
        }
        public void ActivateEffectIndicator(StatusEffect statusEffect, CharacterStat character, bool active)
        {
            switch (statusEffect)
            {
                case StatusEffect.Stun:
                    if (character == _player)
                    {
                        _stunIndicatorPlayer.gameObject.SetActive(active);
                    }
                    if (character == _enemy)
                    {
                        _stunIndicatorEnemy.gameObject.SetActive(active);
                    }
                    break;
                case StatusEffect.DOT:
                    if (character == _player)
                    {
                        _burnIndicatorPlayer.gameObject.SetActive(active);
                    }
                    if (character == _enemy)
                    {
                        _burnIndicatorEnemy.gameObject.SetActive(active);
                    }
                    break;
            }
        }

        public void DisableAllEffectIndicator()
        {
            _stunIndicatorEnemy.gameObject.SetActive(false);
            _stunIndicatorPlayer.gameObject.SetActive(false);
            _burnIndicatorEnemy.gameObject.SetActive(false);
            _burnIndicatorPlayer.gameObject.SetActive(false);
        }

        /// <summary>
        /// Update turn text
        /// </summary>
        public void UpdateTurnUI()
        {
            switch (BattleManager.Instance.GetTurnString())
            {
                case BattleState.NoBattle:
                    _turnText.text = "No Battle";
                    _headerBG.color = Color.black;
                    break;
                case BattleState.PlayerTurn:
                    _turnText.text = "Player Turn";
                    _headerBG.color = _playerTurnColor;
                    break;
                case BattleState.EnemyTurn:
                    _turnText.text = "Enemy Turn";
                    _headerBG.color = _EnemyTurnColor;
                    break;
                case BattleState.Win:
                    _turnText.text = "Player Win";
                    _headerBG.color = _WinTurnColor;
                    break;
                case BattleState.Lose:
                    _turnText.text = "Player Lose";
                    _headerBG.color = Color.black;
                    break;
            }
        }
        /// <summary>
        /// Update HP slider according to current data
        /// </summary>
        Coroutine _playerHPRoutine;
        Coroutine _enemyHPRoutine;
        public void UpdateHPUI()
        {
            if (_playerHPRoutine != null)
                StopCoroutine(_playerHPRoutine);

            if (_enemyHPRoutine != null)
                StopCoroutine(_enemyHPRoutine);

            if (gameObject.activeSelf)
            {
                _playerHPRoutine = StartCoroutine(AnimateSliderBar(_player.GetCurrentHP(), _PlayerHPBar));

                _enemyHPRoutine = StartCoroutine(AnimateSliderBar(_enemy.GetCurrentHP(), _EnemyHPBar));
            }
        }
        Coroutine _playerManaRoutine;
        public void UpdateManaUI()
        {
            if (_playerManaRoutine != null)
                StopCoroutine(_playerManaRoutine);

            if (gameObject.activeSelf)
            {
                _playerManaRoutine = StartCoroutine(AnimateSliderBar(_player.GetCurrentMana(), _PlayerManaBar));
            }
        }
        /// <summary>
        /// Animate slider bar
        /// </summary>
        IEnumerator AnimateSliderBar(float targetValue, Slider slider)
        {
            while (Mathf.Abs(slider.value - targetValue) > 0.01f)
            {
                slider.value = Mathf.Lerp(
                    slider.value,
                    targetValue,
                    Time.deltaTime * 5f
                );

                yield return null;
            }

            slider.value = targetValue;
        }

        #region Player Action
        /// <summary>
        /// On click attack button
        /// </summary>
        public void PlayerAttack()
        {
            BattleManager.Instance.OnPlayerAction(PlayerAction.Attack);
            //close skill if open
            PlayerSkillActionPanelManager.Instance.ClosePanel();
        }
        /// <summary>
        /// On click defend button
        /// </summary>
        public void PlayerDefend()
        {
            BattleManager.Instance.OnPlayerAction(PlayerAction.Defend);
            //close skill if open
            PlayerSkillActionPanelManager.Instance.ClosePanel();
        }
        /// <summary>
        /// On click skill button
        /// </summary>
        public void PlayerSkill()
        {
            if (BattleManager.Instance.GetTurnString().ToString().Contains("Player"))
            {
                PlayerSkillActionPanelManager.Instance.TogglePanel();
            }
        }
        #endregion
    }
}
