public interface IPauseable
{
    public bool IsPaused { get; set; }
    public void OnPauseGame(bool shouldPause);
}
