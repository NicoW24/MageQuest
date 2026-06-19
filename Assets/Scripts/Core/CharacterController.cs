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
        protected float _initialSpriteDir;//starting char sprite direction 

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
            _canMove = true;
            //add event battle started
            BattleManager.Instance.OnBattleStarted += BattleStarted;
            BattleManager.Instance.OnBattleEnded += BattleStarted;
        }

        /// <summary>
        /// Remove event on disable
        /// </summary>
        void OnDisable()
        {
            //remove event on disable
            BattleManager.Instance.OnBattleStarted -= BattleStarted;
            BattleManager.Instance.OnBattleEnded -= BattleStarted;
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
        public virtual void Attack()
        {
            PlayStateAnimation(PlayerState.ATTACK, 0);
            PlayStateAnimation(PlayerState.IDLE);
        }
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
        }

        public void BattleStarted()
        {
            //set idle
            PlayStateAnimation(PlayerState.IDLE);
            _canMove = false;
        }
        public void BattleEnded()
        {
            //set idle
            PlayStateAnimation(PlayerState.IDLE);
            _canMove = true;
        }
    }
}


