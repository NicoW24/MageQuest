using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class CharacterController : MonoBehaviour
    {
        protected SPUM_Prefabs _characterSPUM;
        CharacterStat _characterStat;

        public float moveSpeed = 5;
        protected bool _canMove;
        protected bool _isAlive = true;//another variable to stop update function that controls the character when character is not alive 
        protected float _initialSpriteDir;//starting char sprite direction
        protected Vector2 _initialCharPos;

        [Header("Effects")]
        public Transform effectContainer;
        protected ParticleSystem _currentUsedParticle;
        protected Dictionary<string, ParticleSystem> _spawnedParticles = new Dictionary<string, ParticleSystem>();

        public Dictionary<PlayerState, int> IndexPair = new();

        public virtual void Start()
        {
            //init character stat
            _characterStat = GetComponent<CharacterStat>();
            _characterStat.SetupCharacter();

            //init SPUM plugin
            if (_characterSPUM == null)
            {
                _characterSPUM = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
                if (!_characterSPUM.allListsHaveItemsExist())
                {
                    _characterSPUM.PopulateAnimationLists();
                }
                _characterSPUM.OverrideControllerInit();
            }
            _initialSpriteDir = _characterSPUM.transform.localScale.x;
            _initialCharPos = _characterSPUM.transform.position;
            _canMove = true;
            AddBattleEvent();
        }

        void AddBattleEvent()
        {
            //add event battle started
            BattleManager.Instance.OnBattleStarted += BattleStarted;
            BattleManager.Instance.OnBattleEnded += BattleEnded;
            //reset pos and animation
            BattleManager.Instance.OnPlayAgain += RestartCharacter;
        }

        /// <summary>
        /// Remove event function
        /// </summary>
        void RemoveBattleEvent()
        {
            //remove event on disable
            BattleManager.Instance.OnBattleStarted -= BattleStarted;
            BattleManager.Instance.OnBattleEnded -= BattleEnded;
            //reset pos and animation
            BattleManager.Instance.OnPlayAgain -= RestartCharacter;
        }

        /// <summary>
        /// Remove event on disable
        /// </summary>
        void OnDisable()
        {
            RemoveBattleEvent();
        }

        /// <summary>
        /// Function to play animation through SPUM
        /// </summary>
        public void PlayStateAnimation(PlayerState state, int indexAnimation = 0)
        {
            _characterSPUM.PlayAnimation(state, indexAnimation);
        }

        /// <summary>
        /// Character automatic move function
        /// </summary>
        public virtual void MoveTowards(Vector2 destination, float speed = 0)
        {
            Vector2 dir = (destination - (Vector2)transform.position).normalized;

            //if no input use default movespeed
            if(speed == 0)
            {
                speed = moveSpeed;
            }

            //move character
            transform.position = Vector2.MoveTowards(
                transform.position,
                destination,
                speed * Time.deltaTime
            );

            //set move
            PlayStateAnimation(PlayerState.MOVE);

            //flip sprite
            FlipSprite(dir.x);
        }

        /// <summary>
        /// Flip sprite function, the default sprite in this project is looking left side but the player is right side
        /// </summary>
        public virtual void FlipSprite(float xdir)
        {
            //flip sprite
            if (xdir > 0.01f)
                _characterSPUM.transform.localScale = new Vector3(-_initialSpriteDir, 1, 1);
            else if (xdir < -0.01f)
                _characterSPUM.transform.localScale = new Vector3(_initialSpriteDir, 1, 1);
        }

        /// <summary>
        /// Check animation done playing
        /// </summary>
        public bool AnimationDone()
        {
            return _characterSPUM._anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
        }
        public virtual void Idle()
        {
            PlayStateAnimation(PlayerState.IDLE);
            //reset sprite rotation
            _characterSPUM.transform.localScale = new Vector3(_initialSpriteDir, 1, 1);
        }
        /// <summary>
        /// Character attack function
        /// </summary>
        public virtual void Attack(int animationIndex=0)
        {
            PlayStateAnimation(PlayerState.ATTACK, animationIndex);
            PlayStateAnimation(PlayerState.IDLE);
        }
        /// <summary>
        /// Character defend function
        /// </summary>
        public virtual void Defend(){}
        /// <summary>
        /// Character take damage function
        /// </summary>
        public virtual void TakeDamage()
        {
            PlayStateAnimation(PlayerState.DAMAGED, 0);
        }
        /// <summary>
        /// Character dead function
        /// </summary>
        public virtual void Death()
        {
            PlayStateAnimation(PlayerState.DEATH, 0);

            //disable collider and rigidbody
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            _isAlive = false;
        }
        public void RestartCharacter()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            //reset animation
            PlayStateAnimation(PlayerState.IDLE);
            //reset pos and variables
            transform.position = _initialCharPos;
            _isAlive = true;
            _canMove = true;
            //reenable collider and rigidbody
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        public bool IsAlive()
        {
            return _isAlive;
        }
        public void BattleStarted()
        {
            if (!IsAlive())
            {
                return;
            }
            _canMove = false;
            //set idle
            PlayStateAnimation(PlayerState.IDLE);
        }
        public void BattleEnded()
        {
            if (!IsAlive())
            {
                return;
            }
            _canMove = true;
            //set idle
            PlayStateAnimation(PlayerState.IDLE);
        }

        #region Coroutine for particle and animation
        /// <summary>
        /// Coroutine to disable particle system after particle died
        /// </summary>
        protected IEnumerator DisableParticleAfterPlay()
        {
            yield return new WaitUntil(() => !_currentUsedParticle.IsAlive(true));
            _currentUsedParticle.gameObject.SetActive(false);
        }
        /// <summary>
        /// Coroutine to add delay after playing animation before action
        /// </summary>
        protected IEnumerator WaitForAnimationToSpawn(float sec, Action callback)
        {
            yield return new WaitForSeconds(sec);
            callback?.Invoke();
        }
        /// <summary>
        /// Coroutine wait for animation
        /// </summary>
        protected IEnumerator WaitForAnimation(Action callback)
        {
            yield return new WaitUntil(AnimationDone);
            callback?.Invoke();
        }
        #endregion
    }
}


