using TMPro;
using UnityEngine;

namespace Core.Game
{
    public class SkillActionButton : MonoBehaviour
    {
        CharacterSkillSO _skill;
        [SerializeField] TextMeshProUGUI _skillNameText;

        public void SetupSkillActionButton(CharacterSkillSO skill)
        {
            _skillNameText.text = $"{skill.skillName}\nDamage{skill.damage}\nCost{skill.manaCost}";
            _skill = skill;
        }

        public CharacterSkillSO GetSkill()
        {
            return _skill;
        }

        public void OnClickSkill()
        {
            BattleManager.Instance.OnPlayerSkillAction(_skill);
        }
    }
}
