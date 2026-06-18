using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Game
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] int _currentTurn;
        [SerializeField] List<CharacterStat> _currentCharactersInBattle=new List<CharacterStat>();

        [SerializeField] CharacterStat _playerCharacter;

        public UnityAction OnBattleStarted;
        public UnityAction OnBattleEnded;

        public static BattleManager Instance;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        public Transform GetPlayerObject()
        {
            return _playerCharacter.transform;
        }

        /// <summary>
        /// Function start battle
        /// </summary>
        public void StartBattle(CharacterStat _enemyCharacter,bool enemyInitiate = false)
        {
            _currentCharactersInBattle.Clear();
            _currentTurn = 0;

            OnBattleStarted?.Invoke();

            switch (enemyInitiate)
            {
                case true:
                    //enemy start attacking
                    _currentCharactersInBattle.Add(_enemyCharacter);
                    _currentCharactersInBattle.Add(_playerCharacter);
                    Debug.Log("Enemy started battle");
                    break;
                case false:
                    //player start attacking
                    _currentCharactersInBattle.Add(_playerCharacter);
                    _currentCharactersInBattle.Add(_enemyCharacter);
                    Debug.Log("Player started battle");
                    break;
            }
        }
    }
}

