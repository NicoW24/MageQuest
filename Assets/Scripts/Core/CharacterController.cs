using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] Animator _characterAnimator;
        [SerializeField] float _moveSpeed;
        int _dir = 1;

        void Start()
        {
            _characterAnimator = GetComponent<Animator>();
        }

        void Update()
        {
            Move();
        }

        void Move()
        {
            Vector3 dir = Vector3.zero;
            bool moving = false;
            _characterAnimator.SetBool("isRun", false);

            //check direction
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                _dir = -1;
                dir = Vector3.left;

                transform.localScale = new Vector3(_dir, 1, 1);
                moving = true;
            }
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                _dir = 1;
                dir = Vector3.right;

                transform.localScale = new Vector3(_dir, 1, 1);
                moving= true;
            }

            //set animation
            if (moving)
            {
                _characterAnimator.SetBool("isRun", true);
            }

            //move the character object
            transform.position += dir * _moveSpeed * Time.deltaTime;
        }
    }
}


