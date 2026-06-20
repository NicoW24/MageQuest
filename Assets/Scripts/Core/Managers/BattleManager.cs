using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UI.Game;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

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
        [SerializeField] BattleState _currentBattleState;
        bool _playerCanDoAction = true;//prevent player spamming action
        [SerializeField] List<CharacterStat> _currentCharactersInBattle=new List<CharacterStat>();

        [SerializeField] CharacterStat _playerCharacter;
        CharacterController _playerCharController;
        Vector3 _playerStartBattlePos;
        [SerializeField] CharacterStat _currentEnemy;
        CharacterController _currentEnemyCharController;
        Vector3 _EnemyStartBattlePos;

        public UnityAction OnBattleStarted;
        public UnityAction OnBattleEnded;

        public static BattleManager Instance;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            _currentBattleState = BattleState.NoBattle;
            //set player controller var
            _playerCharController = _playerCharacter.GetController();
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
            _currentEnemy = _enemyCharacter;
            //set current enemy controller var
            _currentEnemyCharController = _enemyCharacter.GetController();

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

            //set battle start position foreach char
            _EnemyStartBattlePos = _currentEnemyCharController.transform.position;
            _playerStartBattlePos = _playerCharController.transform.position;

            //setup battle GUI
            BattleGUIManager.Instance.SetupBattleGUI(_playerCharacter,_currentEnemy);

            //if enemy first turn then enemy start attack
            if(_currentBattleState == BattleState.EnemyTurn)
            {
                _playerCanDoAction = false;
                StartCoroutine(EnemyAttack());
            }
        }
        void BattleEnded()
        {
            //execute event
            OnBattleEnded?.Invoke();
            //close battle ui
            PanelManager.Instance.ClosePanel("BattleGUI");
            //let player loot enemy for skill (NOT DONE)
        }

        #region Battle System
        #region Player Turn
        /// <summary>
        /// Function after player press attack button
        /// </summary>
        public void OnPlayerAttack()
        {
            if(_currentBattleState != BattleState.PlayerTurn || !_playerCanDoAction)
                return;
            StartCoroutine(PlayerAttack());
        }
        /// <summary>
        /// Coroutine for player attack turn
        /// </summary>
        IEnumerator PlayerAttack()
        {
            _playerCanDoAction = false;
            float damage = _playerCharacter.GetCurrentAttack();

            //player attack animation
            //wait for player reach enemy
            while (Vector2.Distance(_playerCharacter.transform.position,_currentEnemy.transform.position) > 3f)
            {
                _playerCharController.MoveTowards(_currentEnemy.transform.position, 5f);
                yield return null;
            }
            //set attack animation
            _playerCharController.Attack();
            while (!_playerCharController.AnimationDone())
                yield return null;
            //enemy take damage animation
            _currentEnemyCharController.TakeDamage();
            _currentEnemy.TakeDamage(damage);
            //check win condition
            if (_currentEnemy.GetCurrentHP() <= 0)
            {
                ChangeBattleState(BattleState.Win);
                _currentEnemyCharController.Death();
                BattleEnded();
                yield break;
            }
            //add delay before player go back to start pos
            yield return new WaitForSeconds(1.5f);
            //wait for player reach starting pos
            while (Vector2.Distance(_playerCharacter.transform.position, _playerStartBattlePos) > 0f)
            {
                _playerCharController.MoveTowards(_playerStartBattlePos, 5f);
                yield return null;
            }
            //make player char idle and flip sprite to be starting pos
            _playerCharController.Idle();


            yield return new WaitForSeconds(1f);
            //change turn
            ChangeBattleState(BattleState.EnemyTurn);
            BattleGUIManager.Instance.UpdateTurnUI();
            StartCoroutine(EnemyAttack());
        }
        #endregion
        #region Enemy Turn
        /// <summary>
        /// Coroutine for enemy attack turn
        /// </summary>
        IEnumerator EnemyAttack()
        {
            yield return new WaitForSeconds(1f);
            float damage = _currentEnemy.GetCurrentAttack();
            //enemy attack animation
            //wait for enemy reach player
            while (Vector2.Distance(_currentEnemy.transform.position, _playerCharacter.transform.position) > 3f)
            {
                _currentEnemyCharController.MoveTowards(_playerCharacter.transform.position, 5f);
                yield return null;
            }
            //set attack animation
            _currentEnemyCharController.Attack();
            while (!_currentEnemyCharController.AnimationDone())
                yield return null;
            //player take damage animation
            _playerCharController.TakeDamage();
            _playerCharacter.TakeDamage(damage);
            //check lose condition
            if (_playerCharacter.GetCurrentHP() <= 0)
            {
                ChangeBattleState(BattleState.Lose);
                _playerCharController.Death();
                BattleEnded();
                yield break;
            }
            //add delay before enemy go back to start pos
            yield return new WaitForSeconds(1.5f);
            //wait for enemy reach starting pos
            while (Vector2.Distance(_currentEnemy.transform.position, _EnemyStartBattlePos) > 0f)
            {
                _currentEnemyCharController.MoveTowards(_EnemyStartBattlePos, 5f);
                yield return null;
            }
            Debug.Log(Vector2.Distance(_currentEnemy.transform.position, _EnemyStartBattlePos));
            //make enemy char idle and flip sprite to be starting pos
            _currentEnemyCharController.Idle();

            //change turn
            ChangeBattleState(BattleState.PlayerTurn);
            BattleGUIManager.Instance.UpdateTurnUI();
            _playerCanDoAction = true;
        }
        #endregion
        #endregion
    }
}

