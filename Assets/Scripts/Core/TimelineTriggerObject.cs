using UnityEngine;
using UnityEngine.Playables;

namespace Core.Game
{
    public class TimelineTriggerObject : MonoBehaviour
    {
        [SerializeField] PlayableAsset _bossIntroTimeline;
        bool _played = false;

        /// <summary>
        /// Collider detection for player
        /// </summary>
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_played)
            {
                TimelineManager.Instance.PlayTimeline(_bossIntroTimeline);
                _played = true;
            }
        }
    }
}
