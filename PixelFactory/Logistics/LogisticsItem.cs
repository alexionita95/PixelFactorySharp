using PixelFactory.Items;

namespace PixelFactory.Logistics
{
    public class LogisticsItem
    {
        public Item Item { get; set; } = null;
        public double Progress { get; set; } = 0;
        public bool Ready { get => Progress.Equals(1); }

        public ItemLogisticsComponentPort.PortDirection SourceDirection { get; set; }
        public uint SourcePosition {  get; set; }
        public ItemLogisticsComponentPort.PortDirection DestinationDirection { get; set; }
        public uint DestinationPosition { get; set; }
        public void Update(double step)
        {
            Progress += step;
            if (Progress > 1)
            {
                Progress = 1;
            }
        }
        public LogisticsItem()
        {
        }
        public LogisticsItem(Item item)
        {
            Item = item;
            Progress = 0;
        }

    }
}
