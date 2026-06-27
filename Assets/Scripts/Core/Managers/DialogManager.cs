using UnityEngine;
using Fungus;

namespace Core.Game
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField] Flowchart _dialogFlowchart;
        bool _inDialog;

        public static DialogManager Instance;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        public void StartDialog(string dialogBlock)
        {
            if (_dialogFlowchart == null)
                return;

            if (_dialogFlowchart.HasBlock(dialogBlock))
            {
                _dialogFlowchart.ExecuteBlock(dialogBlock);
            }
            else
            {
                Debug.Log($"There is no {dialogBlock} in flowchart");
            }
        }

        void OnEnable()
        {
            WriterSignals.OnWriterState += HandleWriterState;
        }

        void OnDisable()
        {
            WriterSignals.OnWriterState -= HandleWriterState;
        }

        public bool IsInDialog()
        {
            return _inDialog;
        }

        private void HandleWriterState(Writer writer, WriterState writerState)
        {
            switch (writerState)
            {
                case WriterState.Start:
                    Debug.Log("Dialogue has opened/started!");
                    BattleManager.Instance.GetPlayerStat().GetController().PauseCharacter();
                    _inDialog = true;
                    break;

                case WriterState.End:
                    Debug.Log("Dialogue is completely finished!");
                    if(!BattleManager.Instance.GetPlayerStat().GetController().IsInBattle())
                        BattleManager.Instance.GetPlayerStat().GetController().ResumeCharacter();
                    _inDialog = false;
                    break;
            }
        }
    }
}

