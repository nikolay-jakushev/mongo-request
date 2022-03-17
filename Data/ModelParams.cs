using MongoDB.Bson;
using System.Collections.Generic;


namespace MongoRequest
{
    public class ModelParams
    {
        public string DeviceID { get; set; } = "^";
        public string SearchID { get; set; } = "";
        public List<string> UserID { get; set; }

        public string Sort { get; set; } = "userTime";
        public int Order { get; set; } = -1;

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Skip { get; set; }

        public bool IsDemo { get; set; } = false;
        public bool IsEmpty { get; set; } = false;

        public List<string> Tags { get; set; }

        public List<BsonElement>  Project { get; set; }
    }
}
