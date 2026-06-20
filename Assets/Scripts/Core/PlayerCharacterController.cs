using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class PlayerCharacterController : CharacterController
    {
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
                dir = Vector3.right;
                moving = true;
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                //left is 1 since sprite is originally looking left
                dir = Vector3.left;
                moving = true;
            }

            //set move
            if (moving && _canMove)
            {
                //flip sprite
                FlipSprite(dir.x);
                PlayStateAnimation(PlayerState.MOVE);
            }

            //move the character object
            transform.localPosition += dir * moveSpeed * Time.deltaTime;
        }

        public override void FlipSprite(float xdir)
        {
            //flip sprite
            if (xdir > 0.01f)
                _characterSPUM.transform.localScale = new Vector3(_initialSpriteDir, 1, 1);
            else if (xdir < -0.01f)
                _characterSPUM.transform.localScale = new Vector3(-_initialSpriteDir, 1, 1);
        }

        /// <summary>
        /// Player attack function
        /// </summary>
        public override void Attack()
        {
            base.Attack();
            //add check before initiate battle
            if (_canMove)
            {
                StartCoroutine(WaitForAttackAnimation());
            }
        }
        private IEnumerator WaitForAttackAnimation()
        {
            yield return new WaitUntil(() => AnimationDone());
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

