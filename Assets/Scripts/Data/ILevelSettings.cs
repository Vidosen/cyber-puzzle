namespace Data
{
    public interface ILevelSettings
    {
        /// <summary>
        /// Hack Points required to win the level
        /// </summary>
        double HPRequired { get; }
        /// <summary>
        /// Duration of level timer (in seconds)
        /// </summary>
        double LevelTimerDuration { get; }
    }
}