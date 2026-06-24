using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public enum SkillEffect
    {
        None,
        Stun,//skip character move for some turn
        DOT//damage over time, character get damaged for some turn
    }

    public enum SkillMinigame
    {
        Sequence,
        ButtonMash
    }

    /// <summary>
    /// This detail is used for skills
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill", menuName = "Game/Character Skill")]
    public class CharacterSkillSO : ScriptableObject
    {
        [Header("Skill Detail")]
        //skill name
        public string skillName;
        public string skillDesc;
        //skill element
        public Elements skillElement;//skill element, if weakness add more damage + 2x
        public float manaCost;//skill mana cost
        public float damage;//skill damage if available add value
        [Header("Skill Effect")]
        public SkillEffect effect = SkillEffect.None;//skill effect if available pick one of 2
        public int effectTurn = 0;//character affected turn
        public float dotDamage;//skill damage overtime if skill effect dot
        public SkillMinigame minigame;//skill minigame for player, player need to win the minigame to activate skill
        [Header("Skill Animation")]
        public ParticleSystem skillParticle;//skill particle effect
        public int skillAttackAnimationIndex;//casting animation index
        public bool shouldShoot;//if shoot spawn from weapon to enemy, if not shoot spawn under enemy feet
    }
}
