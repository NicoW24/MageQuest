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

            //activate hp bar follow
            HPBarFollowCharacter hpBarFollowPlayer = _PlayerHPBar.GetComponent<HPBarFollowCharacter>();
            hpBarFollowPlayer.SetFollow(player.transform);
            HPBarFollowCharacter hpBarFollowEnemy = _EnemyHPBar.GetComponent<HPBarFollowCharacter>();
            hpBarFollowEnemy.SetFollow(enemy.transform);

            UpdateHPUI();
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
        Coroutine _playerHpRoutine;
        Coroutine _enemyHpRoutine;
        public void UpdateHPUI()
        {
            if (_playerHpRoutine != null)
                StopCoroutine(_playerHpRoutine);

            if (_enemyHpRoutine != null)
                StopCoroutine(_enemyHpRoutine);

            _playerHpRoutine = StartCoroutine(AnimateHPBar(_player.GetCurrentHP(), _PlayerHPBar));

            _enemyHpRoutine = StartCoroutine(AnimateHPBar(_enemy.GetCurrentHP(), _EnemyHPBar));
        }
        /// <summary>
        /// Animate HP Bar
        /// </summary>
        IEnumerator AnimateHPBar(float targetValue, Slider hpSlider)
        {
            while (Mathf.Abs(hpSlider.value - targetValue) > 0.01f)
            {
                hpSlider.value = Mathf.Lerp(
                    hpSlider.value,
                    targetValue,
                    Time.deltaTime * 5f
                );

                yield return null;
            }

            hpSlider.value = targetValue;
        }

        #region Player Action
        public void PlayerAttack()
        {
            BattleManager.Instance.OnPlayerAction(0);
        }
        public void PlayerDefend()
        {
            BattleManager.Instance.OnPlayerAction(1);
        }
        public void PlayerSkill()
        {
            //show skill selection UI (NOT DONE)
            BattleManager.Instance.OnPlayerAction(2);
        }
        #endregion
    }
}
