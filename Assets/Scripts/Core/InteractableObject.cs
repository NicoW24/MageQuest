using UI.Game;
using UnityEngine;

namespace Core.Game
{
    public enum InteractObjectType
    {
        Chest,
        Npc
    }

    public class InteractableObject : MonoBehaviour
    {
        public InteractObjectType interactObjectType;
        PlayerCharacterController _playerController;

        LootChestObject _lootChestObject;
        NPCCharacterDialogObject _npcDialogObject;

        void Awake()
        {
            _npcDialogObject = GetComponent<NPCCharacterDialogObject>();
            _lootChestObject = GetComponent<LootChestObject>();
        }
        /// <summary>
        /// Set object to player and show interaction UI
        /// </summary>
        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.transform.tag == "Player")
            { 
                //only add once
                if(_playerController == null)
                    _playerController = BattleManager.Instance.GetPlayerObject().GetComponent<PlayerCharacterController>();

                //check if chest
                if (_lootChestObject != null)
                {
                    //if loot already taken dont let player interact
                    if (!_lootChestObject.hasLoot)
                    {
                        return;
                    }
                }
                //set interactable object to player
                _playerController.SetInteractableObject(this);
                //open interactable gui
                PanelManager.Instance.OpenPanel("InteractNotif");
            }
        }
        /// <summary>
        /// Function when player exit collider
        /// </summary>
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.tag == "Player")
            {
                //if chest close the chest
                if(_lootChestObject != null)
                {
                    //close the chest
                    _lootChestObject.CloseChest();
                    //if loot has been taken hide chest after leaving trigger
                    if (!_lootChestObject.hasLoot)
                        _lootChestObject.HideChest();
                }

                //close skill panel
                PanelManager.Instance.ClosePanel("ObtainSkillDetail");

                //remove interactable object from player
                _playerController.SetInteractableObject(null);
                PanelManager.Instance.ClosePanel("InteractNotif");
            }
        }
        /// <summary>
        /// Get chest object
        /// </summary>
        public LootChestObject GetChestObject()
        {
            return _lootChestObject;
        }
        /// <summary>
        /// Get dialog object
        /// </summary>
        public NPCCharacterDialogObject GetDialogObject()
        {
            return _npcDialogObject;
        }
    }
}

