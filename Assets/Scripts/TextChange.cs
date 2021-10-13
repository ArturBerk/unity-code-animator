namespace CodeAnimator
{
    public struct TextChange
    {
        public ChangeType ChangeType;
        public int StartCurrent;
        public int StartNew;
        public int Length;

        public TextChange(ChangeType changeType, int startCurrent, int startNew, int length)
        {
            ChangeType = changeType;
            StartCurrent = startCurrent;
            StartNew = startNew;
            Length = length;
        }
    }

    public enum ChangeType
    {
        Change,
        Delete,
        Insert
    }
}