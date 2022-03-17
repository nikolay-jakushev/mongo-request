using S42.Core;
using MongoRequest.Question;
using System;

namespace API
{
    class Program
    {
        static void Main(string[] args)
        {

            string name = "RequestAPI";
            Console.Title = name;
            Console.WriteLine(name);

            Module module = new(name);
            module.BindActor(new Query("Query"));
            module.BindActor(new QueryInfo("QueryInfo"));
            module.Run();
        }
    }
}
