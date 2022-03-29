using MongoDB.Bson;


namespace MongoRequest.Question.Pipeline
{
    public class PipelineList
    {
        public BsonDocument Match { get; set; }
        public BsonDocument Project { get; set; }
        public BsonElement SortItem { get; set; }
        public BsonDocument Sort { get; set; }
        public BsonDocument Skip { get; set; }
        public BsonDocument Limit { get; set; }
        public BsonDocument CountQuery { get; set; }
    }
}
