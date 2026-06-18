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

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                _currentEnemyInTrigger = other.GetComponent<CharacterStat>();
            }
        }

        public CharacterStat GetCurrentEnemy()
        {
            return _currentEnemyInTrigger;
        }

        public void BattleEnded()
        {
            _currentEnemyInTrigger = null;
        }
    }
}

