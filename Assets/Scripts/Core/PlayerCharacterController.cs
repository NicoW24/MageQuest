using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class PlayerCharacterController : CharacterController
    {
        [SerializeField] float _moveSpeed = 5;
        int _dir = 1;

        [Header("Battle")]
        [SerializeField] PlayerWeaponObject _weaponObject;

        void Update()
        {
            if (!_canMove)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
            }
            PlayerMove();
        }

        /// <summary>
        /// Player move function
        /// </summary>
        void PlayerMove()
        {
            Vector3 dir = Vector3.zero;
            bool moving = false;
            PlayStateAnimation(PlayerState.IDLE);

            //check direction
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                //right is -1 since sprite is originally looking left
                _dir = -1;
                dir = Vector3.right;

                _characterSPUM.transform.localScale = new Vector3(_dir, 1, 1);
                moving = true;
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                //left is 1 since sprite is originally looking left
                _dir = 1;
                dir = Vector3.left;

                _characterSPUM.transform.localScale = new Vector3(_dir, 1, 1);
                moving = true;
            }

            //set move
            if (moving && _canMove)
            {
                PlayStateAnimation(PlayerState.MOVE);
            }

            //move the character object
            transform.localPosition += dir * _moveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Player attack function
        /// </summary>
        protected override void Attack()
        {
            base.Attack();
            //get enemy character stat
            CharacterStat enemy = _weaponObject.GetCurrentEnemy();
            //if enemy detected start battle from player
            if (enemy != null)
            {
                BattleManager.Instance.StartBattle(enemy);
            }
        }
    }
}

