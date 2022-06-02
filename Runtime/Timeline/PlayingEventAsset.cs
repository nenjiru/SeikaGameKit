using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace SeikaGameKit.Timeline
{
    public class PlayingEventAsset : PlayableAsset, ITimelineClipAsset
    {
        public ExposedReference<PlayingEventItem> target;
        public TimelineClip clip { get; set; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<PlayingEventBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.item = target.Resolve(graph.GetResolver()).GetComponent(typeof(PlayingEventItem)) as PlayingEventItem;
            behaviour.clip = clip;
            return playable;
        }

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }
    }
}
