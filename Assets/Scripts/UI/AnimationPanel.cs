using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Game
{
    public class AnimationPanel : Panel
    {
        [SerializeField] Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Open panel and play animation, panel will close automatically
        /// </summary>
        public override void OpenPanel()
        {
            base.OpenPanel();
            //play animation
            _animator.Play(0, 0, 0f);
        }

        /// <summary>
        /// Close panel, after animation
        /// </summary>
        public override void ClosePanel()
        {
            base.ClosePanel();
            //add event invoke after animation end
            PanelManager.Instance.OnPanelAnimationComplete?.Invoke();
        }
    }
}
