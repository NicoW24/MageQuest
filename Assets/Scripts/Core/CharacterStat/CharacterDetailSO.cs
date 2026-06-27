using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    /// <summary>
    /// The elements used to determine weakness fire weak to ice, ice weak to nature, and nature weak to fire
    /// </summary>
    public enum Elements
    {
        Fire,
        Ice,
        Nature
    }
    /// <summary>
    /// Character type for now used only for differentiate action behaviour from mob and boss
    /// </summary>
    public enum CharacterType
    {
        Player,
        Mob,
        Boss
    }
    /// <summary>
    /// This detail is used for enemy and player (character that can do battle)
    /// </summary>
    [CreateAssetMenu(fileName = "New Character", menuName = "Game/Character")]
    public class CharacterDetailSO : ScriptableObject
    {
        //the character name
        public string characterName;
        //character element
        public Elements characterElement;
        //character type
        public CharacterType characterType;

        //character initial stats
        public float HP;
        public float Def;
        public float Attack;
        public float Mana;
    }
}
