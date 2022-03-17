



namespace MongoRequest
{
    public class Condition
    {
        private readonly Deserializer deserializer = new();
        public bool CheckParams(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            //Существует ли пользователь
            string user = query.PARAMS.User?.ToString();

            //Существует ли диапазон
            string range = query.PARAMS.Range?.ToString();
            string device = query.PARAMS.Device?.ToString();
            string tags = query.PARAMS.Tags?.ToString();

            return user == "" && range == "" && device == "" && tags == "";
        }

        public bool CheckModel(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            //Существует ли пользователь
            string user = query.PARAMS.User?.ToString();

            //Существует ли диапазон
            string range = query.PARAMS.Range?.ToString();
            string device = query.PARAMS.Device?.ToString();
            string tags = query.PARAMS?.Tags.ToString();

            return user == "" && range == "" && (device != "" || tags != "");
        }

        public bool CheckRange(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            //Существует ли пользователь
            string user = query.PARAMS.User?.ToString();

            //Существует ли диапазон
            string range = query.PARAMS.Range?.ToString();
            string device = query.PARAMS.Device?.ToString();
            string tags = query.PARAMS?.Tags.ToString();

            return user == "" && range != "" && device == "" && tags == "";
        }

        public bool CheckRangeAndType(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            //Существует ли пользователь
            string user = query.PARAMS.User?.ToString();

            //Существует ли диапазон
            string range = query.PARAMS.Range?.ToString();
            string device = query.PARAMS.Device?.ToString();
            string tags = query.PARAMS?.Tags.ToString();

            return user == "" && range != "" && (device != "" || tags != "");
        }

        public bool CheckUser(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            //Существует ли пользователь
            string user = query.PARAMS.User?.ToString();
            //Существует ли диапазон
            string range = query.PARAMS.Range?.ToString();
            string device = query.PARAMS.Device?.MeterType.ToString();
            return user != "" && range == "" && device == "^";
        }

        public bool CheckUserAndRange(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            //Существует ли пользователь
            string user = query.PARAMS.User?.ToString();
            //Существует ли диапазон
            string range = query.PARAMS.Range?.ToString();
            string device = query.PARAMS.Device?.MeterType.ToString();
            return user != "" && range != "" && device == "^";
        }

        public bool CheckUserRangeAndModel(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            //Существует ли пользователь
            string user = query.PARAMS.User?.ToString();
            //Существует ли диапазон
            string range = query.PARAMS.Range?.ToString();
            string device = query.PARAMS.Device?.MeterType.ToString();
            return user != "" && range != "" && device != "^";
        }

        public bool CheckUserModel(string msg)
        {
            dynamic query = deserializer.Deserialize(msg);
            //Существует ли пользователь
            string user = query.PARAMS.User?.ToString();
            //Существует ли диапазон
            string range = query.PARAMS.Range?.ToString();
            string device = query.PARAMS.Device?.MeterType.ToString();
            return user != "" && range == "" && device != "^";
        }
    }
}
