using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Core.Game
{
    public class SequenceMinigameManager : MinigameManager
    {
        [SerializeField] TextMeshProUGUI _timerText;
        [SerializeField] Transform _sequenceKeyContainer;
        [SerializeField] SequenceMinigameKeyObject _sequenceKeyPrefab;
        List<SequenceMinigameKeyObject> _spawnedSequenceKeyObject = new List<SequenceMinigameKeyObject>();
        [SerializeField] List<Sprite> _listKeySequenceSprite = new List<Sprite>();

        [Header("Minigame Variable Mechanic")]
        int _sequenceLength = 4;
        float _timeLimit = 5f;

        List<KeyCode> _listSequence = new List<KeyCode>();
        int _currentIndex;

        public static SequenceMinigameManager Instance;

        void Awake()
        {
            if (Instance == null) 
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Start sequence minigame
        /// </summary>
        public void StartSequenceMinigame(int sequenceLength = 4, float timeLimit = 5)
        {
            //set length and time limit
            _sequenceLength = sequenceLength;
            _timeLimit = timeLimit;
            //setup key placeholder
            SetupSequenceKeyObj();
            _listSequence.Clear();

            KeyCode[] possibleKeys =
            {
                KeyCode.UpArrow,
                KeyCode.DownArrow,
                KeyCode.LeftArrow,
                KeyCode.RightArrow
            };
            //set sequence
            KeyCode firstKey = possibleKeys[Random.Range(0, possibleKeys.Length)];
            _listSequence.Add(firstKey);
            _spawnedSequenceKeyObject[0].SetData(GetKeySprite(firstKey));
            KeyCode lastKey = firstKey;
            for (int i = 1; i < sequenceLength; i++)
            {
                lastKey = GetNextSequence(lastKey);
                _spawnedSequenceKeyObject[i].SetData(GetKeySprite(lastKey));
                _listSequence.Add(lastKey);
            }
            //update timer UI
            _timerText.text = Mathf.CeilToInt(_timeLimit).ToString();

            _currentIndex = 0;
            _timer = timeLimit;
            _isPlaying = true;
            _playerWin = false;
        }
        void Update()
        {
            if (!_isPlaying)
                return;

            _timer -= Time.deltaTime;
            _timerText.text = Mathf.CeilToInt(_timer).ToString();
            //time limit reached game end
            if (_timer <= 0)
            {
                _isPlaying = false;
                _playerWin = false;
                return;
            }

            CheckInput();
        }
        /// <summary>
        /// Spawn sequence key object, key used for UI minigame
        /// </summary>
        void SetupSequenceKeyObj()
        {
            //create key placeholder
            if (_spawnedSequenceKeyObject.Count == 0)
            {
                for (int i = 0; i < _sequenceLength; i++)
                {
                    SequenceMinigameKeyObject keyObj = Instantiate(_sequenceKeyPrefab, _sequenceKeyContainer);
                    _spawnedSequenceKeyObject.Add(keyObj);
                }
            }
            else
            {
                //setup key needed
                if(_spawnedSequenceKeyObject.Count < _sequenceLength)
                {
                    int keyNeeded = _sequenceLength - _spawnedSequenceKeyObject.Count;
                    for (int i = 0; i < keyNeeded; i++)
                    {
                        //create key placeholder
                        SequenceMinigameKeyObject keyObj = Instantiate(_sequenceKeyPrefab, _sequenceKeyContainer);
                        _spawnedSequenceKeyObject.Add(keyObj);
                    }
                }
                else if(_spawnedSequenceKeyObject.Count > _sequenceLength)
                {
                    //disable all key object
                    foreach(SequenceMinigameKeyObject keyObject in _spawnedSequenceKeyObject)
                    {
                        keyObject.gameObject.SetActive(false);
                    }
                    //activate key object according to sequence count
                    for (int i = 0; i < _sequenceLength; i++)
                    {
                        _spawnedSequenceKeyObject[i].gameObject.SetActive(true);
                    }
                }
            }
        }
        /// <summary>
        /// Get key sprite for key object
        /// </summary>
        Sprite GetKeySprite(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.UpArrow:
                    return _listKeySequenceSprite[0];
                case KeyCode.DownArrow:
                    return _listKeySequenceSprite[1];
                case KeyCode.LeftArrow:
                    return _listKeySequenceSprite[2];
                case KeyCode.RightArrow:
                    return _listKeySequenceSprite[3];
            }
            return null;
        }
        /// <summary>
        /// Get next sequence function with weighted random
        /// </summary>
        KeyCode GetNextSequence(KeyCode previous)
        {
            List<KeyCode> candidates = new()
            {
                KeyCode.UpArrow,
                KeyCode.DownArrow,
                KeyCode.LeftArrow,
                KeyCode.RightArrow
            };

            float[] weights = { 1f, 1f, 1f, 1f };

            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i] == previous)
                    weights[i] = 0.2f;
            }

            float total = weights.Sum();
            float roll = Random.value * total;

            for (int i = 0; i < candidates.Count; i++)
            {
                roll -= weights[i];

                if (roll <= 0)
                    return candidates[i];
            }

            return candidates[0];
        }
        /// <summary>
        /// Sequence minigame check input
        /// </summary>
        void CheckInput()
        {
            if (!Input.anyKeyDown)
                return;

            KeyCode expectedKey = _listSequence[_currentIndex];
            if (Input.GetKeyDown(expectedKey))
            {
                //correct input
                _spawnedSequenceKeyObject[_currentIndex].KeyCorrect();
                _currentIndex++;
                //check sequence input finished
                if (_currentIndex >= _listSequence.Count)
                {
                    //stop minigame
                    _isPlaying = false;
                    //player win
                    _playerWin = true;
                }
            }
            else
            {
                //stop minigame
                _isPlaying = false;
                //player lose
                _playerWin = false;
            }
        }
    }
}

