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

public enum PlayerAction
{
    Attack,
    Defend,
    Skill
}

public enum EnemyAction
{
    Attack,
    Skill,
    FinalBuff
}

namespace Core.Game
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] BattleState _currentBattleState;
        bool _playerCanDoAction = true;//prevent player spamming action
        [SerializeField] List<CharacterStat> _currentCharactersInBattle=new List<CharacterStat>();
        [SerializeField] List<CharacterSkillSO> _listAllCharacterSkill = new List<CharacterSkillSO>();
        public IReadOnlyList<CharacterSkillSO> ListAllCharacterSkill => _listAllCharacterSkill;

        [Header("For Dialog")]
        [SerializeField] CharacterStat _bossEnemy;

        [SerializeField] CharacterStat _playerCharacter;
        CharacterController _playerCharController;
        [SerializeField] CharacterStat _currentEnemy;
        CharacterController _currentEnemyCharController;

        [SerializeField] LootChestObject _lootChestObject;

        bool _playerDefend = false;
        bool _currentCharacterStunned = false;
        bool _currentCharacterDOT = false;

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
        void OnDestroy()
        {
            if (PanelManager.Instance != null)
            {
                var panel = PanelManager.Instance.GetPanel("GameOverSplashscreen");
                if (panel != null)
                {
                    panel.GetComponent<GameOverPanel>().OnPlayAgain -= PlayAgain;
                }
            }
        }

        #region Helper
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
        /// Function to get current enemy character stat
        /// </summary>
        public CharacterStat GetCurrentEnemy()
        {
            return _currentEnemy;
        }
        /// <summary>
        /// Function to get battle state
        /// </summary>
        public BattleState GetTurnString()
        {
            return _currentBattleState;
        }
        /// <summary>
        /// Check if battle ended to stop coroutine
        /// </summary>
        bool IsBattleEnded()
        {
            return _currentBattleState == BattleState.Lose || _currentBattleState == BattleState.Win;
        }
        #endregion

        #region Battle State Function
        /// <summary>
        /// Change battle state
        /// </summary>
        public void ChangeBattleState(BattleState state)
        {
            _currentBattleState = state;
            BattleGUIManager.Instance.UpdateTurnUI();
        }
        /// <summary>
        /// Boss battle after dialog end
        /// </summary>
        public void StartBossBattle()
        {
            //boss always has 1st turn
            StartBattle(_bossEnemy,true);
        }
        /// <summary>
        /// Function start battle
        /// </summary>
        public void StartBattle(CharacterStat _enemyCharacter,bool enemyInitiate = false)
        {
            //reset player stat on new battle
            _playerCharacter.SetupCharacter();

            //reset variable
            _currentCharacterStunned = false;
            _currentCharacterDOT = false;
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
                //if not boss let player get scroll loot
                if(_currentEnemy != _bossEnemy)
                {
                    //execute event, currently the event is resuming player control and alive enemy AI patrol and chase
                    OnBattleEnded?.Invoke();

                    //disable enemy body
                    _currentEnemy.gameObject.SetActive(false);
                    //move chest to enemy body
                    _lootChestObject.SetupChest(_currentEnemy.transform.position);
                }
                else
                {
                    PanelManager.Instance.OpenPanel("GameOverSplashscreen");
                    PanelManager.Instance.GetPanel("GameOverSplashscreen").GetComponent<GameOverPanel>().SetupEndGameScreen();
                }
            }
        }
        #endregion

        #region Battle System
        #region Damage System
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
        /// <summary>
        /// Take dot status effect damage
        /// </summary>
        IEnumerator TakeDOTDamage(CharacterStat currentChar)
        {
            //floating text dot
            FloatingTextManager.Instance.ShowFloatingText("Burn", FloatingTextType.Damage, currentChar.transform);
            float dotDamage = 0;
            float skillDamage = 0;
            foreach (CharacterSkillSO dotSkill in currentChar.GetAllDOTSkillEffect())
            {
                //if weakness
                if (currentChar.IsSkillWeakness(dotSkill.skillElement))
                {
                    skillDamage = dotSkill.dotDamage * 2;
                    FloatingTextManager.Instance.ShowFloatingText("Weakness", FloatingTextType.Damage, currentChar.transform);
                }
                else
                {
                    skillDamage = dotSkill.dotDamage;
                }
                dotDamage += skillDamage;
            }
            yield return StartCoroutine(GiveDamage(dotDamage, currentChar));
        }
        #endregion
        #region Status Effect System
        /// <summary>
        /// Check active status effect on character
        /// </summary>
        void CheckActiveStatusEffectOnCharacter(CharacterStat currentCharacter)
        {
            _currentCharacterStunned = false;
            _currentCharacterDOT = false;
            //check status effect
            if (currentCharacter.GetStatusEffectDuration().Count > 0)
            {
                //check for stun
                if (currentCharacter.GetStatusEffectDuration(StatusEffect.Stun) > 0)
                {
                    _currentCharacterStunned = true;
                }
                //check for dot
                if (currentCharacter.GetStatusEffectDuration(StatusEffect.DOT) > 0)
                {
                    _currentCharacterDOT = true;
                }
            }
        }
        #endregion
        /// <summary>
        /// Coroutine for character attack
        /// </summary>
        IEnumerator CharacterAttackSequence(CharacterStat currentCharacter, CharacterStat Target)
        {
            float damage = currentCharacter.GetCurrentAttack();
            //character attack animation
            //start attack sequence
            yield return StartCoroutine(currentCharacter.GetController().PlayMeleeAttackSequence(Target.transform.position));

            //player can charge mana from basic attack
            if(currentCharacter == _playerCharacter)
            {
                currentCharacter.AddManaFromBasicAttack();
            }

            //character give damage to target
            yield return StartCoroutine(GiveDamage(damage, Target));
            //stop the loop if battle ended either win or lose
            if (IsBattleEnded())
                yield break;

            //add delay after giving damage
            yield return new WaitForSeconds(1.2f);
            //move character to starting pos
            yield return StartCoroutine(currentCharacter.GetController().ReturnToStartingBattlePosition());
        }
        /// <summary>
        /// Coroutine for character skill
        /// </summary>
        IEnumerator CharacterSkillSequence(CharacterStat currentCharacter,CharacterStat target, CharacterSkillSO skill)
        {
            //set skill cast animation
            currentCharacter.GetController().Attack(skill.skillAttackAnimationIndex);
            //wait for skill cast animation
            while (!currentCharacter.GetController().AnimationDone())
                yield return null;
            //spawn skill particle system either shoot from character or spawn at target
            currentCharacter.GetController().SpawnParticleSkill(skill.skillParticle, skill.shouldShoot, target.GetController());
            //wait until particle reach target
            yield return new WaitForSeconds(skill.skillParticle.main.duration);
            //skill damage
            bool weakness = target.IsSkillWeakness(skill.skillElement);
            float damage = currentCharacter.GetCurrentAttack() + skill.damage;
            //mana cost
            currentCharacter.DecreaseMana(skill.manaCost);
            //if skill weakness of target add more damage
            if (weakness)
            {
                damage = damage * 2;
                FloatingTextManager.Instance.ShowFloatingText("Weakness", FloatingTextType.Damage, target.transform);
            }
            //add status effect to target if available
            if (skill.effect != StatusEffect.None)
                target.AddStatusEffect(skill, skill.effectTurn);

            //player give damage to enemy
            yield return StartCoroutine(GiveDamage(damage, target));
        }
        #region Player Turn
        /// <summary>
        /// Function after player press action button
        /// </summary>
        public void OnPlayerAction(PlayerAction playerAction)
        {
            if(_currentBattleState != BattleState.PlayerTurn || !_playerCanDoAction)
                return;

            switch (playerAction)
            {
                case PlayerAction.Attack:
                    StartCoroutine(PlayerAttack());
                    break;
                case PlayerAction.Defend:
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

            //check status effect
            CheckActiveStatusEffectOnCharacter(_playerCharacter);
            //if stunned skip attack
            if (!_currentCharacterStunned)
            {
                #region Attack
                yield return StartCoroutine(CharacterAttackSequence(_playerCharacter, _currentEnemy));
                //stop coroutine if enemy died
                if (IsBattleEnded())
                    yield break;
                #endregion
            }
            else
            {
                _playerCharController.Stun();
                //wait for animation finished
                while (!_playerCharController.AnimationDone())
                    yield return null;
                //delay
                yield return new WaitForSeconds(0.5f);
            }

            //check dot
            if (_currentCharacterDOT)
            {
                yield return StartCoroutine(TakeDOTDamage(_playerCharacter));
                //stop the loop if battle ended either win or lose
                if (IsBattleEnded())
                    yield break;
            }

            //decrease status effect if affected
            _playerCharacter.DecreaseStatusEffectDuration();

            //make player char idle and flip sprite to be starting pos
            _playerCharController.Idle();
            //delay to next turn
            yield return new WaitForSeconds(0.5f);
            //change turn
            ChangeBattleState(BattleState.EnemyTurn);
            yield return StartCoroutine(EnemyAttack());
        }
        /// <summary>
        /// Coroutine for player defend turn
        /// </summary>
        IEnumerator PlayerDefend()
        {
            _playerCanDoAction = false;

            //check status effect
            CheckActiveStatusEffectOnCharacter(_playerCharacter);
            //if stunned skip defend
            if (!_currentCharacterStunned)
            {
                //player defend animation
                //set defend animation
                _playerCharController.Defend();
                //enable player defend
                _playerDefend = true;
                while (!_playerCharController.AnimationDone())
                    yield return null;
            }
            else
            {
                _playerCharController.Stun();
                //wait for animation finished
                while (!_playerCharController.AnimationDone())
                    yield return null;
                //delay
                yield return new WaitForSeconds(0.5f);
            }


            //check dot
            if (_currentCharacterDOT)
            {
                yield return StartCoroutine(TakeDOTDamage(_playerCharacter));
                //stop the loop if battle ended either win or lose
                if (IsBattleEnded())
                    yield break;
            }

            //decrease status effect if affected
            _playerCharacter.DecreaseStatusEffectDuration();

            //make player char idle and flip sprite to be starting pos
            _playerCharController.Idle();
            //delay to next turn
            yield return new WaitForSeconds(0.5f);
            //change turn
            ChangeBattleState(BattleState.EnemyTurn);
            yield return StartCoroutine(EnemyAttack());
        }
        /// <summary>
        /// Coroutine for player skill turn
        /// </summary>
        IEnumerator PlayerSkill(CharacterSkillSO skill)
        {
            _playerCanDoAction = false;

            //check status effect
            CheckActiveStatusEffectOnCharacter(_playerCharacter);
            //if stunned skip defend
            if (!_currentCharacterStunned)
            {
                //player skill animation
                MinigameManager currentMinigame;
                string minigamePanel;
                //let player play minigame
                if (skill.minigame == SkillMinigame.Sequence)
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
                while (currentMinigame.StillPlaying())
                {
                    yield return null;
                }

                //close minigame panel
                PanelManager.Instance.ClosePanel(minigamePanel);
                //if win skill execute
                if (currentMinigame.IsPlayerWin())
                {
                    yield return StartCoroutine(CharacterSkillSequence(_playerCharacter,_currentEnemy,skill));
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
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                _playerCharController.Stun();
                //wait for animation finished
                while (!_playerCharController.AnimationDone())
                    yield return null;
                //delay
                yield return new WaitForSeconds(0.5f);
            }


            //check dot
            if (_currentCharacterDOT)
            {
                yield return StartCoroutine(TakeDOTDamage(_playerCharacter));
                //stop the loop if battle ended either win or lose
                if (IsBattleEnded())
                    yield break;
            }

            //decrease status effect if affected
            _playerCharacter.DecreaseStatusEffectDuration();

            //make player char idle and flip sprite to be starting pos
            _playerCharController.Idle();
            //change turn
            ChangeBattleState(BattleState.EnemyTurn);
            yield return StartCoroutine(EnemyAttack());
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

            //check status effect
            CheckActiveStatusEffectOnCharacter(_currentEnemy);

            if (!_currentCharacterStunned)
            {
                CharacterSkillSO skill = null;

                //randomize enemy action
                EnemyAction chosenAction = EnemyAction.Attack;
                //only randomize action if enemy has skill
                if (_currentEnemy.listOwnedSkill.Count > 0)
                {
                    //randomize enemy skill
                    skill = _currentEnemy.listOwnedSkill[Random.Range(0, _currentEnemy.listOwnedSkill.Count)];
                    //use weight action
                    float randomValue = Random.value;
                    if (randomValue < 0.70f)
                    {
                        chosenAction = EnemyAction.Attack;
                    }
                    else
                    {
                        //check mana
                        if(_currentEnemy.GetCurrentMana() >= skill.manaCost)
                        {
                            chosenAction = EnemyAction.Skill;
                        }
                    }
                }

                //boss action
                if (_currentEnemy.characterDetailSO.characterType == CharacterType.Boss)
                {
                    //when skill can change character element
                    if (chosenAction == EnemyAction.Skill)
                    {
                        _currentEnemy.ChangeElement(skill.skillElement);
                        FloatingTextManager.Instance.ShowFloatingText($"Boss changed element to {skill.skillElement.ToString()}", FloatingTextType.StatusEffect, _currentEnemy.transform);
                    }

                    //when hp is low boss will get enrage
                    if(_currentEnemy.GetCurrentHP() <= 50 && !_currentEnemy.IsEnrage())
                    {
                        chosenAction = EnemyAction.FinalBuff;
                    }
                }

                #region Attack
                if (chosenAction == EnemyAction.Attack)
                {
                    yield return StartCoroutine(CharacterAttackSequence(_currentEnemy, _playerCharacter));
                }
                else if(chosenAction == EnemyAction.Skill)
                {
                    //enemy skill sequence
                    yield return StartCoroutine(CharacterSkillSequence(_currentEnemy, _playerCharacter, skill));
                }
                else
                {
                    //boss berserk
                    //set buff animation
                    _currentEnemyCharController.Buff();
                    //wait for skill cast animation
                    while (!_currentEnemyCharController.AnimationDone())
                        yield return null;

                    //delay
                    yield return new WaitForSeconds(1f);

                    //add attack buff
                    _currentEnemy.Enrage();
                }
                //stop the loop if battle ended either win or lose
                if (IsBattleEnded())
                    yield break;
                #endregion
            }
            else
            {
                _currentEnemyCharController.Stun();
                //wait for animation finished
                while (!_currentEnemyCharController.AnimationDone())
                    yield return null;
                //delay
                yield return new WaitForSeconds(0.5f);
            }

            //check dot
            if (_currentCharacterDOT)
            {
                yield return StartCoroutine(TakeDOTDamage(_currentEnemy));
                //stop the loop if battle ended either win or lose
                if (IsBattleEnded())
                    yield break;
            }

            //decrease status effect if affected
            _currentEnemy.DecreaseStatusEffectDuration();

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

