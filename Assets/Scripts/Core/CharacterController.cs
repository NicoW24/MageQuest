using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] SPUM_Prefabs _characterSPUM;
        CharacterStat _characterStat;

        protected bool _canMove;

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

            _canMove = true;
            //add event battle started
            BattleManager.Instance.OnBattleStarted += BattleStarted;
            BattleManager.Instance.OnBattleEnded += BattleStarted;
        }

        /// <summary>
        /// Function to play animation through SPUM
        /// </summary>
        public void PlayStateAnimation(PlayerState state, int indexAnimation=0)
        {
            _characterSPUM.PlayAnimation(state, indexAnimation);
        }

        void OnDisable()
        {
            //remove event on disable
            BattleManager.Instance.OnBattleStarted -= BattleStarted;
            BattleManager.Instance.OnBattleEnded -= BattleStarted;
        }

        /// <summary>
        /// Character move function
        /// </summary>
        public virtual void Move() { }
        /// <summary>
        /// Character attack function
        /// </summary>
        public virtual void Attack() { }

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


