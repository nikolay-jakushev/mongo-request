using System;
using S42.Core;
using MongoQuery;
using MongoQuery.Query;
using MongoDB.Bson;
using System.Collections.Generic;




namespace MongoRequest.Question
{
    /// <summary>
    /// Универсальный запросчик для страницы
    /// </summary>
    public class Query : Actor
    {
        /// <summary>
        /// Тестовые переменные
        /// </summary>
        public string TestValue { get; set; }
        public string ExpectedValue { get; set; }

        /// <summary>
        /// Классы для создания запроса в БД
        /// </summary>
        private readonly Condition condition = new();
        private readonly MatchBuilder matchBuilder = new();
        private readonly ParamsBuilder paramsBuilder = new();
        private readonly Builder build = new();
        private readonly QueryBase queryBase = new();

        /// <summary>
        /// Точка входа
        /// </summary>
        internal Variable Input;

        /// <summary>
        /// Переменная отправляющая запрос в БД
        /// </summary>
        internal Variable MongoQuery;

        /// <summary>
        /// Переменная отправляющая запрос на кол-во счетчиков в условии
        /// </summary>
        internal Variable MongoCount;

        /// <summary>
        /// Переменная адресата
        /// </summary>
        internal Variable Address;

        /// <summary>
        /// Успешное событие на отправку запроса в БД
        /// </summary>
        internal Event ON_OK;

        /// <summary>
        /// Неудачное событие на отправку запроса в БД
        /// </summary>
        internal Event ON_BAD;

        /// <summary>
        /// Конструктор принимающий сообщение S42 со страницы
        /// </summary>
        /// <param name="name"></param>
        public Query(string name) : base(name)
        {
            Input = Variables.Add("IN", GetParams);
            MongoQuery = Variables.Add("MongoQuery");
            MongoCount = Variables.Add("MongoCount");
            Address = Variables.Add("Address");

            ON_OK = Events.Add("ON_OK", this);
            ON_BAD = Events.Add("ON_BAD", this);
        }

        /// <summary>
        /// Функция получения данных и отправки данных
        /// </summary>
        /// <param name="msg"></param>
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

        /// <summary>
        /// Функция, определяющая какой запрос пришел со страницы
        /// </summary>
        /// <param name="msg"></param>
        private void SetParams(string msg)
        {
            if (condition.CheckParams(msg) || condition.CheckModel(msg) || condition.CheckRange(msg) || condition.CheckRangeAndType(msg))
            {
                GetDevice(msg);
            }
            else if (condition.CheckUser(msg) || condition.CheckUserAndRange(msg) || condition.CheckUserRangeAndModel(msg) || condition.CheckUserModel(msg))
            {
                GetUser(msg);
            }
        }

