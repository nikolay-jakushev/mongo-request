using MongoDB.Bson;
using MongoQuery.Query;
using MongoRequest.Data.State;
using System.Collections.Generic;

namespace MongoRequest.Data.Strategy.State
{
    class Model : IStrategy
    {
        private static QueryBase queryBase = new();
        private BsonDocument match = new();
        public BsonDocument GetMatch(ModelParams model, In msg)
        {            
            BsonElement isDemo = new("isDemo", model.IsDemo);
            BsonElement isEmpty = new("isEmpty", model.IsEmpty);

            BsonDocument regex = queryBase.AddOperator("$regex", model.DeviceID);
            BsonDocument id = new("_id", regex);
            List<BsonValue> list = new();
            list.Add(id);

            BsonArray array = queryBase.Array(list);
            BsonElement and = new("$and", array);
            
            BsonDocument matchList = new();
            matchList.Add(and);
            matchList.Add(isEmpty);
            matchList.Add(isDemo);

            return match = queryBase.AddOperator("$match", matchList);            
        }
    }
}
