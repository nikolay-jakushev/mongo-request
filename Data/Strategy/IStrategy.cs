using MongoDB.Bson;


namespace MongoRequest.Data.State
{
    interface IStrategy
    {
        public BsonDocument GetMatch(ModelParams model, In msg);
    }
}
