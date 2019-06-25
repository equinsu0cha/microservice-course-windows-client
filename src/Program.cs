using System;
using System.Diagnostics;
using WMIdataCollector.Job;
using WMIdataCollector.Scheduller;

namespace WMIdataCollector
{
    public static class ExecutionCfg
    {
        public static string MacAddressFromArgs { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                ExecutionCfg.MacAddressFromArgs = args[0];
            }




            //CustomScheduller.Start();


#if DEBUG
            WMIdataGetter getter = new WMIdataGetter();
            getter.WmiCollectorExecution();
#else
                CustomScheduller.Start();
#endif


            Console.ReadLine();
        }
    }
}
