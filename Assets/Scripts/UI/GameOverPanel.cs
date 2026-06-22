using UnityEngine;
using UnityEngine.Events;

namespace UI.Game
{
    public class GameOverPanel : AnimationPanel
    {
        public UnityAction OnPlayAgain;

        /// <summary>
        /// Play again button function
        /// </summary>
        public void PlayAgain()
        {
            OnPlayAgain?.Invoke();
            ClosePanel();
        }
        /// <summary>
        /// Exit game button function
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
