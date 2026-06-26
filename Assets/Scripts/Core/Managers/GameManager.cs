using System.Collections;
using UI.Game;
using UnityEngine;

namespace Core.Game
{
    public class GameManager : MonoBehaviour
    {
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

