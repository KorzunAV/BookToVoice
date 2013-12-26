namespace OpusWrapper.Opus.Presets
{
    public abstract class BaseOption<T>
    {
        public abstract string OptionName { get; }
        public abstract string InfoValue { get; }
        public abstract T Value { get; }
    }
}