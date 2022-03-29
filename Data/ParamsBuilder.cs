using System;
using S42.Core;
using MongoDB.Bson;
using System.Collections.Generic;


namespace MongoRequest
{
    public class ParamsBuilder
    {
        private readonly ModelParams modelParams = new();
        private readonly Condition condition = new();
        public ModelParams GenerateDeviceModel(In query)
        {                        
            ModelParams modelParams = new();
            try
            {
                if (condition.CheckParams(query))
                {
                    modelParams = SetModelDefault(query);
                }

                if (condition.CheckModel(query))
                {
                    modelParams = SetModelDevice(query);
                }

                if (condition.CheckRangeAndType(query))
                {
                    modelParams = SetModelDeviceRange(query);
                }

                if (condition.CheckRange(query))
                {
                    modelParams = SetDeviceRange(query);
                }

                if (condition.CheckUser(query))
                {
                    modelParams = SetModelUser(query);
                }

                if (condition.CheckUserAndRange(query) || condition.CheckUserRangeAndModel(query) || condition.CheckUserModel(query))
                {
                    modelParams = SetUserRange(query);
                }
            }

            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetModelDefault(In query)
        {
            try
            {
                if(query.Params.Device == null)
                {
                    modelParams.IsDemo = false;
                    modelParams.IsEmpty = false;
                }
                modelParams.Sort = query.Params.Sort;
                modelParams.Order = query.Params.Order;

                modelParams.PageSize = query.Params.Paging.PageSize;
                modelParams.PageNumber = query.Params.Paging.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetModelDevice(In query)
        {
            try
            {
                if (query.Params.Device != null)
                {
                    modelParams.DeviceID = query.Params.Device?.MeterType;
                    modelParams.SearchID = query.Params.Device?.Search;
                    modelParams.IsDemo = query.Params.Device.IsDemo;
                    modelParams.IsEmpty = query.Params.Device.IsEmpty;
                }
                else if (query.Params.Device == null)
                {
                    modelParams.DeviceID = "";
                    modelParams.SearchID = "";
                }
                modelParams.UserID = GetUser(query);
                modelParams.Tags = GetTags(query);

                modelParams.Sort = query.Params.Sort;
                modelParams.Order = query.Params.Order;

                modelParams.PageSize = query.Params.Paging.PageSize;
                modelParams.PageNumber = query.Params.Paging.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetModelDeviceRange(In query)
        {
            try
            {
                if (query.Params.Device != null)
                {
                    modelParams.DeviceID = query.Params.Device.MeterType;
                    modelParams.SearchID = query.Params.Device.Search;
                    modelParams.IsDemo = query.Params.Device.IsDemo;
                    modelParams.IsEmpty = query.Params.Device.IsEmpty;
                }
                else if (query.Params.Device == null)
                {
                    modelParams.DeviceID = query.Params.Device.MeterType;
                    modelParams.SearchID = query.Params.Device.Search;
                }
                modelParams.Tags = GetTags(query);

                modelParams.Sort = query.Params.Sort;
                modelParams.Order = query.Params.Order;

                modelParams.PageSize = query.Params.Paging.PageSize;
                modelParams.PageNumber = query.Params.Paging.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetDeviceRange(In query)
        {
            try
            {
                if (query.Params.Device != null)
                {
                    modelParams.DeviceID = query.Params.Device.MeterType;
                    modelParams.SearchID = query.Params.Device.Search;
                    modelParams.IsDemo = query.Params.Device.IsDemo;
                    modelParams.IsEmpty = query.Params.Device.IsEmpty;
                }
                else if (query.Params.Device == null)
                {
                    modelParams.DeviceID = "^";
                    modelParams.SearchID = "";
                }
                modelParams.Sort = query.Params.Sort;
                modelParams.Order = query.Params.Order;
                modelParams.Tags = GetTags(query);

                modelParams.PageSize = query.Params.Paging.PageSize;
                modelParams.PageNumber = query.Params.Paging.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetModelUser(In query)
        {
            try
            {
                modelParams.DeviceID = query.Params.Device.MeterType;
                modelParams.SearchID = query.Params.Device.Search;
                modelParams.UserID = GetUser(query);
                modelParams.Tags = GetTags(query);

                modelParams.Sort = query.Params.Sort;
                modelParams.Order = query.Params.Order;

                modelParams.PageSize = query.Params.Paging.PageSize;
                modelParams.PageNumber = query.Params.Paging.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetUserRange(In query)
        {
            try
            {
                modelParams.DeviceID = query.Params.Device?.MeterType;
                modelParams.SearchID = query.Params.Device?.Search;
                modelParams.UserID = GetUser(query);
                modelParams.Tags = GetTags(query);

                modelParams.Sort = query.Params.Sort;
                modelParams.Order = query.Params.Order;

                modelParams.PageSize = query.Params.Paging.PageSize;
                modelParams.PageNumber = query.Params.Paging.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        private List<BsonElement> GetProject(In query)
        {
            string user = query.Params.User?.ToString();
            bool userIsNull = user == "" || user == null;
            bool userIsNotNull = user != "" || user != null;

            BsonElement value;
            List<BsonElement> element = new();
            var project = query.Params.Project;

            if (userIsNull)
            {
                foreach (var item in project)
                {
                    string elements = item;
                    var projection = SetDeviceProjection();
                    projection.TryGetElement(elements, out value);
                    element.Add(value);
                }
                return element;
            }
            else if (userIsNotNull)
            {
                foreach (var item in project)
                {
                    string elements = item;
                    var projection = SetUserProjection();
                    projection.TryGetElement(elements, out value);
                    element.Add(value);
                }
                return element;
            }
            return null;
        }

        private static List<string> GetTags(In query)
        {
            var tags = query.Params.Tags;
            
            List<string> elements = new();

            if (tags != null)
            {
                foreach (string item in tags)
                {
                    elements.Add(item);
                }
            }
            else if (tags == null)
            {
                return elements;
            }
            return elements;
        }

        private static List<string> GetUser(In query)
        {
            var user = query.Params.User;
            
            List<string> elements = new();

            if (user != null)
            {
                foreach (var item in user)
                {
                    elements.Add(item);
                }
            }
            else if (user == null)
            {
                return elements;
            }
            return elements;
        }

        private static BsonDocument SetUserProjection()
        {
            var deviceUser = new Dictionary<string, object>
            {
                {"_id", "$devID._id" },
                {"TMSN", 1 },
                {"serialNum", "$devID.serialNum" },
                {"deviceType", "$devID.deviceType" },
                
            };
            var document = deviceUser.ToBsonDocument();
            return document;
        }

        private static BsonDocument SetDeviceProjection()
        {
            var device1 = new Dictionary<string, object>
            {
                {"_id", 1 },
                {"serialNum", 1 },
                {"deviceType",  1 },
                {"modelParams",  1 },
                {"lastUpDate", 1 },
                {"releaseDate", 1 },
                {"firmwareVersion", 1 },
                {"checkUpDate", 1 },
                {"TMSN", 1 },
                {"tags", 1},                
                
            };

            var document = device1.ToBsonDocument();
            return document;
        }
    }
}
