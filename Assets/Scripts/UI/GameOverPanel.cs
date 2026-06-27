using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Game
{
    public class GameOverPanel : AnimationPanel
    {
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] Button _playAgainButton;
        public UnityAction OnPlayAgain;

        public void SetupEndGameScreen()
        {
            _text.text = $"Thank You For Playing";
            _playAgainButton.gameObject.SetActive(false);
        }

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
