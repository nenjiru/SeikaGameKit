using UnityEngine;
using UnityEngine.Playables;

namespace SeikaGameKit.Timeline
{
    public class PlayingEventAsset : PlayableAsset
    {
        public ExposedReference<GameObject> item;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<PlayingEventBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.item = item.Resolve(graph.GetResolver()).GetComponent(typeof(IPlayingEventItem)) as IPlayingEventItem;
            return playable;
        }
    }
}
