using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Game
{
    public class SkillActionButton : MonoBehaviour
    {
        CharacterSkillSO _skill;
        [SerializeField] TextMeshProUGUI _skillNameText;
        Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClickSkill);
        }

        public Button GetButton()
        {
            return _button;
        }

        public void SetupSkillActionButton(CharacterSkillSO skill)
        {
            _skillNameText.text = $"{skill.skillName}\n<Size=8>Dmg {skill.damage}/Cost {skill.manaCost}";
            _skill = skill;
        }

        public CharacterSkillSO GetSkill()
        {
            return _skill;
        }

        public void OnClickSkill()
        {
            BattleManager.Instance.OnPlayerSkillAction(_skill);
            //close skill
            PlayerSkillActionPanelManager.Instance.ClosePanel();
        }
    }
}
