using Microsoft.DotNet.PlatformAbstractions;
using System.Collections;

namespace SH.Algorithm
{
    /// <summary>
    /// BoomFilter 判断是否存在
    /// 误判 ，是hash 碰撞的问题
    ///  多个生成 几个hash 解决
    /// </summary>
    public class BoomFilter
    {
        public BitArray bitArray;
        public int bitArraySize;
        private readonly int hashValue;
        /// <summary>
        ///  大数据中 判断 过滤 是否存在，的中间层 处理
        ///  默认配置 是 10MB 空间处理 81,920
        ///    
        /// 1字节 = 1byte = 1B = 8位；
        /// 1KB = 1024B;    8192
        /// 1MB = 1024KB;   838-8608
        /// 1GB = 1024MB:   85-8993-4592
        /// </summary>
        /// <param name="bitArraySize">8 bit *1024(1MB)*10(10M) </param>
        /// <param name="hashValue">8 bit *1024(1MB)*10(10M) /3</param>
        public BoomFilter(int bitArraySize = 8 * 1024 * 10, int hashValue = 3)
        {
            this.bitArraySize = bitArraySize;
            this.hashValue = hashValue;
            this.bitArray = new BitArray(1000);
        }
        public void Add(string value)
        {
            //1 数据HASH
            for (int i = 0; i < hashValue; i++)
            {
                int hashValue = Hash(value, i);
                //2 数据取模
                var index = hashValue / this.bitArraySize;
                //3 将数据存入 数组
                bitArray.Set(index, true);
            }
        }
        public bool Container(string value)
        {
            for (int i = 0; i < hashValue; i++)
            {
                //1 数据HASH
                int hashValue = Hash(value, i);
                //2 数据取模
                var index = hashValue / this.bitArraySize;
                //3 索引中取值
                var flag = this.bitArray.Get(index);
                //4 检查 是否存在
                if (!flag)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 误判 ，是hash 碰撞的问题
        ///  多个生成 几个hash 解决
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int Hash(string item, int index)
        {
            HashCodeCombiner hashCodeCombiner = new HashCodeCombiner();
            hashCodeCombiner.Add(item);
            hashCodeCombiner.Add(index);
            return hashCodeCombiner.GetHashCode();
        }
    }
}
