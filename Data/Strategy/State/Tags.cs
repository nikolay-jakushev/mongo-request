using MongoDB.Bson;
using MongoQuery.Query;
using MongoRequest.Data.State;
using System.Collections.Generic;

namespace MongoRequest.Data.Strategy.State
{
    class Tags : IStrategy
    {
        private static QueryBase queryBase = new();
        private BsonDocument match = new();

        public BsonDocument GetMatch(ModelParams model, In msg)
        {
            BsonDocument element = queryBase.AddOperator("$in", model.Tags);
            BsonElement isDemo = new("isDemo", model.IsDemo);
            BsonElement isEmpty = new("isEmpty", model.IsEmpty);
            BsonDocument tag = new("tags", element);

            List<BsonValue> list = new();
            list.Add(tag);

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
