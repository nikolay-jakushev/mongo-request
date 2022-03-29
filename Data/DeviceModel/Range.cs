namespace MongoRequest.Data.DeviceModel
{
    public class Range
    {
        public string Fields { get; set; } = "serialNum";

        public int Start { get; set; } = int.MinValue;

        public int End { get; set; } = int.MaxValue;
    }
}
