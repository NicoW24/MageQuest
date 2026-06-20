using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class EnemyCharacterController : CharacterController
    {
        [SerializeField] float patrolDistance = 3f;
        public float patrolWaitTime = 1.5f;
        bool _waiting;
        float _waitTimer;

        [Header("Detection")]
        [SerializeField] Transform _player;
        [SerializeField] float _detectionRadius = 5f;

        [Header("Battle")]
        public float _stoppingDistance = 1.2f;
        CharacterStat _thisCharacterStat;

        private Vector2 _APos;
        private Vector2 _BPos;
        private Vector2 _targetPos;

        public override void Start()
        {
            base.Start();

            _thisCharacterStat = GetComponent<CharacterStat>();

            //setup patrol position
            _APos = transform.position + Vector3.left * patrolDistance;
            _BPos = transform.position + Vector3.right * patrolDistance;

            _targetPos = _BPos;
            _player = BattleManager.Instance.GetPlayerObject();
        }

        void Update()
        {
            if (!_canMove)
            {
                return;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, _player.position);
            if (distanceToPlayer <= _detectionRadius)
            {
                Chase();
            }
            else
            {
                Patrol();
            }
        }

        /// <summary>
        /// AI patrol function
        /// </summary>
        void Patrol()
        {
            if (_waiting)
            {
                //set idle
                PlayStateAnimation(PlayerState.IDLE);

                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                    _waiting = false;
                return;
            }

            MoveTowards(_targetPos, moveSpeed);

            if (Vector2.Distance(transform.position, _targetPos) < 0.1f)
            {
                _waiting = true;
                _waitTimer = patrolWaitTime;

                _targetPos = (_targetPos == _APos) ? _BPos : _APos;
            }
        }
        /// <summary>
        /// AI chase function
        /// </summary>
        void Chase()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, _player.position);

            if (distanceToPlayer > _stoppingDistance)
            {
                MoveTowards(_player.position, moveSpeed * 1.5f);
            }
            else if (distanceToPlayer < _stoppingDistance)
            {
                //attack
                Attack();
                _canMove = false;
            }

            _waiting = false;
        }

        public override void MoveTowards(Vector2 destination, float moveSpeed)
        {
            base.MoveTowards(destination, moveSpeed);
        }

        public override void Attack()
        {
            base.Attack();
            if (_canMove)
            {
                StartCoroutine(WaitForAttackAnimation());
            }
        }
        private IEnumerator WaitForAttackAnimation()
        {
            yield return new WaitUntil(() => AnimationDone());
            BattleManager.Instance.StartBattle(_thisCharacterStat, true);
        }
    }
}

