using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game
{
    public class MashButtonMinigameManager : MinigameManager
    {
        [SerializeField] Slider _progressSlider;
        [SerializeField] TextMeshProUGUI _timerText;

        [Header("Minigame Variable Mechanic")]
        public KeyCode mashKey = KeyCode.Space;
        int _pressNeeded = 50;

        int _pressCounter;

        public static MashButtonMinigameManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        /// <summary>
        /// Start mash button minigame
        /// </summary>
        public void StartMashButtonMinigame(int pressNeeded = 100, float timeLimit=5)
        {
            //reset press counter
            _pressCounter = 0;
            _pressNeeded = pressNeeded;
            _timer = timeLimit;
            _timerText.text = Mathf.CeilToInt(_timer).ToString();

            _progressSlider.maxValue = _pressNeeded;
            _progressSlider.minValue = 0;

            _isPlaying = true;
        }
        void Update()
        {
            if (!_isPlaying)
                return;

            _timer -= Time.deltaTime;
            _timerText.text = Mathf.CeilToInt(_timer).ToString();
            //time limit reached
            if (_timer <= 0)
            {
                //stop minigame
                _isPlaying = false;
                //player lose
                _playerWin = false;
                return;
            }

            if (Input.GetKeyDown(mashKey))
            {
                _progressSlider.value = _pressCounter;
                _pressCounter++;
            }
            CheckResult();
        }
        /// <summary>
        /// Check result if target reached end minigame and set player win
        /// </summary>
        void CheckResult()
        {
            //stop minigame
            _isPlaying = false;

            //player win
            if (_pressCounter >= _pressNeeded)
                _playerWin = true;
            //player lose
            else
                _playerWin = false;
        }
    }
}
