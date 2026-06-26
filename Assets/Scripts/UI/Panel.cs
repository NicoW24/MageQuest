using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Game
{
    public class Panel : MonoBehaviour
    {
        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        public virtual void OpenPanel()
        {
            gameObject.SetActive(true);
            OnOpen?.Invoke();
        }

        public virtual void ClosePanel()
        {
            gameObject.SetActive(false);
            OnClose?.Invoke();
        }
    }
}

