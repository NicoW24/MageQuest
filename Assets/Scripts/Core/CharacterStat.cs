using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Game;
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

        Dictionary<CharacterSkillSO,int> _currentActiveStatusEffectWithDuration = new Dictionary<CharacterSkillSO, int>();//current character active status effect and its duration

        [SerializeField] List<CharacterSkillSO> _listOwnedSkill = new List<CharacterSkillSO>();
        public IReadOnlyList<CharacterSkillSO> listOwnedSkill => _listOwnedSkill;

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
        /// Get current active status effect dictionary that contains its duration
        /// </summary>
        public Dictionary<CharacterSkillSO,int> GetStatusEffectDuration()
        {
            return _currentActiveStatusEffectWithDuration;
        }
        /// <summary>
        /// Get current active status effect duration
        /// </summary>
        public int GetStatusEffectDuration(StatusEffect effect)
        {
            var statusEffect = _currentActiveStatusEffectWithDuration.Keys.Where(x=>x.effect == effect).FirstOrDefault();
            if(statusEffect == null)
            {
                return 0;
            }
            return _currentActiveStatusEffectWithDuration[statusEffect];
        }
        /// <summary>
        /// Get all active DOT
        /// </summary>
        public List<CharacterSkillSO> GetAllDOTSkillEffect()
        {
            return _currentActiveStatusEffectWithDuration
                .Where(x => x.Key.effect == StatusEffect.DOT)
                .Select(x => x.Key)
                .ToList();
        }
        /// <summary>
        /// Add status effect to character
        /// </summary>
        public void AddStatusEffect(CharacterSkillSO statusEffect,int turnDuration)
        {
            //check if already affected dont add
            if (_currentActiveStatusEffectWithDuration.ContainsKey(statusEffect))
            {
                return;
            }

            _currentActiveStatusEffectWithDuration.Add(statusEffect, turnDuration);
        }
        /// <summary>
        /// Decrease status effect duration in character and remove if expired
        /// </summary>
        public void DecreaseStatusEffectDuration()
        {
            if(_currentActiveStatusEffectWithDuration.Count == 0)
            {
                return;
            }

            List<CharacterSkillSO> listExpiredStatusEffect = new List<CharacterSkillSO>();
            foreach (var skill in _currentActiveStatusEffectWithDuration.Keys.ToList())
            {
                _currentActiveStatusEffectWithDuration[skill]--;
                if (_currentActiveStatusEffectWithDuration[skill] <= 0)
                {
                    listExpiredStatusEffect.Add(skill);
                }
            }
            foreach (CharacterSkillSO statusEffect in listExpiredStatusEffect)
            {
                _currentActiveStatusEffectWithDuration.Remove(statusEffect);
            }
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
            switch (_currentElement)
            {
                case Elements.Fire:
                    return skillElement == Elements.Ice;
                case Elements.Ice:
                    return skillElement == Elements.Nature;
                case Elements.Nature:
                    return skillElement == Elements.Fire;
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
            //floating text
            FloatingTextManager.Instance.ShowFloatingText(Mathf.CeilToInt(damageIN).ToString(), FloatingTextType.Damage, transform);
        }
        /// <summary>
        /// Skill use mana
        /// </summary>
        public void DecreaseMana(float value)
        {
            _currentMana = _currentMana - value;
            BattleGUIManager.Instance.UpdateManaUI();
        }
        /// <summary>
        /// Basic attack add mana
        /// </summary>
        public void AddManaFromBasicAttack()
        {
            AddMana(20);
        }
        public void AddMana(float value)
        {
            if(_currentMana == GetMaxMana())
            {
                return;
            }
            _currentMana = _currentMana + value;
            BattleGUIManager.Instance.UpdateManaUI();
            //floating text
            FloatingTextManager.Instance.ShowFloatingText("20", FloatingTextType.GainMana, transform);
        }
        public float GetMaxHP()
        {
            return characterDetailSO.HP;
        }
        public float GetCurrentHP()
        {
            return _currentHP;
        }
        public float GetMaxMana()
        {
            return characterDetailSO.Mana;
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

            //remove all effect
            _currentActiveStatusEffectWithDuration.Clear();
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
            _listOwnedSkill.Add(skill);
            //add skill to panel
            PlayerSkillActionPanelManager.Instance.AddSkill(skill);
        }
    }
}

