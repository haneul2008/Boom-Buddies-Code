namespace Code.EventSystems
{
    public static class CameraEvents
    {
        public static readonly CameraModeChangeEvent CameraModeChangeEvent = new CameraModeChangeEvent();
    }

    public class CameraModeChangeEvent : GameEvent
    {
        public bool isCamMode;

        public CameraModeChangeEvent Initializer(bool isCamMode)
        {
            this.isCamMode = isCamMode;
            return this;
        }
    }
}