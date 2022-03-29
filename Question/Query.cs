using System;
using S42.Core;
using MongoDB.Bson;
using MongoQuery.Query;
using MongoQuery.Builder;
using System.Collections.Generic;
using MongoRequest.Question.Pipeline;
using MongoRequest.Data.State;

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
        private readonly PipelineList pipelineList = new();
        private readonly Deserializer deserializer = new();

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
            Input       = Variables.Add("IN", GetParams);
            MongoQuery  = Variables.Add("MongoQuery");
            MongoCount  = Variables.Add("MongoCount");
            Address     = Variables.Add("Address");

            ON_OK       = Events.Add("ON_OK", this);
            ON_BAD      = Events.Add("ON_BAD", this);
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
            In query = deserializer.DeserializeTest(msg);

            if (condition.CheckParams(query) || condition.CheckModel(query) || condition.CheckRange(query) || condition.CheckRangeAndType(query))
            {
                GetDevice(query);
            }
            else if (condition.CheckUser(query) || condition.CheckUserAndRange(query) || condition.CheckUserRangeAndModel(query) || condition.CheckUserModel(query))
            {
                GetUser(query);
            }
        }

        /// <summary>
        /// Функция получения и отправки данных счетчиков из БД на страницу
        /// </summary>
        /// <param name="msg"></param>
        private void GetDevice(In msg)
        {
            ModelParams modelParams = paramsBuilder.GenerateDeviceModel(msg);
            bool deviceDefault = condition.CheckParams(msg);
            bool deviceModel = condition.CheckModel(msg);
            bool deviceHaveRange = condition.CheckRange(msg);
            bool deviceHaveRangeAndType = condition.CheckRangeAndType(msg);

            pipelineList.Project = queryBase.AddOperator("$project", modelParams.Project);
            pipelineList.SortItem = new(modelParams.Sort, modelParams.Order);
            pipelineList.Sort = queryBase.AddOperator("$sort", pipelineList.SortItem);
            pipelineList.Skip = queryBase.AddOperator("$skip", modelParams.Skip);
            pipelineList.Limit = queryBase.AddOperator("$limit", modelParams.PageSize);
            pipelineList.CountQuery = queryBase.AddOperator("$count", "devices");

            if (deviceDefault || deviceModel)
            {
                //---------------------------------Device----------------------------------------//                
                pipelineList.Match = matchBuilder.GetMatchDevice(modelParams, msg);

                List<BsonValue> listPipeline = new()
                {
                    pipelineList.Match,
                    pipelineList.Project,
                    pipelineList.Sort,
                    pipelineList.Skip,
                    pipelineList.Limit
                };
                
                BsonDocument device = build.Aggregate("devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = device.ToBsonDocument().ToString();

                Console.WriteLine(device.ToString().Replace(device.ToString(), "Счетчики"));


                //----------------------------------Test----------------------------------------//
                TestValue = device.ToBsonDocument().ToString();
                ExpectedValue = device.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> countPipe = new()
                {
                    pipelineList.Match,
                    pipelineList.Project,                    
                    pipelineList.CountQuery
                };

                BsonDocument count = build.Aggregate("devices", countPipe, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
                Console.WriteLine(count.ToString().Replace(count.ToString(), "Кол-во"));

            }

            if (deviceHaveRange || deviceHaveRangeAndType)
            {
                //---------------------------------Device----------------------------------------//
                pipelineList.Match = matchBuilder.GetMatchRangeAndType(modelParams, msg);

                List<BsonValue> listPipeline = new()
                {
                    pipelineList.Match,
                    pipelineList.Project,
                    pipelineList.Sort,
                    pipelineList.Skip,
                    pipelineList.Limit
                };
                
                BsonDocument device = build.Aggregate("devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = device.ToBsonDocument().ToString();
                Console.WriteLine(device.ToString().Replace(device.ToString(), "Счетчики"));
                //----------------------------------Test----------------------------------------//
                TestValue = device.ToBsonDocument().ToString();
                ExpectedValue = device.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> countPipe = new()
                {
                    pipelineList.Match,
                    pipelineList.Project,
                    pipelineList.CountQuery
                };
                
                BsonDocument count = build.Aggregate("devices", countPipe, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
                Console.WriteLine(count.ToString().Replace(count.ToString(), "Кол-во"));
            }
        }
        
        /// <summary>
        /// Функция получения и отправки данных счетчиков пользователей из БД на страницу
        /// </summary>
        /// <param name="msg"></param>
        private void GetUser(In msg)
        {
            ModelParams modelParams = paramsBuilder.GenerateDeviceModel(msg);            
            bool userDefault = condition.CheckUser(msg);
            bool userRange = condition.CheckUserAndRange(msg);
            bool userHaveRangeAndModel = condition.CheckUserRangeAndModel(msg);
            bool userHaveGsmBox = condition.CheckUserModel(msg);
            
            BsonDocument lookup = queryBase.LookUp("devices", "device", "_id", "devID");
            BsonDocument lookupTags = queryBase.LookUp("tags", "tags", "_id", "tagsID");
            BsonDocument unwind = queryBase.AddOperator("$unwind", "$devID");
            pipelineList.Project = queryBase.AddOperator("$project", modelParams.Project);

            pipelineList.SortItem = new(modelParams.Sort, modelParams.Order);
            pipelineList.Sort = queryBase.AddOperator("$sort", pipelineList.SortItem);
            pipelineList.Skip = queryBase.AddOperator("$skip", modelParams.Skip);
            pipelineList.Limit = queryBase.AddOperator("$limit", modelParams.PageSize);            
            
            pipelineList.CountQuery = queryBase.AddOperator("$count", "devices");

            if (userDefault)
            {
                BsonDocument match = matchBuilder.GetMatchUser(modelParams);

                List<BsonValue> listPipeline = new()
                {
                    match,
                    lookup,
                    unwind,
                    pipelineList.Project,
                    pipelineList.Sort,
                    pipelineList.Skip,
                    pipelineList.Limit
                };

                BsonDocument user = build.Aggregate("user_devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = user.ToBsonDocument().ToString();
                Console.WriteLine(user.ToString().Replace(user.ToString(), "Счетчики"));
                //----------------------------------Test----------------------------------------//
                TestValue = user.ToBsonDocument().ToString();
                ExpectedValue = user.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> listCount = new()
                {
                    match,
                    lookup,
                    unwind,
                    pipelineList.Project,
                    pipelineList.CountQuery
                };

                BsonDocument count = build.Aggregate("user_devices", listCount, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
                Console.WriteLine(count.ToString().Replace(count.ToString(), "Кол-во"));
            }

            if(userRange || userHaveRangeAndModel)
            {
                BsonDocument match = matchBuilder.GetMatchUser(modelParams);
                BsonDocument matchRange = matchBuilder.GetMatchUserRange(msg);

                List<BsonValue> listPipeline = new()
                { 
                    match,
                    lookup,
                    unwind,                    
                    pipelineList.Project,
                    matchRange,
                    pipelineList.Sort,
                    pipelineList.Skip,
                    pipelineList.Limit
                };

                BsonDocument user = build.Aggregate("user_devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = user.ToBsonDocument().ToString();

                Console.WriteLine(user.ToString().Replace(user.ToString(), "Счетчики: Диапазоны / Модели"));
                //----------------------------------Test----------------------------------------//
                TestValue = user.ToBsonDocument().ToString();
                ExpectedValue = user.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> listCount = new()
                { 
                    match,
                    lookup,
                    unwind,
                    pipelineList.Project,
                    matchRange,
                    pipelineList.CountQuery
                };

                BsonDocument count = build.Aggregate("user_devices", listCount, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
                Console.WriteLine(count.ToString().Replace(count.ToString(), "Кол-во: Диапазоны / Модели"));
            }

            if (userHaveGsmBox)
            {
                BsonDocument match = matchBuilder.GetMatchUser(modelParams);
                List<BsonValue> listPipeline = new()
                {
                    match,
                    lookup,
                    unwind,
                    pipelineList.Project,
                    pipelineList.Sort,
                    pipelineList.Skip,
                    pipelineList.Limit
                };

                BsonDocument user = build.Aggregate("user_devices", listPipeline, modelParams.PageSize);
                MongoQuery.Value = user.ToBsonDocument().ToString();
                Console.WriteLine(user.ToString().Replace(user.ToString(), "Счетчики"));
                //----------------------------------Test----------------------------------------//
                TestValue = user.ToBsonDocument().ToString();
                ExpectedValue = user.ToBsonDocument().ToString();

                //---------------------------------Count----------------------------------------//
                List<BsonValue> listCount = new()
                {
                    match,
                    lookup,
                    unwind,
                    pipelineList.Project,
                    pipelineList.CountQuery
                };

                BsonDocument count = build.Aggregate("user_devices", listCount, modelParams.PageSize);
                MongoCount.Value = count.ToBsonDocument().ToString();
                Console.WriteLine(count.ToString().Replace(count.ToString(), "Кол-во"));
            }
        }        
    }
}
