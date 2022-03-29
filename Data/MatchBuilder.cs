using System;
using MongoDB.Bson;
using MongoQuery.Query;
using MongoRequest.Data.State;
using System.Collections.Generic;
using MongoRequest.Data.Strategy.State;


namespace MongoRequest
{
    public class MatchBuilder
    {
        private readonly QueryBase queryBase = new();
                
        public BsonDocument GetMatchDevice(ModelParams modelParams, In query)
        {
            BsonDocument match = SetModelDevice(modelParams, query);
            return match;
        }

        public BsonDocument SetModelDevice(ModelParams modelParams, In query)
        {
            var model = query.Params.Device;
            var tags = query.Params.Tags;

            BsonDocument match = new();
            
            
            if (model == null && tags == null) 
            {
                Match matchDefault = new(new Default());
                return matchDefault.GetMatch(modelParams, query);
            }

            if (model.Search == "" && model.MeterType != "" && tags == null)
            {
                Match matchModel = new(new Model());
                return matchModel.GetMatch(modelParams, query);
            }

            if (model.Search != "" && model.MeterType != "" && tags == null)
            {
                Match match1 = new(new ModelSearch());
                return match1.GetMatch(modelParams, query);
            }

            if (model.Search == "" && model.MeterType != "" && tags != null)
            {
                Match tagsModel = new(new TagsModel());
                return tagsModel.GetMatch(modelParams, query);
            }

            if(model.Search != "" && model.MeterType != "" && tags != null)
            {
                Match isNotEmpty = new(new TagsModelSearch());
                return isNotEmpty.GetMatch(modelParams, query);
            }

            if (model.Search == "" && model.MeterType == "" && tags != null)
            {
                Match tag = new(new Tags());
                return tag.GetMatch(modelParams, query);
            }

            return match;
        }
        
        public BsonDocument GetMatchRangeAndType(ModelParams modelParams, In query)
        {
            Int32 defaultValueStart = 300;
            Int32 defaultValueEnd = int.MaxValue;
            string fields = query.Params.Range?.Fields;
            Int32 start = query.Params.Range?.Start ?? defaultValueStart;
            Int32 end = query.Params.Range?.End ?? defaultValueEnd;
            BsonDocument matchRange = new();

            if (query.Params.Tags == null)
            {
                BsonElement isDemo = new("isDemo", modelParams.IsDemo);
                BsonElement isEmpty = new("isEmpty", modelParams.IsEmpty);

                BsonDocument regex = queryBase.AddOperator("$regex", modelParams.DeviceID);
                BsonDocument regex2 = queryBase.AddOperator("$regex", modelParams.SearchID);

                BsonDocument id = new("_id", regex);
                BsonDocument seatchField = new("_id", regex2);

                List<BsonValue> list = new();
                list.Add(id);
                list.Add(seatchField);

                BsonArray array = queryBase.Array(list);
                BsonElement and = new("$and", array);

                BsonDocument startGte = queryBase.AddOperator("$gte", start);
                BsonDocument endLte = queryBase.AddOperator("$lte", end);
                BsonDocument field = new();
                field.AddRange(startGte);
                field.AddRange(endLte);

                BsonDocument value = new();
                value.Add(fields, field);

                BsonDocument matchList = new();
                matchList.Add(and);
                matchList.AddRange(value);
                matchList.Add(isEmpty);
                matchList.Add(isDemo);

                matchRange = queryBase.AddOperator("$match", matchList);
                return matchRange;
            }
            else if (query.Params.Tags != null)
            {
                BsonDocument element = queryBase.AddOperator("$in", modelParams.Tags);
                BsonElement isDemo = new("isDemo", modelParams.IsDemo);
                BsonElement isEmpty = new("isEmpty", modelParams.IsEmpty);

                BsonDocument regex = queryBase.AddOperator("$regex", modelParams.DeviceID);
                BsonDocument regex2 = queryBase.AddOperator("$regex", modelParams.SearchID);

                BsonDocument id = new("_id", regex);
                BsonDocument tag = new("tags", element);
                BsonDocument seatchField = new("_id", regex2);

                List<BsonValue> list = new();
                list.Add(id);
                list.Add(seatchField);
                list.Add(tag);

                BsonArray array = queryBase.Array(list);
                BsonElement and = new("$and", array);

                BsonDocument startGte = queryBase.AddOperator("$gte", start);
                BsonDocument endLte = queryBase.AddOperator("$lte", end);
                BsonDocument field = new();
                field.AddRange(startGte);
                field.AddRange(endLte);

                BsonDocument value = new();
                value.Add(fields, field);

                BsonDocument matchList = new();
                matchList.Add(and);
                matchList.AddRange(value);
                matchList.Add(isEmpty);
                matchList.Add(isDemo);

                matchRange = queryBase.AddOperator("$match", matchList);
                return matchRange;
            }
            return matchRange;
        }

        public BsonDocument GetMatchUser(ModelParams modelParams)
        {
            BsonDocument element = queryBase.AddOperator("$in", modelParams.UserID);
            BsonDocument deviceID = queryBase.AddOperator("$regex", modelParams.DeviceID);
            BsonDocument searchID = queryBase.AddOperator("$regex", modelParams.SearchID);

            BsonDocument userEq = new("user", element);
            BsonDocument deviceRegex = new("device", deviceID);
            BsonDocument searchRegex = new("device", searchID);

            List<BsonValue> user = new();
            user.Add(userEq);
            user.Add(deviceRegex);
            user.Add(searchRegex);

            BsonArray and = queryBase.Array(user);
            BsonElement And = new("$and", and);

            BsonDocument parametrs = queryBase.AddOperator("$match", And);

            return parametrs;
        }

        public BsonDocument GetMatchUserRange(In query)
        {
            Int32 defaultValueStart = 300;
            Int32 defaultValueEnd = int.MaxValue;
            string fields = query.Params.Range?.Fields;
            Int32 start = query.Params.Range?.Start ?? defaultValueStart;
            Int32 end = query.Params.Range?.End ?? defaultValueEnd;

            BsonDocument startGte = queryBase.AddOperator("$gte", start);
            BsonDocument endLte = queryBase.AddOperator("$lte", end);
            BsonDocument field = new();
            field.AddRange(startGte);
            field.AddRange(endLte);
            BsonDocument value = new();
            value.Add(fields, field);

            BsonArray and = queryBase.Array(value);
            BsonElement And = new("$and", and);

            BsonDocument parametrs = queryBase.AddOperator("$match", And);
            return parametrs;
        }
    }
}
