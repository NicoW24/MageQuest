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
        int currentTurn = 0;
        [SerializeField] BattleState _currentBattleState;
        bool _playerCanDoAction = true;//prevent player spamming action
        [SerializeField] List<CharacterStat> _currentCharactersInBattle=new List<CharacterStat>();
        public List<CharacterSkillSO> listAllCharacterSkill = new List<CharacterSkillSO>();

        [SerializeField] CharacterStat _playerCharacter;
        CharacterController _playerCharController;
        Vector3 _playerStartBattlePos;
        [SerializeField] CharacterStat _currentEnemy;
        CharacterController _currentEnemyCharController;
        Vector3 _EnemyStartBattlePos;

        [SerializeField] LootChestObject _lootChestObject;

        bool _playerDefend = false;

        public UnityAction OnBattleStarted;
        public UnityAction OnBattleEnded;
        public UnityAction OnPlayAgain;

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
            //get game over panel to add playagain function
            PanelManager.Instance.GetPanel("GameOverSplashscreen").GetComponent<GameOverPanel>().OnPlayAgain += PlayAgain;

            //disable chest object
            _lootChestObject.HideChest();
        }
        /// <summary>
        /// Function to get player character object
        /// </summary>
        public Transform GetPlayerObject()
        {
            return _playerCharacter.transform;
        }
        /// <summary>
        /// Function to get player character stat
        /// </summary>
        public CharacterStat GetPlayerStat()
        {
            return _playerCharacter;
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
            currentTurn++;
        }
        /// <summary>
        /// Function start battle
        /// </summary>
        public void StartBattle(CharacterStat _enemyCharacter,bool enemyInitiate = false)
        {
            //set turn count
            currentTurn = 0;
            _playerCanDoAction = true;

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
        /// <summary>
        /// When player pick play again restart all character pos and stat
        /// </summary>
        void PlayAgain()
        {
            OnPlayAgain?.Invoke();
        }
        /// <summary>
        /// Battle ended function
        /// </summary>
        void BattleEnded()
        {
            //close battle ui
            PanelManager.Instance.ClosePanel("BattleGUI");
            if (_currentBattleState == BattleState.Lose)
            {
                //open gameover panel
                PanelManager.Instance.OpenPanel("GameOverSplashscreen");
            }
            else
            {
                //execute event, currently the event is resuming player control and alive enemy AI patrol and chase
                OnBattleEnded?.Invoke();

                //disable enemy body
                _currentEnemy.gameObject.SetActive(false);
                //move chest to enemy body
                _lootChestObject.SetupChest(_currentEnemy.transform.position);
            }
        }

        #region Battle System
        /// <summary>
        /// Check if battle ended to stop coroutine
        /// </summary>
        /// <returns></returns>
        bool IsBattleEnded()
        {
            return _currentBattleState == BattleState.Lose|| _currentBattleState == BattleState.Win;
        }
        /// <summary>
        /// Give damage to character function
        /// </summary>
        IEnumerator GiveDamage(float damage, CharacterStat damageTo)
        {
            //char take damage animation
            damageTo.GetController().TakeDamage();
            if (damageTo == _currentEnemy)
                damageTo.TakeDamage(damage, true);//enemy always use defense stat
            else
                damageTo.TakeDamage(damage, _playerDefend);//player only defend after action

            //stop player defend after taken damage, if player is defending
            if (_playerDefend)
                _playerDefend = false;
            //check win condition
            if (damageTo.GetCurrentHP() <= 0)
            {
                if (damageTo == _currentEnemy)
                    ChangeBattleState(BattleState.Win);
                else
                    ChangeBattleState(BattleState.Lose);
                //add delay before death animation
                yield return new WaitForSeconds(0.5f);
                damageTo.GetController().Death();
                //wait for death animation finish
                while (!damageTo.GetController().AnimationDone())
                    yield return null;
                //delay before battle ended
                yield return new WaitForSeconds(1f);
                BattleEnded();
                yield break;
            }
        }
        #region Player Turn
        /// <summary>
        /// Function after player press action button
        /// </summary>
        public void OnPlayerAction(float actionCode)//0 attack 1 defend 2 skill
        {
            if(_currentBattleState != BattleState.PlayerTurn || !_playerCanDoAction)
                return;

            switch (actionCode)
            {
                case 0:
                    StartCoroutine(PlayerAttack());
                    break;
                case 1:
                    StartCoroutine(PlayerDefend());
                    break;
            }
        }
        /// <summary>
        /// Function after player press a skill
        /// </summary>
        public void OnPlayerSkillAction(CharacterSkillSO skill)
        {
            if (_currentBattleState != BattleState.PlayerTurn || !_playerCanDoAction)
                return;

            StartCoroutine(PlayerSkill(skill));
        }
        /// <summary>
        /// Coroutine for player attack turn
        /// </summary>
        IEnumerator PlayerAttack()
        {
            _playerCanDoAction = false;
            float damage = _playerCharacter.GetCurrentAttack();
            //player attack animation
            //move player to enemy
            while (Vector2.Distance(_playerCharacter.transform.position,_currentEnemy.transform.position) > 3f)
            {
                _playerCharController.MoveTowards(_currentEnemy.transform.position, 5f);
                yield return null;
            }
            //set attack animation
            _playerCharController.Attack();
            while (!_playerCharController.AnimationDone())
                yield return null;
            //player gain mana after basic attack
            _playerCharacter.AddMana(20);
            //player give damage to enemy
            StartCoroutine(GiveDamage(damage, _currentEnemy));
            //stop the loop if battle ended either win or lose
            if (IsBattleEnded())
                yield break;

            //add delay after giving damage
            yield return new WaitForSeconds(1.2f);
            //move player to starting pos
            while (Vector2.Distance(_playerCharacter.transform.position, _playerStartBattlePos) > 0f)
            {
                _playerCharController.MoveTowards(_playerStartBattlePos, 5f);
                yield return null;
            }
            //make player char idle and flip sprite to be starting pos
            _playerCharController.Idle();

            //delay to next turn
            yield return new WaitForSeconds(0.5f);
            //change turn
            ChangeBattleState(BattleState.EnemyTurn);
            StartCoroutine(EnemyAttack());
        }
        /// <summary>
        /// Coroutine for player defend turn
        /// </summary>
        IEnumerator PlayerDefend()
        {
            _playerCanDoAction = false;

            //player defend animation
            //set defend animation
            _playerCharController.Defend();
            //enable player defend
            _playerDefend = true;
            while (!_playerCharController.AnimationDone())
                yield return null;
            //make player char idle and flip sprite to be starting pos
            _playerCharController.Idle();

            //delay to next turn
            yield return new WaitForSeconds(0.5f);
            //change turn
            ChangeBattleState(BattleState.EnemyTurn);
            StartCoroutine(EnemyAttack());
        }
        /// <summary>
        /// Coroutine for player skill turn
        /// </summary>
        IEnumerator PlayerSkill(CharacterSkillSO skill)
        {
            _playerCanDoAction = false;

            //player skill animation
            MinigameManager currentMinigame;
            string minigamePanel;
            //let player play minigame
            if(skill.minigame == SkillMinigame.Sequence)
            {
                currentMinigame = SequenceMinigameManager.Instance;
                minigamePanel = "SequenceMinigame";
                PanelManager.Instance.OpenPanel(minigamePanel);
                SequenceMinigameManager.Instance.StartSequenceMinigame();
            }
            else
            {
                currentMinigame = MashButtonMinigameManager.Instance;
                minigamePanel = "MashbuttonMinigame";
                PanelManager.Instance.OpenPanel(minigamePanel);
                MashButtonMinigameManager.Instance.StartMashButtonMinigame();
            }

            //wait for minigame ended
            while (currentMinigame.MinigameFinish())
            {
                yield return null;
            }

            //close minigame panel
            PanelManager.Instance.ClosePanel(minigamePanel);
            //if win skill execute
            if (currentMinigame.IsPlayerWin())
            {
                //set skill cast animation
                _playerCharController.Attack(skill.skillAttackAnimationIndex);
                //wait for skill cast animation
                while (!_playerCharController.AnimationDone())
                    yield return null;
                //spawn skill particle system either shoot from player or spawn at enemy
                _playerCharController.SpawnParticleSkill(skill.skillParticle,skill.shouldShoot,_currentEnemyCharController);
                //wait until particle reach enemy
                yield return new WaitForSeconds(skill.skillParticle.main.duration);
                //skill damage
                bool weakness = _currentEnemy.IsSkillWeakness(skill.skillElement);
                float damage = _playerCharacter.GetCurrentAttack() + skill.damage;
                //mana cost
                _playerCharacter.DecreaseMana(skill.manaCost);
                //if skill weakness of enemy add more damage
                if (weakness)
                {
                    damage = damage * 2;
                    //add skip enemy turn
                }
                //check effect
                if(skill.effect == SkillEffect.Stun)
                {
                    //make enemy skip turn according to effect turn 
                }
                else if(skill.effect == SkillEffect.DOT)
                {
                    //make enemy take dot damage according to effect turn 
                }

                //player give damage to enemy
                StartCoroutine(GiveDamage(damage, _currentEnemy));
                //stop the loop if battle ended either win or lose
                if (IsBattleEnded())
                    yield break;
            }
            //else lose skill not executed and player skipped turn
            else
            {
                //play animation
                _playerCharController.Stun();
                //wait for animation finished
                while (!_playerCharController.AnimationDone())
                    yield return null;
            }

            //delay for animation let it loop a bit
            yield return new WaitForSeconds(1f);
            //make player char idle and flip sprite to be starting pos
            _playerCharController.Idle();
            //change turn
            ChangeBattleState(BattleState.EnemyTurn);
            StartCoroutine(EnemyAttack());
        }
        #endregion
        #region Enemy Turn
        /// <summary>
        /// Coroutine for enemy attack turn
        /// </summary>
        IEnumerator EnemyAttack()
        {
            //delay
            yield return new WaitForSeconds(0.5f);
            float damage = _currentEnemy.GetCurrentAttack();
            #region Basic Attack
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

            //enemy give damage to player
            StartCoroutine(GiveDamage(damage, _playerCharacter));
            //stop the loop if battle ended either win or lose
            if (IsBattleEnded())
                yield break;

            //add delay after giving damage
            yield return new WaitForSeconds(1.2f);
            //wait for enemy reach starting pos
            while (Vector2.Distance(_currentEnemy.transform.position, _EnemyStartBattlePos) > 0f)
            {
                _currentEnemyCharController.MoveTowards(_EnemyStartBattlePos, 5f);
                yield return null;
            }
            //make enemy char idle and flip sprite to be starting pos
            _currentEnemyCharController.Idle();
            #endregion

            //change turn
            ChangeBattleState(BattleState.PlayerTurn);
            BattleGUIManager.Instance.UpdateTurnUI();
            _playerCanDoAction = true;
        }
        #endregion
        #endregion
    }
}

