using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class CharacterController : MonoBehaviour
    {
        protected SPUM_Prefabs _characterSPUM;
        CharacterStat _characterStat;

        protected bool _canMove;
        float _initialSpriteDir;//char sprite direction 

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
        public virtual void MoveTowards(Vector2 destination, float moveSpeed)
        {
            Vector2 dir = (destination - (Vector2)transform.position).normalized;

            //move character
            transform.position = Vector2.MoveTowards(
                transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );

            //set move
            PlayStateAnimation(PlayerState.MOVE);

            //flip sprite
            if (dir.x > 0.01f)
                //right is -1 since sprite is originally looking left
                _characterSPUM.transform.localScale = new Vector3(-_initialSpriteDir, 1, 1);
            else if (dir.x < -0.01f)
                //left is 1 since sprite is originally looking left
                _characterSPUM.transform.localScale = new Vector3(_initialSpriteDir, 1, 1);
        }
        /// <summary>
        /// Character attack function
        /// </summary>
        protected virtual void Attack()
        {
            //set attack
            PlayStateAnimation(PlayerState.ATTACK, 0);
            PlayStateAnimation(PlayerState.IDLE);
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


