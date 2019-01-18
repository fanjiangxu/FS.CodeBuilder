using System;
using System.Text;

namespace FS.CodeBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var tableNameList = "TB_ActivityDictionary,TB_ActivityDictionaryDetail,TB_SignIn_Active,TB_SignIn_ActiveDetail,TB_SignInRecord";
            foreach (var name in tableNameList.Split(','))
            {
                ////生成model
                //TableEntityHelper.CreateModel(name);
                ////生成Dal
                //TableEntityHelper.CreateTrade(name);
                //生成PageList
                TableEntityHelper.CreatePageList(name);
            }
            Console.WriteLine("完成!");
            Console.ReadKey();
        }
    }
}
