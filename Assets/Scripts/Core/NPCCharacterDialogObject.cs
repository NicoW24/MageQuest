using UI.Game;
using UnityEngine;

namespace Core.Game
{
    public class NPCCharacterDialogObject : MonoBehaviour
    {
        [Header("Dialog")]
        [SerializeField] string dialogBlockWithReward;
        [SerializeField] CharacterSkillSO skillRewardFromDialog;
        bool _rewardTaken = false;
        [SerializeField] string dialogBlockRewardTaken;

        public void StartDialog()
        {
            if (!_rewardTaken)
            {
                DialogManager.Instance.StartDialog(dialogBlockWithReward);
            }
            else
            {
                DialogManager.Instance.StartDialog(dialogBlockRewardTaken);
            }
        }

        public void GivePlayerSkill()
        {
            BattleManager.Instance.GetPlayerStat().AddSkill(skillRewardFromDialog);

            //setup skill detail panel
            SkillDetailPanelManager.Instance.SetData(skillRewardFromDialog);
            PanelManager.Instance.OpenPanel("ObtainSkillDetail");
            _rewardTaken = true;
        }
    }
}
