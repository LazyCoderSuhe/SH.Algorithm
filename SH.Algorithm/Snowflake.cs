using System.Net.Http.Headers;
using System.Threading;
namespace SH.Algorithm
{
    /// <summary>
    /// 雪花算法
    /// </summary>
    public class Snowflake
    {

        // 1 时间戳 41 位   第一个位符号位
        private long startTimestape;
        // 2 数据中心 5 位
        private long dataCenterId = 1;
        // 3 机器码 5 位
        private long workId = 1;
        // 4 序号 12 位
        private static long sequence = 0;

        //5 位数初始化
        private static int dataCenterBit = 5; // 机房5位 32个机房 
        private static int workIdBit = 5;     // 机房5位32个主机   可改成 10位 1024 个主机
        private static int sequenceBit = 12;  // 序号12位4095  6-12

        // 最大 序号数
        private int sequenceBitMaxNum;

        // 移动的位数
        private int startTimestapeShift = dataCenterBit + workIdBit + sequenceBit;
        private int dataCenterShift = workIdBit + sequenceBit;
        private int workIdBitShift = sequenceBit;
        public Snowflake(long dataCenterId, int workId)
        {
            if (dataCenterId >= Math.Pow(2, dataCenterBit))
            {
                throw new ArgumentException($"dataCenterId 大于 配置配置的 Pow(2, {dataCenterBit})");
            }
            this.dataCenterId = dataCenterId;
            if (workId >= Math.Pow(2, workIdBit))
            {
                throw new ArgumentException($"workId 大于 配置配置的 Pow(2, {workIdBit})");

            }
            this.workId = workId;
            sequenceBitMaxNum = (int)Math.Pow(2, sequenceBit);

        }
        static long lastTimeSpan = 0;

        public static object obj = new object();
        public long NextId()
        {
            // 1  获取拼接数据 & | ^
            // 2  去除重复 时间戳
            lock (obj)
            {

                //bool l = false;
                //SpinLock.Enter(ref l);
                GetTimestape();
                if (lastTimeSpan == startTimestape)
                {
                    sequence++;
                    if (sequence >= sequenceBitMaxNum)
                    {
                        GetTimestape();
                        while (lastTimeSpan == startTimestape)
                        {
                            GetTimestape();
                        }
                        sequence = 0;
                    }
                }
                lastTimeSpan = startTimestape;

                return (startTimestape << startTimestapeShift)
                        | (dataCenterId << dataCenterShift)
                        | (workId << workIdBitShift)
                        | sequence;
            }
        }


        // 跟新时间戳
        private void GetTimestape()
        {
            startTimestape = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        }

    }
}