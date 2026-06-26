using System.Collections.Generic;
using UI.Game;
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
            CheckAvailableSkill();
        }

        void Start()
        {
            //setup player owned skill
            CharacterStat playerStat = BattleManager.Instance.GetPlayerStat();
            foreach(CharacterSkillSO playerSkill in playerStat.listOwnedSkill)
            {
                AddSkill(playerSkill);
            }
            CheckAvailableSkill();
        }

        /// <summary>
        /// Disable skill that can't be executed because of mana cost
        /// </summary>
        void CheckAvailableSkill()
        {
            //disable skill that can't execute because mana cost
            foreach(SkillActionButton skillButton in listSpawnedSkill)
            {
                skillButton.GetButton().interactable = BattleManager.Instance.GetPlayerStat().GetCurrentMana() >= skillButton.GetSkill().manaCost;
            }
        }
        /// <summary>
        /// Add new skill to UI
        /// </summary>
        public void AddSkill(CharacterSkillSO skill)
        {
            SkillActionButton newSkillButton = Instantiate(_skillActionButtonPrefab, _skillActionButtonContent);
            newSkillButton.SetupSkillActionButton(skill);
            listSpawnedSkill.Add(newSkillButton);
        }
        /// <summary>
        /// Open skill panel
        /// </summary>
        public void OpenPanel()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// Close skill panel
        /// </summary>
        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }
    }
}

