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
        CharacterStat _player, _enemy;

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
        /// <summary>
        /// Update turn text
        /// </summary>
        public void UpdateTurnUI()
        {
            _turnText.text = BattleManager.Instance.GetTurnString();
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

            _playerHPRoutine = StartCoroutine(AnimateSliderBar(_player.GetCurrentHP(), _PlayerHPBar));

            _enemyHPRoutine = StartCoroutine(AnimateSliderBar(_enemy.GetCurrentHP(), _EnemyHPBar));
        }
        Coroutine _playerManaRoutine;
        public void UpdateManaUI()
        {
            if (_playerManaRoutine != null)
                StopCoroutine(_playerManaRoutine);

            _playerManaRoutine = StartCoroutine(AnimateSliderBar(_player.GetCurrentMana(), _PlayerManaBar));
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
            BattleManager.Instance.OnPlayerAction(0);
        }
        /// <summary>
        /// On click defend button
        /// </summary>
        public void PlayerDefend()
        {
            BattleManager.Instance.OnPlayerAction(1);
        }
        /// <summary>
        /// On click skill button
        /// </summary>
        public void PlayerSkill()
        {
            if (BattleManager.Instance.GetTurnString().Contains("Player"))
            {
                PlayerSkillActionPanelManager.Instance.OpenPanel();
            }
        }
        #endregion
    }
}
