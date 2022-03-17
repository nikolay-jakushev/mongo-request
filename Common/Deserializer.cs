using Newtonsoft.Json;




namespace MongoRequest
{
    public class Deserializer
    {
        public object Deserialize(string msg)
        {
           object query = JsonConvert.DeserializeObject(msg);
            return query;
        }
    }
}
