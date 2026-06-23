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
        CharacterController _characterController;

        //current character stats used for battle
        [SerializeField] float _currentHP;
        [SerializeField] float _currentDef;
        [SerializeField] float _currentAttack;
        [SerializeField] float _currentMana;
        Elements _currentElement;

        public List<CharacterSkillSO> listOwnedSkill = new List<CharacterSkillSO>();

        public bool isDead = false;

        void Awake()
        {
            //check SO
            if (characterDetailSO == null)
            {
                Debug.LogError("NO CHARACTER DETAIL DATA");
            }
            _characterController = GetComponent<CharacterController>();
        }

        void Start()
        {
            //reset stat
            BattleManager.Instance.OnPlayAgain += SetupCharacter;
        }

        /// <summary>
        /// Change character element
        /// </summary>
        public void ChangeElement(Elements el)
        {
            _currentElement = el;
        }
        /// <summary>
        /// Skill weakness check, if weakness ad 2x damage value
        /// </summary>
        public bool IsSkillWeakness(Elements skillElement)
        {
            switch (skillElement)
            {
                case Elements.Fire:
                    return _currentElement == Elements.Ice;
                case Elements.Ice:
                    return _currentElement == Elements.Nature;
                case Elements.Nature:
                    return _currentElement == Elements.Fire;
            }
            return false;
        }
        /// <summary>
        /// Take damage function
        /// </summary>
        public void TakeDamage(float damageValue,bool defend = false)
        {
            float damageIN = damageValue;
            if (defend)
            {
                damageIN = damageValue - _currentDef;
                if (damageIN < 0)
                    damageIN = 1;
            }
            _currentHP -= damageIN;
            BattleGUIManager.Instance.UpdateHPUI();

            if(_currentHP <= 0)
            {
                isDead = true;
            }
        }
        public float GetMaxHP()
        {
            return characterDetailSO.HP;
        }
        public float GetCurrentHP()
        {
            return _currentHP;
        }
        public float GetCurrentMana()
        {
            return _currentMana;
        }
        public float GetCurrentAttack()
        {
            return _currentAttack;
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
            _currentElement = characterDetailSO.characterElement;
        }
        /// <summary>
        /// Return character controller, use for battle
        /// </summary>
        public CharacterController GetController()
        {
            return _characterController;
        }
        /// <summary>
        /// Add new skill
        /// </summary>
        public void AddSkill(CharacterSkillSO skill)
        {
            listOwnedSkill.Add(skill);
        }
    }
}

