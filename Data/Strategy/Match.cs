using MongoDB.Bson;


namespace MongoRequest.Data.State
{
    class Match
    {
        private readonly IStrategy _state;
        public Match(IStrategy state)
        {
            _state = state;
        }

        public BsonDocument GetMatch(ModelParams model, In msg)
        {
            return _state.GetMatch(model, msg);
        }
    }
}
