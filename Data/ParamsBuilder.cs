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
        private readonly Deserializer deserializer = new();
        public ModelParams GenerateDeviceModel(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            ModelParams modelParams = new();
            try
            {
                if (condition.CheckParams(msg))
                {
                    modelParams = SetModelDefault(query);
                }
                else if (condition.CheckModel(msg))
                {
                    modelParams = SetModelDevice(query);
                }
                else if (condition.CheckRangeAndType(msg))
                {
                    modelParams = SetModelDeviceRange(query);
                }
                else if (condition.CheckRange(msg))
                {
                    modelParams = SetDeviceRange(query);
                }
                else if (condition.CheckUser(msg))
                {
                    modelParams = SetModelUser(query);
                }
                else if (condition.CheckUserAndRange(msg) || condition.CheckUserRangeAndModel(msg) || condition.CheckUserModel(msg))
                {
                    modelParams = SetUserRange(query);
                }
            }

            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetModelDefault(dynamic query)
        {
            try
            {
                modelParams.Sort = query.PARAMS?.Sort;
                modelParams.Order = query.PARAMS?.Order;

                modelParams.PageSize = query.PARAMS?.Paging?.PageSize;
                modelParams.PageNumber = query.PARAMS?.Paging?.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetModelDevice(dynamic query)
        {
            try
            {
                if (query.PARAMS?.Device.ToString() != "")
                {
                    modelParams.DeviceID = query.PARAMS?.Device?.MeterType;
                    modelParams.SearchID = query.PARAMS?.Device?.Search;
                    modelParams.IsDemo = query.PARAMS?.Device?.IsDemo;
                    modelParams.IsEmpty = query.PARAMS?.Device?.IsEmpty;
                }
                else if (query.PARAMS?.Device.ToString() == "")
                {
                    modelParams.DeviceID = query.PARAMS?.Device;
                    modelParams.SearchID = query.PARAMS?.Device;
                }
                modelParams.UserID = GetUser(query);
                modelParams.Tags = GetTags(query);

                modelParams.Sort = query.PARAMS?.Sort;
                modelParams.Order = query.PARAMS?.Order;

                modelParams.PageSize = query.PARAMS?.Paging?.PageSize;
                modelParams.PageNumber = query.PARAMS?.Paging?.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetModelDeviceRange(dynamic query)
        {
            try
            {
                if (query.PARAMS?.Device.ToString() != "")
                {
                    modelParams.DeviceID = query.PARAMS?.Device?.MeterType;
                    modelParams.SearchID = query.PARAMS?.Device?.Search;
                    modelParams.IsDemo = query.PARAMS?.Device.IsDemo;
                    modelParams.IsEmpty = query.PARAMS?.Device.IsEmpty;
                }
                else if (query.PARAMS?.Device.ToString() == "")
                {
                    modelParams.DeviceID = query.PARAMS?.Device;
                    modelParams.SearchID = query.PARAMS?.Device;
                }
                modelParams.Tags = GetTags(query);

                modelParams.Sort = query.PARAMS?.Sort;
                modelParams.Order = query.PARAMS?.Order;

                modelParams.PageSize = query.PARAMS?.Paging?.PageSize;
                modelParams.PageNumber = query.PARAMS?.Paging?.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetDeviceRange(dynamic query)
        {
            try
            {
                modelParams.Sort = query.PARAMS?.Sort;
                modelParams.Order = query.PARAMS?.Order;
                modelParams.Tags = GetTags(query);

                modelParams.PageSize = query.PARAMS?.Paging?.PageSize;
                modelParams.PageNumber = query.PARAMS?.Paging?.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetModelUser(dynamic query)
        {
            try
            {
                modelParams.DeviceID = query.PARAMS?.Device?.MeterType;
                modelParams.SearchID = query.PARAMS?.Device?.Search;
                modelParams.UserID = GetUser(query);
                modelParams.Tags = GetTags(query);

                modelParams.Sort = query.PARAMS?.Sort;
                modelParams.Order = query.PARAMS?.Order;

                modelParams.PageSize = query.PARAMS?.Paging?.PageSize;
                modelParams.PageNumber = query.PARAMS?.Paging?.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        public ModelParams SetUserRange(dynamic query)
        {
            try
            {
                modelParams.DeviceID = query.PARAMS?.Device?.MeterType;
                modelParams.SearchID = query.PARAMS?.Device?.Search;
                modelParams.UserID = GetUser(query);
                modelParams.Tags = GetTags(query);

                modelParams.Sort = query.PARAMS?.Sort;
                modelParams.Order = query.PARAMS?.Order;

                modelParams.PageSize = query.PARAMS?.Paging?.PageSize;
                modelParams.PageNumber = query.PARAMS?.Paging?.PageNumber;
                modelParams.Skip = (modelParams.PageNumber - 1) * modelParams.PageSize;

                modelParams.Project = GetProject(query);
            }
            catch (Exception e) { Log.Write(e); }
            return modelParams;
        }

        private List<BsonElement> GetProject(dynamic query)
        {
            string user = query.PARAMS.User?.ToString();
            bool userIsNull = user == "" || user == null;
            bool userIsNotNull = user != "" || user != null;

            BsonElement value;
            List<BsonElement> element = new();
            var project = query.PARAMS.Project;

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

        private static List<string> GetTags(dynamic query)
        {
            var tags = query.PARAMS?.Tags;
            List<string> elements = new();

            if (tags.ToString() != "")
            {
                foreach (string item in tags)
                {
                    elements.Add(item);
                }
            }
            else if (tags.ToString() == "")
            {
                return elements;
            }
            return elements;
        }

        private static List<string> GetUser(dynamic query)
        {
            var user = query.PARAMS?.User;
            List<string> elements = new();

            if (user.ToString() != "")
            {
                foreach (string item in user)
                {
                    elements.Add(item);
                }
            }
            else if (user.ToString() == "")
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
                
            };

            var document = device1.ToBsonDocument();
            return document;
        }
    }
}
