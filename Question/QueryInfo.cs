using S42.Core;
using System;
using MongoQuery.Builder;
using MongoQuery.Query;
using MongoDB.Bson;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace MongoRequest.Question
{
    class QueryInfo : Actor
    {
        private Builder builder = new();
        private QueryBase qyeryBase = new();
        


        internal Variable Input;
        internal Variable MongoInfo;
        internal Variable Address;

        internal Event ON_OK;
        internal Event ON_BAD;

        public QueryInfo(string name) : base(name)
        {
            Input       = Variables.Add("IN", GetParams);
            MongoInfo   = Variables.Add("MongoInfo");
            Address     = Variables.Add("Address");


            ON_OK       = Events.Add("ON_OK", this);
            ON_BAD      = Events.Add("ON_BAD", this);
        }

        public void GetParams(string msg)
        {
            try
            {
                SetParams(msg);
                ON_OK.Raise();
            }

            catch (Exception e)
            {
                Log.Write(e);
                ON_BAD.Raise();
            }
        }

        private void SetParams(string msg)
        {
            dynamic query = JsonConvert.DeserializeObject(msg);

            int bound = query.PARAMS.bound;
            string TMSN = query.PARAMS.TMSN;
            //----------Filter-------------//
            BsonValue value = qyeryBase.AddOperator("$gte", bound);
            BsonElement tmsn = new("TMSN", TMSN);
            BsonElement userTime = new("userTime", value);            
            List<BsonElement> listFilter = new();
            listFilter.Add(tmsn);
            listFilter.Add(userTime);
            //---------Projection-----------//
            BsonElement _id = new("_id", 0);
            BsonElement _value = new("value", 1);
            BsonElement _tempterature = new("temperature", 1);
            BsonElement _battery = new("battery", 1);
            BsonElement _userTime = new("userTime", 1);
            BsonElement _tmns = new("TMSN", 1);
            List<BsonElement> listProjection = new();
            listProjection.Add(_id);
            listProjection.Add(_battery);
            listProjection.Add(_userTime);
            listProjection.Add(_tmns);
            listProjection.Add(_tempterature);
            listProjection.Add(_value);
            //---------Sort-----------------//
            BsonElement sort = new("userTime", -1);

            BsonDocument find = builder.Find("devices_history", sort, listFilter, listProjection, 1000000);
            MongoInfo.Value = find.ToBsonDocument().ToString();
        }
    }
}
