using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SeikaGameKit.Timeline
{
    [System.Serializable]
    [TrackClipType(typeof(PlayingEventAsset), false)]
    public class PlayingEventTrack : PlayableTrack
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (TimelineClip clip in m_Clips)
            {
                var playableAsset = clip.asset as PlayingEventAsset;
                playableAsset.clip = clip;
            }
            return base.CreateTrackMixer(graph, go, inputCount);
        }
    }
}