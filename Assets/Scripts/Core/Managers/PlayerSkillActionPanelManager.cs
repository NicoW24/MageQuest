using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class PlayerSkillActionPanelManager : MonoBehaviour
    {
        [SerializeField] SkillActionButton _skillActionButtonPrefab;
        [SerializeField] Transform _skillActionButtonContent;
        List<SkillActionButton> listSpawnedSkill = new List<SkillActionButton>();

        public static PlayerSkillActionPanelManager Instance;

        void Awake()
        {
            if (Instance == null) 
            {
                Instance = this;
            }
        }

        void OnEnable()
        {
            
        }

        void Start()
        {
            //setup player owned skill
            CharacterStat playerStat = BattleManager.Instance.GetPlayerStat();
            foreach(CharacterSkillSO playerSkill in playerStat.listOwnedSkill)
            {
                AddSkill(playerSkill);
            }
        }

        void DisableSkillButton()
        {
            //DISABLE SKILL THAT PLAYER CANT EXECUTE BECAUSE OF MANA
        }

        public void AddSkill(CharacterSkillSO skill)
        {
            SkillActionButton newSkillButton = Instantiate(_skillActionButtonPrefab, _skillActionButtonContent);
            newSkillButton.SetupSkillActionButton(skill);
            listSpawnedSkill.Add(newSkillButton);
        }
    }
}

