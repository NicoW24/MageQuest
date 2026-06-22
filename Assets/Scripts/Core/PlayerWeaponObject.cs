using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class PlayerWeaponObject : MonoBehaviour
    {
        CharacterStat _currentEnemyInTrigger;

        void Start()
        {
            BattleManager.Instance.OnBattleEnded += BattleEnded;
        }
        /// <summary>
        /// Collider detection for enemy
        /// </summary>
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                CharacterStat stat = other.GetComponent<CharacterStat>();
                if (!stat.isDead)
                {
                    _currentEnemyInTrigger = stat;
                }
            }
        }
        /// <summary>
        /// Return current detected enemy
        /// </summary>
        public CharacterStat GetCurrentEnemy()
        {
            return _currentEnemyInTrigger;
        }
        /// <summary>
        /// When battle ended reset current enemy variable
        /// </summary>
        public void BattleEnded()
        {
            _currentEnemyInTrigger = null;
        }
    }
}

