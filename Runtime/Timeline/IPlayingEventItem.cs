namespace SeikaGameKit.Timeline
{
    public interface IPlayingEventItem
    {
        public bool isPlaying { get; set; }

        public void OnPlay();

        public void OnStop();
    }
}
