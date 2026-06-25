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

        /// <summary>
        /// Move and activate chest to dead enemy pos
        /// </summary>
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
        /// <summary>
        /// Open chest animation and randomize unlearned skill
        /// </summary>
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
            List<CharacterSkillSO> listUnownedSkill = BattleManager.Instance.ListAllCharacterSkill.Except(playerStat.listOwnedSkill).ToList();
            if(listUnownedSkill.Count > 0)
            {
                CharacterSkillSO rewardSkill = listUnownedSkill[Random.Range(0, listUnownedSkill.Count)];
                playerStat.AddSkill(rewardSkill);

                //setup skill detail panel
                SkillDetailPanelManager.Instance.SetData(rewardSkill, this);
                PanelManager.Instance.OpenPanel("ObtainSkillDetail");
            }
            hasLoot = false;
        }
        /// <summary>
        /// Hide chest after loot taken
        /// </summary>
        public void HideChest()
        {
            //hide chest
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Close chest animation
        /// </summary>
        public void CloseChest()
        {
            _animator.SetBool("Open", false);
        }
    }
}

