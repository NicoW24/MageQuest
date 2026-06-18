using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    /// <summary>
    /// This script is used to track character stat in gameplay
    /// </summary>
    public class CharacterStat : MonoBehaviour
    {
        public CharacterDetailSO characterDetailSO;

        //current character stats used for battle
        [SerializeField] float _currentHP;
        [SerializeField] float _currentDef;
        [SerializeField] float _currentAttack;
        [SerializeField] float _currentMana;
        [SerializeField] float _currentSpeed;

        void Start()
        {
            //check SO
            if(characterDetailSO == null)
            {
                Debug.LogError("FORGOT TO ADD CHARACTER DETAIL DATA");
            }
        }

        /// <summary>
        /// Setup initial stat
        /// </summary>
        public void SetupCharacter()
        {
            _currentHP = characterDetailSO.HP;
            _currentDef = characterDetailSO.Def;
            _currentAttack = characterDetailSO.Attack;
            _currentMana = characterDetailSO.Mana;
            _currentSpeed = characterDetailSO.Speed;
        }
    }
}

