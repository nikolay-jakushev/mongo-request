using System.Collections.Generic;


namespace MongoRequest.Data.DeviceModel
{
    public class Params
    {
        public Device Device { get; set; }
        public List<string> User { get; set; }
        public string Sort { get; set; } = "userTime";
        public int Order { get; set; } = -1;
        public Paging Paging { get; set; }
        public Range Range { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Project { get; set; }
    }
}
