using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Game
{
    public class PlayerCharacterController : CharacterController
    {
        [Header("Interactable")]
        [SerializeField] InteractableObject _currentInteractableObject;

        [Header("Battle")]
        [SerializeField] PlayerWeaponObject _weaponObject;

        [Header("Effects")]
        [SerializeField] ParticleSystem _defenseParticle;

        public override void Start()
        {
            base.Start();

            //setup character particle
            ParticleSystem def = Instantiate(_defenseParticle, effectContainer);
            _spawnedParticles.Add(_defenseParticle.name, def);
            def.gameObject.SetActive(false);
        }

        void Update()
        {
            if (!_canMove || !_isAlive)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayerInteract();
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
        /// <summary>
        /// Player flip sprite function
        /// </summary>
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
        public override void Attack(int animationIndex = 0)
        {
            base.Attack(animationIndex);
            //add check before initiate battle
            if (_canMove)
            {
                StartCoroutine(WaitForAnimation(StartBattle));
            }
        }
        /// <summary>
        /// Player start battle function
        /// </summary>
        void StartBattle()
        {
            //get enemy character stat
            CharacterStat enemy = _weaponObject.GetCurrentEnemy();
            //if enemy detected start battle from player
            if (enemy != null)
            {
                BattleManager.Instance.StartBattle(enemy);
            }
        }
        /// <summary>
        /// Player defend function
        /// </summary>
        public override void Defend()
        {
            PlayStateAnimation(PlayerState.OTHER, 3);
            StartCoroutine(WaitForAnimationToSpawn(1.2f,SpawnParticleDefend));
        }
        /// <summary>
        /// Spawn player defend particle
        /// </summary>
        public void SpawnParticleDefend()
        {
            _currentUsedParticle = _spawnedParticles.Where(x => x.Key == _defenseParticle.name).FirstOrDefault().Value;
            _currentUsedParticle.gameObject.SetActive(true);
            _currentUsedParticle.Clear();
            _currentUsedParticle.Emit(1);
            StartCoroutine(DisableParticleAfterPlay());
        }
        /// <summary>
        /// Set current interactable object
        /// </summary>
        public void SetInteractableObject(InteractableObject obj)
        {
            _currentInteractableObject = obj;
        }
        /// <summary>
        /// Player interact object function
        /// </summary>
        public void PlayerInteract()
        {
            if(_currentInteractableObject == null)
            {
                return;
            }

            switch (_currentInteractableObject.interactObjectType)
            {
                case InteractObjectType.Chest:
                    _currentInteractableObject.GetChestObject().OpenChest();
                    break;
                case InteractObjectType.Npc:
                    //open dialog (NOT DONE)
                    break;
            }
        }
    }
}

