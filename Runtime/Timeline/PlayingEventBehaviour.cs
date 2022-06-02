using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SeikaGameKit.Timeline
{
    public class PlayingEventBehaviour : PlayableBehaviour
    {
        public PlayingEventItem item;
        public TimelineClip clip { get; set; }

        public override void OnGraphStart(Playable playable)
        {
            if (item != null)
            {
                item.clipStart = clip.start;
                item.clipEnd = clip.end;
                item.clipDuration = clip.duration;
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (item != null)
            {
                item.isPlaying = true;
                item.OnPlay();
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (item != null)
            {
                item.currentTime = playable.GetTime();
                item.normalizedTime = item.currentTime / playable.GetDuration();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (item != null)
            {
                item.isPlaying = false;
                item.OnStop();
            }
        }
    }
}