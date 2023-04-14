using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SH.Algorithm;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           Stopwatch stopwatch = new  Stopwatch();
            stopwatch.Start();
            ConcurrentQueue<long> strings = NewMethod();
            stopwatch.Stop();
            var t1 = stopwatch.ElapsedTicks;
         
            //List<long> sss=new List<long> ();
            //strings.ForEach(t =>
            //{
            //    long s = 0;
            //    int c = 0;
            //    for (int i = 0; i < strings.Count; i++)
            //    {
            //        if (t == strings[i])
            //        {
            //            c++;
            //            if (c > 1)
            //                sss.Add(strings[i]);
            //        }

            //    }
            //});



            var s = strings.Distinct().ToList();

            //Assert.IsTrue(strings.Count == 1000000);
        }
   
        private static ConcurrentQueue<long> NewMethod()
        {
            ConcurrentQueue<long> strings = new ConcurrentQueue<long>();
            Snowflake snowflake = new Snowflake(1, 1);
            SpinLock spinLock = new SpinLock();
            Parallel.For(0, 100000, t =>
            {
                try
                {
                    var loc = false;
                    spinLock.Enter(ref loc);
                    strings.Enqueue(snowflake.NextId());

                }
                finally
                {
                    spinLock.Exit();
                }
            });
            return strings;
        }
    }
}