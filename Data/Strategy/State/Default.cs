using MongoDB.Bson;
using MongoQuery.Query;
using MongoRequest.Data.State;
using System.Collections.Generic;

namespace MongoRequest.Data.Strategy.State
{
    class Default : IStrategy
    {
        private static QueryBase queryBase = new();
        private static BsonDocument match = new();
        public BsonDocument GetMatch(ModelParams model, In msg)
        {
            BsonElement isDemo = new("isDemo", model.IsDemo);
            BsonElement isEmpty = new("isEmpty", model.IsEmpty);
            List<BsonElement> element = new();
            element.Add(isDemo);
            element.Add(isEmpty);
            return match = queryBase.AddOperator("$match", element);
        }
    }
}
