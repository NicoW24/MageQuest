using UI.Game;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    public class LootChestObject : MonoBehaviour
    {
        Animator _animator;
        public bool hasLoot = true;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetupChest(Vector2 pos)
        {
            CloseChest();
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            transform.position = new Vector2(pos.x,transform.position.y);
            hasLoot = true;
        }

        public void OpenChest()
        {
            if (!hasLoot)
            {
                return;
            }
            PanelManager.Instance.ClosePanel("InteractNotif");
            _animator.SetBool("Open", true);
            CharacterStat playerStat = BattleManager.Instance.GetPlayerStat();
            
            //get random unowned skill from chest
            List<CharacterSkillSO> listUnownedSkill = BattleManager.Instance.listAllCharacterSkill.Except(playerStat.listOwnedSkill).ToList();
            CharacterSkillSO rewardSkill = listUnownedSkill[Random.Range(0, listUnownedSkill.Count)];
            playerStat.AddSkill(rewardSkill);

            //setup skill detail panel
            SkillDetailPanelManager.Instance.SetData(rewardSkill,this);
            PanelManager.Instance.OpenPanel("ObtainSkillDetail");

            hasLoot = false;
        }

        public void HideChest()
        {
            //hide chest
            gameObject.SetActive(false);
        }

        public void CloseChest()
        {
            _animator.SetBool("Open", false);
        }
    }
}

