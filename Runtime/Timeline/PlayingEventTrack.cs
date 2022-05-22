using UnityEngine.Timeline;

namespace SeikaGameKit.Timeline
{
    [System.Serializable]
    [TrackClipType(typeof(PlayingEventAsset), false)]
    public class PlayingEventTrack : PlayableTrack
    {
    }
}