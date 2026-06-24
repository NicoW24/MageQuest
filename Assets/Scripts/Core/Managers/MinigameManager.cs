using UnityEngine;

namespace Core.Game
{
    public class MinigameManager : MonoBehaviour
    {
        protected float _timer;
        protected bool _isPlaying;
        protected bool _playerWin;

        /// <summary>
        /// Get minigame result
        /// </summary>
        public bool IsPlayerWin()
        {
            return _playerWin;
        }
        /// <summary>
        /// Get minigame state
        /// </summary>
        public bool MinigameFinish()
        {
            return _isPlaying;
        }
    }
}