        /// <summary>
        /// Функция получения и отправки данных счетчиков из БД на страницу
        /// </summary>
        /// <param name="msg"></param>
        private void GetDevice(string msg)
        {
            ModelParams modelParams = paramsBuilder.GenerateDeviceModel(msg);            
            bool deviceDefault = condition.CheckParams(msg);
            bool deviceModel = condition.CheckModel(msg);
            bool deviceHaveRange = condition.CheckRange(msg);
            bool deviceHaveRangeAndType = condition.CheckRangeAndType(msg);

            BsonDocument project = queryBase.AddOperator("$project", modelParams.Project);

            BsonElement sortItem = new(modelParams.Sort, modelParams.Order);            
            BsonDocument sort = queryBase.AddOperator("$sort", sortItem);
            BsonDocument skip = queryBase.AddOperator("$skip", modelParams.Skip);
            BsonDocument limit = queryBase.AddOperator("$limit", modelParams.PageSize);

            BsonDocument countQuery = queryBase.AddOperator("$count", "devices");

            if (deviceDefault)
            {
                //---------------------------------Device----------------------------------------//
                BsonDocument matchDefault = matchBuilder.GetMatchDefault(modelParams);

                List<BsonValue> listPipeline = new();
                listPipeline.Add(matchDefault);
                listPipeline.Add(project);
                listPipeline.Add(sort);
                listPipeline.Add(skip);
                listPipeline.Add(limit);

                BsonDocument devices = build.Aggregate("devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = devices.ToBsonDocument().ToString();
                //----------------------------------Test----------------------------------------//
                TestValue = devices.ToBsonDocument().ToString();
                ExpectedValue = devices.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> countPipe = new();
                countPipe.Add(countQuery);
                BsonDocument count = build.Aggregate("devices", countPipe, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
                
            }
            else if (deviceModel)
            {
                //---------------------------------Device----------------------------------------//
                BsonDocument match = matchBuilder.GetMatch(modelParams, msg);
                List<BsonValue> listPipeline = new();
                listPipeline.Add(match);
                listPipeline.Add(project);
                listPipeline.Add(sort);
                listPipeline.Add(skip);
                listPipeline.Add(limit);

                BsonDocument device = build.Aggregate("devices", listPipeline, modelParams.PageSize);                
                MongoQuery.Value = device.ToBsonDocument().ToString();
                //----------------------------------Test----------------------------------------//
                TestValue = device.ToBsonDocument().ToString();
                ExpectedValue = device.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> countPipe = new();
                countPipe.Add(match);
                countPipe.Add(countQuery);
                BsonDocument count = build.Aggregate("devices", countPipe, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
            }
            else if (deviceHaveRange || deviceHaveRangeAndType)
            {
                //---------------------------------Device----------------------------------------//                
                BsonDocument matchRange = matchBuilder.GetMatchRangeAndType(modelParams, msg);
                List<BsonValue> listPipeline = new();
                listPipeline.Add(matchRange);
                listPipeline.Add(project);
                listPipeline.Add(sort);
                listPipeline.Add(skip);
                listPipeline.Add(limit);

                BsonDocument device = build.Aggregate("devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = device.ToBsonDocument().ToString();
                //----------------------------------Test----------------------------------------//
                TestValue = device.ToBsonDocument().ToString();
                ExpectedValue = device.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> countPipe = new();
                countPipe.Add(matchRange);
                countPipe.Add(countQuery);
                BsonDocument count = build.Aggregate("devices", countPipe, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
            }
        }
        
        /// <summary>
        /// Функция получения и отправки данных счетчиков пользователей из БД на страницу
        /// </summary>
        /// <param name="msg"></param>
        private void GetUser(string msg)
        {
            ModelParams modelParams = paramsBuilder.GenerateDeviceModel(msg);            
            bool userDefault = condition.CheckUser(msg);
            bool userRange = condition.CheckUserAndRange(msg);
            bool userHaveRangeAndModel = condition.CheckUserRangeAndModel(msg);
            bool userHaveGsmBox = condition.CheckUserModel(msg);
            
            BsonDocument lookup = queryBase.LookUp("devices", "device", "_id", "devID");
            BsonDocument lookupTags = queryBase.LookUp("tags", "tags", "_id", "tagsID");
            BsonDocument unwind = queryBase.AddOperator("$unwind", "$devID");
            BsonDocument project = queryBase.AddOperator("$project", modelParams.Project);

            BsonElement sortItem = new(modelParams.Sort, modelParams.Order);            
            BsonDocument sort = queryBase.AddOperator("$sort", sortItem);
            BsonDocument skip = queryBase.AddOperator("$skip", modelParams.Skip);
            BsonDocument limit = queryBase.AddOperator("$limit", modelParams.PageSize);            

            BsonDocument countQuery = queryBase.AddOperator("$count", "devices");

            if (userDefault)
            {
                BsonDocument matchDefault = matchBuilder.GetMatchUser(modelParams);

                List<BsonValue> listPipeline = new()
                {
                    matchDefault,
                    lookup,
                    unwind,
                    project,
                    sort,
                    skip,
                    limit
                };

                BsonDocument user = build.Aggregate("user_devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = user.ToBsonDocument().ToString();
                //----------------------------------Test----------------------------------------//
                TestValue = user.ToBsonDocument().ToString();
                ExpectedValue = user.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> listCount = new()
                {
                    matchDefault,
                    lookup,
                    unwind,
                    countQuery
                };
                BsonDocument count = build.Aggregate("user_devices", listCount, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
            }
            else if(userRange || userHaveRangeAndModel)
            {                
                BsonDocument matchDefault = matchBuilder.GetMatchUser(modelParams);
                BsonDocument matchRange = matchBuilder.GetMatchUserRange(msg);

                List<BsonValue> listPipeline = new()
                { 
                    matchDefault,
                    lookup,
                    unwind,
                    project,
                    matchRange,
                    sort,
                    skip,
                    limit
                };
                BsonDocument user1 = build.Aggregate("user_devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = user1.ToBsonDocument().ToString();
                //----------------------------------Test----------------------------------------//
                TestValue = user1.ToBsonDocument().ToString();
                ExpectedValue = user1.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> listCount = new()
                { 
                    matchDefault,
                    lookup,
                    unwind,
                    matchRange,
                    countQuery
                };
                BsonDocument count = build.Aggregate("user_devices", listCount, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
            }
            else if (userHaveGsmBox)
            {
                BsonDocument matchDefault = matchBuilder.GetMatchUser(modelParams);
                List<BsonValue> listPipeline = new()
                {
                    matchDefault,
                    lookup,
                    unwind,
                    project,
                    sort,
                    skip,
                    limit
                };

                BsonDocument user1 = build.Aggregate("user_devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = user1.ToBsonDocument().ToString();
                //----------------------------------Test----------------------------------------//
                TestValue = user1.ToBsonDocument().ToString();
                ExpectedValue = user1.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> listCount = new()
                {
                    matchDefault,
                    lookup,
                    unwind,
                    countQuery
                };

                BsonDocument count = build.Aggregate("user_devices", listCount, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();                
            }
        }        
    }
}
