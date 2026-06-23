using TMPro;
using UnityEngine;

namespace Core.Game
{
    public class SkillDetailPanelManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _skillNameText;
        [SerializeField] TextMeshProUGUI _skillDescText;
        [SerializeField] TextMeshProUGUI _skillDamageText;
        [SerializeField] TextMeshProUGUI _skillManaCostText;

        LootChestObject _currentChest;

        public static SkillDetailPanelManager Instance;

        void Awake()
        {
            if(Instance == null) 
            { 
                Instance = this; 
            }
        }

        public void SetData(CharacterSkillSO skill,LootChestObject chest)
        {
            _skillNameText.text = skill.skillName;
            _skillDescText.text = skill.skillDesc;
            _skillDamageText.text = $"Damage:/n{skill.damage}";
            _skillManaCostText.text = $"Mana Cost:/n{skill.manaCost}";

            _currentChest = chest;
        }

        public void CloseChest()
        {
            _currentChest.CloseChest();
        }
    }
}

