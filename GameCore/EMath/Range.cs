namespace GameCore.EMath
{
    public struct Range
    {
        public float From { get; set; }
        public float To { get; set; }

        public Range(float from, float to)
        {
            From = from;
            To = to;
        }

        public override string ToString()
        {
            return $"{From} - {To}";
        }
    }
}