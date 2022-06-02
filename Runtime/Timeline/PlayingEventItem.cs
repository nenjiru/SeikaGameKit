using UnityEngine;

namespace SeikaGameKit.Timeline
{
    public class PlayingEventItem : MonoBehaviour
    {
        public bool isPlaying { get; set; }
        public double clipStart { get; set; }
        public double clipEnd { get; set; }
        public double clipDuration { get; set; }
        public double currentTime { get; set; }
        public double normalizedTime { get; set; }

        public virtual void OnPlay()
        {
        }

        public virtual void OnStop()
        {
        }
    }
}
