using UnityEngine;
using UnityEngine.Playables;


namespace Core.Game
{
    public class TimelineManager : MonoBehaviour
    {
        PlayableDirector _director;
        [SerializeField] PlayableAsset timelineIntroAsset;

        public static TimelineManager Instance;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _director = GetComponent<PlayableDirector>();
        }

        private void Start()
        {
            _director.stopped += OnTimelineStopped;
        }
        void OnDisable()
        {
            _director.stopped -= OnTimelineStopped;
        }

        void OnTimelineStopped(PlayableDirector director) 
        {
            GameManager.Instance.ResumeAllCharacter();
        }

        public void PlayIntroTimeline()
        {
            PlayTimeline(timelineIntroAsset);
        }

        public void PlayTimeline(PlayableAsset timeline)
        {
            GameManager.Instance.PauseAllCharacter();
            _director.Play(timeline);
        }
    }
}

