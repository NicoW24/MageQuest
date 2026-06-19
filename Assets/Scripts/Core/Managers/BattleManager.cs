using System.Collections;
using System.Collections.Generic;
using UI.Game;
using UnityEngine;
using UnityEngine.Events;

public enum BattleState
{
    NoBattle,
    PlayerTurn,
    EnemyTurn,
    Win,
    Lose
}

namespace Core.Game
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] int _currentTurn;
        [SerializeField] BattleState _currentBattleState;
        [SerializeField] List<CharacterStat> _currentCharactersInBattle=new List<CharacterStat>();

        [SerializeField] CharacterStat _playerCharacter;
        [SerializeField] CharacterStat _currentEnemy;

        public UnityAction OnBattleStarted;
        public UnityAction OnBattleEnded;

        public static BattleManager Instance;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            _currentBattleState = BattleState.NoBattle;
        }
        /// <summary>
        /// Function to get player character object
        /// </summary>
        public Transform GetPlayerObject()
        {
            return _playerCharacter.transform;
        }
        /// <summary>
        /// Function to get battle state
        /// </summary>
        public string GetTurnString()
        {
            return _currentBattleState.ToString();
        }
        /// <summary>
        /// Change battle state
        /// </summary>
        public void ChangeBattleState(BattleState state)
        {
            _currentBattleState = state;
            BattleGUIManager.Instance.UpdateTurnUI();
        }
        /// <summary>
        /// Function start battle
        /// </summary>
        public void StartBattle(CharacterStat _enemyCharacter,bool enemyInitiate = false)
        {
            _currentCharactersInBattle.Clear();
            _currentTurn = 0;
            _currentEnemy = _enemyCharacter;

            //invoke event to stop characters from moving
            OnBattleStarted?.Invoke();

            if (enemyInitiate)
            {
                //enemy start attacking
                _currentCharactersInBattle.Add(_enemyCharacter);
                _currentCharactersInBattle.Add(_playerCharacter);
                ChangeBattleState(BattleState.EnemyTurn);
                Debug.Log("Enemy started battle");
            }
            else
            {
                //player start attacking
                _currentCharactersInBattle.Add(_playerCharacter);
                _currentCharactersInBattle.Add(_enemyCharacter);
                ChangeBattleState(BattleState.PlayerTurn);
                Debug.Log("Player started battle");
            }

            //open battle splashscreen, its automatically close after animation
            PanelManager.Instance.OpenPanel("BattleSplashscreen");
            //add event setup battle after animation end
            PanelManager.Instance.OnPanelAnimationComplete += SetupBattle;
        }
        /// <summary>
        /// Setup battle after battle splashscreen animation end
        /// </summary>
        public void SetupBattle()
        {
            //remove event
            PanelManager.Instance.OnPanelAnimationComplete -= SetupBattle;
            //open battle gui panel
            PanelManager.Instance.OpenPanel("BattleGUI");

            //move enemy character to battle position
            Vector3 attackPos = _playerCharacter.transform.position + Vector3.right * 10f;
            _currentEnemy.transform.position = attackPos;

            //setup battle GUI
            BattleGUIManager.Instance.SetupBattleGUI(_playerCharacter,_currentEnemy);

            //if enemy first turn then enemy start attack
            if(_currentBattleState == BattleState.EnemyTurn)
            {
                StartCoroutine(EnemyAttack());
            }
        }

        #region Battle System
        public void OnPlayerAttack()
        {
            if(_currentBattleState != BattleState.PlayerTurn)
                return;
            StartCoroutine(PlayerAttack());
        }

        IEnumerator PlayerAttack()
        {
            float damage = _playerCharacter.GetCurrentAttack();
            //player attack animation (NOT DONE)
            //enemy take damage animation (NOT DONE)

            _currentEnemy.TakeDamage(damage);
            //check win condition
            if (_currentEnemy.GetCurrentHP() <= 0)
            {
                ChangeBattleState(BattleState.Win);
                Debug.Log("You Win!");
                yield break;
            }

            yield return new WaitForSeconds(1f);
            //change turn
            ChangeBattleState(BattleState.EnemyTurn);
            BattleGUIManager.Instance.UpdateTurnUI();
            StartCoroutine(EnemyAttack());
        }

        IEnumerator EnemyAttack()
        {
            yield return new WaitForSeconds(1f);
            float damage = _currentEnemy.GetCurrentAttack();
            //enemy attack animation (NOT DONE)
            //player take damage animation (NOT DONE)

            _playerCharacter.TakeDamage(damage);
            //check lose condition
            if (_playerCharacter.GetCurrentHP() <= 0)
            {
                ChangeBattleState(BattleState.Lose);
                Debug.Log("You Lose!");
                yield break;
            }
            //change turn
            ChangeBattleState(BattleState.PlayerTurn);
            BattleGUIManager.Instance.UpdateTurnUI();
        }
        #endregion
    }
}

