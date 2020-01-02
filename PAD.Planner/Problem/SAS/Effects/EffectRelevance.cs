
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Enum representing a relevance of a single SAS+ effect.
    /// </summary>
    public enum EffectRelevance
    {
        /// <summary>
        /// Relevant effect, i.e. positively contributing to the specified conditions or relative state.
        /// </summary>
        RELEVANT,

        /// <summary>
        /// Irrelevant effect, i.e. not affecting the specified conditions or relative state at all.
        /// </summary>
        IRRELEVANT,

        /// <summary>
        /// "Anti-relevant" effect, i.e. negatively contributing to the specified conditions or relative state, or conflicted.
        /// </summary>
        ANTI_RELEVANT
    }
}
