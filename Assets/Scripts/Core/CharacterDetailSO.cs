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
    /// This detail is used for enemy and player (character that can do battle)
    /// </summary>
    [CreateAssetMenu(fileName = "New Character", menuName = "Game/Character")]
    public class CharacterDetailSO : ScriptableObject
    {
        //the character name
        public string characterName;
        //character element
        public Elements characterElement;

        //character initial stats
        public float HP;
        public float Def;
        public float Attack;
        public float Mana;
        public float Speed;
    }
}
