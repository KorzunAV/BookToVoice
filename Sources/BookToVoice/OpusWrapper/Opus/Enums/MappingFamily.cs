namespace OpusWrapper.Opus.Enums
{

    /// <summary>
    /// The channel mapping family, in the range 0...255.
    /// </summary>
    public enum MappingFamily : int
    {
        /// <summary>
        /// Channel mapping family 0 covers mono or stereo in a single stream.
        /// </summary>
        SingleStream = 0,

        /// <summary>
        /// Channel mapping family 1 covers 1 to 8 channels in one or more streams, using the Vorbis speaker assignments.
        /// </summary>
        Vorbis = 1,

        /// <summary>
        /// Channel mapping family 255 covers 1 to 255 channels in one or more streams, but without any defined speaker assignment.
        /// </summary>
        Multy = 255
    }
}
