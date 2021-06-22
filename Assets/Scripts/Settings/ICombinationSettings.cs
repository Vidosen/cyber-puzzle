namespace Data
{
    public interface ICombinationSettings
    {
        /// <summary>
        /// Hack Points added to the combination per one cell
        /// </summary>
        double HPRewardForCell { get; }
        /// <summary>
        /// Hack Points added to the combination for a disjoint pair of cells
        /// </summary>
        double HPRewardForDisjointPair { get;}
        /// <summary>
        /// Hack Points added to the combination for a not neighboring pair of cells
        /// </summary>
        double HPRewardForNotNeighboringPair { get; }
    }
}