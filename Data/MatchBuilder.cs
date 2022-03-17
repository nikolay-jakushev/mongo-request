using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;




namespace MongoRequest
{
    public class MatchBuilder
    {
        MongoQuery.Query.QueryBase queryBase = new();

        public BsonDocument GetMatchDefault(ModelParams modelParams)
        {
            BsonElement isDemo = new("isDemo", modelParams.IsDemo);
            BsonElement isEmpty = new("isEmpty", modelParams.IsEmpty);
            List<BsonElement> element = new();
            element.Add(isDemo);
            element.Add(isEmpty);

            BsonDocument parametrs = queryBase.AddOperator("$match", element);
            return parametrs;
        }
        
        public BsonDocument GetMatch(ModelParams modelParams, string msg)
        {
            dynamic query = JsonConvert.DeserializeObject(msg);
            BsonDocument match = new BsonDocument();

            if (query.PARAMS?.Device.ToString() == "")
            {
                match = SetDevice(modelParams, msg);
                return match;
            }
            else if (query.PARAMS?.Device.ToString() != "")
            {
                match = SetModelDevice(modelParams, msg);
                return match;
            }
            return match;
        }

        public BsonDocument SetModelDevice(ModelParams modelParams, string msg)
        {
            dynamic query = JsonConvert.DeserializeObject(msg);
            string model = query.PARAMS?.Device.MeterType.ToString();
            string search = query.PARAMS?.Device.Search.ToString();
            string tags = query.PARAMS?.Tags.ToString();
            BsonDocument match = new BsonDocument();

            if (search == "" && model != "" && tags == "")
            {
                BsonElement isDemo = new("isDemo", modelParams.IsDemo);
                BsonElement isEmpty = new("isEmpty", modelParams.IsEmpty);

                BsonDocument regex = queryBase.AddOperator("$regex", modelParams.DeviceID);
                BsonDocument id = new("_id", regex);
                List<BsonValue> list = new();
                list.Add(id);

                BsonArray array = queryBase.Array(list);
                BsonElement and = new("$and", array);

                BsonDocument matchList = new();
                matchList.Add(and);
                matchList.Add(isEmpty);
                matchList.Add(isDemo);

                match = queryBase.AddOperator("$match", matchList);
                return match;
            }            
            else if (search != "" && model != "" && tags == "")
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

                BsonDocument matchList = new();
                matchList.Add(and);
                matchList.Add(isEmpty);
                matchList.Add(isDemo);

                match = queryBase.AddOperator("$match", matchList);
                return match;
            }
            else if (search == "" && model != "" && tags != "")
            {
                BsonDocument element = queryBase.AddOperator("$in", modelParams.Tags);
                BsonElement isDemo = new("isDemo", modelParams.IsDemo);
                BsonElement isEmpty = new("isEmpty", modelParams.IsEmpty);

                BsonDocument regex = queryBase.AddOperator("$regex", modelParams.DeviceID);                
                BsonDocument id = new("_id", regex);
                BsonDocument tag = new("tags", element);


                List<BsonValue> list = new();
                list.Add(id);
                list.Add(tag);

                BsonArray array = queryBase.Array(list);
                BsonElement and = new("$and", array);

                BsonDocument matchList = new();
                matchList.Add(and);
                matchList.Add(isEmpty);
                matchList.Add(isDemo);

                match = queryBase.AddOperator("$match", matchList);
                return match;
            }
            else if(search != "" && model != "" && tags != "")
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

                BsonDocument matchList = new();
                matchList.Add(and);
                matchList.Add(isEmpty);
                matchList.Add(isDemo);

                match = queryBase.AddOperator("$match", matchList);
                return match;
            }
            return match;
        }
        
        public BsonDocument SetDevice(ModelParams modelParams, string msg)
        {
            dynamic query = JsonConvert.DeserializeObject(msg);
            string model = query.PARAMS?.Device.ToString();
            string search = query.PARAMS?.Device.ToString();
            string tags = query.PARAMS?.Tags.ToString();
            BsonDocument match = new BsonDocument();

            if (search == "" && model == "" && tags != "")
            {
                BsonDocument element = queryBase.AddOperator("$in", modelParams.Tags);
                BsonElement isDemo = new("isDemo", modelParams.IsDemo);
                BsonElement isEmpty = new("isEmpty", modelParams.IsEmpty);
                BsonDocument tag = new("tags", element);


                List<BsonValue> list = new();
                list.Add(tag);

                BsonArray array = queryBase.Array(list);
                BsonElement and = new("$and", array);

                BsonDocument matchList = new();
                matchList.Add(and);
                matchList.Add(isEmpty);
                matchList.Add(isDemo);

                match = queryBase.AddOperator("$match", matchList);
                return match;
            }            
            return match;
        }

        public BsonDocument GetMatchRangeAndType(ModelParams modelParams, string msg)
        {
            dynamic query = JsonConvert.DeserializeObject(msg);
            Int32 defaultValueStart = 300;
            Int32 defaultValueEnd = int.MaxValue;
            string fields = query.PARAMS.Range?.Fields;
            Int32 start = query.PARAMS.Range?.Start ?? defaultValueStart;
            Int32 end = query.PARAMS.Range?.End ?? defaultValueEnd;
            BsonDocument matchRange = new BsonDocument();

            if (query.PARAMS.Tags.ToString() == "")
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
            else if (query.PARAMS.Tags.ToString() != "")
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
            BsonDocument userID = queryBase.AddOperator("$eq", modelParams.UserID);
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

        public BsonDocument GetMatchUserRange(string msg)
        {
            dynamic query = JsonConvert.DeserializeObject(msg);
            
            Int32 defaultValueStart = 300;
            Int32 defaultValueEnd = int.MaxValue;
            string fields = query.PARAMS.Range?.Fields;
            Int32 start = query.PARAMS.Range?.Start ?? defaultValueStart;
            Int32 end = query.PARAMS.Range?.End ?? defaultValueEnd;

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
