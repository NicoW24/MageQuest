using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Game
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField] List<Panel> _listPanel = new List<Panel>();
        [SerializeField] Transform _panelContainer;
        Dictionary<string,Panel> _listPanelSpawned = new Dictionary<string, Panel>();

        Panel _lastOpenedPanel;
        public UnityAction OnPanelAnimationComplete;

        public static PanelManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            //spawn panel from prefab to canvas
            foreach (Panel panel in _listPanel)
            {
                Panel newPanel = Instantiate(panel, _panelContainer);
                //to prevent event invoke we dont use function close on first spawn
                newPanel.gameObject.SetActive(false);
                string panelName = panel.name.Replace("_Panel", "");
                _listPanelSpawned.Add(panelName, newPanel);
            }
        }
        /// <summary>
        /// Open panel with panel name, the name is taken from the prefab name
        /// </summary>
        public void OpenPanel(string panelName)
        {
            if (_listPanelSpawned.TryGetValue(panelName, out Panel panel))
            {
                panel.OpenPanel();
                _lastOpenedPanel = panel;
            }
            else
            {
                Debug.LogWarning($"Can't Open, Panel '{panelName}' not found.");
            }
        }
        /// <summary>
        /// Close panel with panel name, the name is taken from the prefab name
        /// </summary>
        public void ClosePanel(string panelName)
        {
            if (_listPanelSpawned.TryGetValue(panelName, out Panel panel))
            {
                if(_lastOpenedPanel == panel)
                {
                    _lastOpenedPanel = null;
                }

                //only close opened panel
                if(panel != null)
                {
                    if (panel.gameObject.activeSelf)
                        panel.ClosePanel();
                }
            }
            else
            {
                Debug.LogWarning($"Can't Close, Panel '{panelName}' not found.");
            }
        }
        /// <summary>
        /// Close last opened panel
        /// </summary>
        public void CloseLastPanel()
        {
            if( _lastOpenedPanel != null && _lastOpenedPanel.gameObject.activeSelf)
            {
                _lastOpenedPanel.ClosePanel();
            }
        }

        public Panel GetPanel(string panelName)
        {
            return _listPanelSpawned[panelName];
        }
    }
}
