namespace MongoRequest.Data.DeviceModel
{
    public class Device
    {
        public string MeterType { get; set; } = "^";
        public string Search { get; set; } = "^";
        public bool IsDemo { get; set; } = false;
        public bool IsEmpty { get; set; } = false;
    }
}
