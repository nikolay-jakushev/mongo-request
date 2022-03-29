namespace MongoRequest
{
    public class Condition
    {
        public bool CheckParams(In query)
        {
            //Существует ли пользователь
            var user = query.Params.User;
            //Существует ли диапазон
            var range = query.Params.Range;
            var device = query.Params.Device;
            var tags = query.Params.Tags;

            return user == null && range == null && device == null && tags == null;
        }

        public bool CheckModel(In query)
        {
            //Существует ли пользователь
            var user = query.Params.User;
            //Существует ли диапазон
            var range = query.Params.Range;
            var device = query.Params.Device;
            var tags = query.Params.Tags;

            return user == null && range == null && (device != null || tags != null);
        }

        public bool CheckRange(In query)
        {
            //Существует ли пользователь
            var user = query.Params.User;
            //Существует ли диапазон
            var range = query.Params.Range;
            var device = query.Params.Device;
            var tags = query.Params.Tags;

            return user == null && range != null && (device == null || tags == null);
        }

        public bool CheckRangeAndType(In query)
        {
            //Существует ли пользователь
            var user = query.Params.User;
            //Существует ли диапазон
            var range = query.Params.Range;
            var device = query.Params.Device;
            var tags = query.Params.Tags;

            return user == null && range != null && (device != null || tags != null);
        }

        public bool CheckUser(In query)
        {
            //Существует ли пользователь
            var user = query.Params.User;
            //Существует ли диапазон
            var range = query.Params.Range;
            var device = query.Params.Device;
            var tags = query.Params.Tags;

            return user != null && range == null && (device == null || device != null);
        }

        public bool CheckUserAndRange(In query)
        {
            //Существует ли пользователь
            var user = query.Params.User;
            //Существует ли диапазон
            var range = query.Params.Range;
            var device = query.Params.Device;
            var tags = query.Params.Tags;

            return user != null && range != null && device == null;
        }

        public bool CheckUserRangeAndModel(In query)
        {
            //Существует ли пользователь
            var user = query.Params.User;
            //Существует ли диапазон
            var range = query.Params.Range;
            var device = query.Params.Device;
            var tags = query.Params.Tags;

            return user != null && range != null && device != null;
        }

        public bool CheckUserModel(In query)
        {
            //Существует ли пользователь
            var user = query.Params.User;
            //Существует ли диапазон
            var range = query.Params.Range;
            var device = query.Params.Device;
            var tags = query.Params.Tags;

            return user != null && range == null && device != null;
        }
    }
}
