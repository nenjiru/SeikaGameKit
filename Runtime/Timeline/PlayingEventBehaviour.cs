using UnityEngine.Playables;

namespace SeikaGameKit.Timeline
{
    public class PlayingEventBehaviour : PlayableBehaviour
    {
        public IPlayingEventItem item;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (item != null)
            {
                item.isPlaying = true;
                item.OnPlay();
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