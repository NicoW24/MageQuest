using System.Collections;
using UI.Game;
using UnityEngine;
using Fungus;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Core.Game
{
    public class GameManager : MonoBehaviour
    {

        public UnityAction OnPauseAllCharacter;
        public UnityAction OnResumeAllCharacter;

        public static GameManager Instance;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        public void PauseAllCharacter()
        {
            OnPauseAllCharacter?.Invoke();
        }
        public void ResumeAllCharacter()
        {
            //before resume check dialog and battle
            if (DialogManager.Instance.IsInDialog() || BattleManager.Instance.GetPlayerStat().GetController().IsInBattle())
                return;
            OnResumeAllCharacter?.Invoke();
        }

        void Start()
        {
            StartCoroutine(InitGame());
        }

        IEnumerator InitGame()
        {
            yield return null;
            OpenPanelCustomizePlayerChar();
        }

        void OpenPanelCustomizePlayerChar()
        {
            PanelManager.Instance.OpenPanel("CustomizePlayerCharacter");
        }
    }
}

