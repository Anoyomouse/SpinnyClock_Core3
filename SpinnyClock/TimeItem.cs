namespace SpinnyClock
{
    internal class TimeItem
    {
        private string sName;

        public string Name
        {
            get
            {
                return sName;
            }
            set
            {
                sName = value;
            }
        }

        private float dOffset;

        public float Offset
        {
            get
            {
                return dOffset;
            }
            set
            {
                dOffset = value;
            }
        }

        private bool bDST;

        public bool DST
        {
            get
            {
                return bDST;
            }
            set
            {
                bDST = value;
            }
        }

        public TimeItem(string name, float offset, bool dst)
        {
            sName = name;
            dOffset = offset;
            bDST = dst;
        }
    }
}
