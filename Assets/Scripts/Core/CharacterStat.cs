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
        
        public void SetupCharacter(CharacterDetailSO characterDetail)
        {
            characterDetailSO = characterDetail;
            _currentHP = characterDetail.HP;
            _currentDef = characterDetail.Def;
            _currentAttack = characterDetail.Attack;
            _currentMana = characterDetail.Mana;
            _currentSpeed = characterDetail.Speed;
        }
    }
}

