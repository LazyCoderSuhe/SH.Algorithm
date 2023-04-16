using Microsoft.DotNet.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.Algorithm
{
    /// <summary>
    /// 布谷鸟过滤器 
    /// 指纹与 碰撞
    /// 场景 ：大数据推荐
    /// </summary>
    public class CuckooFilter
    {
        /// <summary>
        /// 桶
        /// </summary>
        public byte[] buckets;
        public byte[] bakbuckets;
        /// <summary>
        /// 碰撞阈值处理
        /// </summary>
        private int crachCount = 4;
        /// <summary>
        /// 桶大小
        /// </summary>
        public uint bucketSize;

        public CuckooFilter(uint bucketSize = 1024*1024)
        {
            this.bucketSize = bucketSize;
            buckets = new byte[bucketSize];
            bakbuckets = new byte[bucketSize];
        }

        public bool Add(string val)
        {
            var _crachCount = crachCount;
            // 1 获取Hash
            var hash = (uint)Hash(val);
            // 2 与桶的长度 取模
            var index = hash % bucketSize;
            // 3 生成 指纹(8bit)
            var fingerprint = (byte)(hash & 0xff);
            // 4 存入
            if (buckets[index] == 0) // 第一个桶中没有 指纹碰撞
            {
                buckets[index] = fingerprint;
                return true;
            }
            // 5 指纹碰撞后 存入 备用桶
            // 5.1 指纹 Hash
            hash = (uint)Hash(fingerprint);

            var hashvalue = hash ^ 0;
            // 5.2 取模
            index = hashvalue % bucketSize;
            // 5.4 存储
            if (bakbuckets[index] == 0)// 第二个桶中没有 指纹碰撞
            {
                bakbuckets[index] = fingerprint;
                return true;
            }
            do
            {
                _crachCount--;
                // 6 双桶发生碰撞后 任意一桶 放弃指纹 旧的值 存取另一个桶
                // 6.1 随机桶
                uint rendomIndex = (uint)new Random().Next(2);
                byte[] temp;
                byte[] backtmp;
                if (rendomIndex == 0)
                {
                    temp = buckets;
                    backtmp = bakbuckets;
                }
                else
                {
                    temp = bakbuckets;
                    backtmp = buckets;
                }
                // 保留原有之
                byte oldfingerprint = temp[index];
                // 存入新值 
                temp[index] = fingerprint;
                // 重新计算 存入到 另一个桶
                hash = (uint)Hash(fingerprint);
                hashvalue = hash ^ rendomIndex;
                index = hashvalue % bucketSize;
                if (backtmp[index] == 0)
                {
                    backtmp[index] = oldfingerprint;
                    return true;
                }
            } while (crachCount > 0);
            return false;
        }

        public bool Container(string val)
        {
            var hash = (uint)Hash(val);
            // 2 与桶的长度 取模
            var index = hash % bucketSize;
            // 3 生成 指纹(8bit)
            var fingerprint = (byte)(hash & 0xff);            
            if (buckets[index] == fingerprint)
            {
                return true;
            }
            hash = (uint)Hash(fingerprint);

            var hashvalue = hash ^ 0;
            // 5.2 取模
            index = hashvalue % bucketSize;
            // 5.4 存储
            if (bakbuckets[index] == fingerprint)// 第二个桶中没有 指纹碰撞
            {
                bakbuckets[index] = fingerprint;
                return true;
            }
            return false;
        }
        public bool Remove(string val)
        {
            var hash = (uint)Hash(val);
            // 2 与桶的长度 取模
            var index = hash % bucketSize;
            // 3 生成 指纹(8bit)
            var fingerprint = (byte)(hash & 0xff);
            if (buckets[index] == fingerprint)
            {
                buckets[index] = 0;
                return true;
            }
            hash = (uint)Hash(fingerprint);

            var hashvalue = hash ^ 0;
            // 5.2 取模
            index = hashvalue % bucketSize;
            // 5.4 存储
            if (bakbuckets[index] == fingerprint)// 第二个桶中没有 指纹碰撞
            {
                bakbuckets[index] = 0;
                return true;
            }
            return false;
        }



        private int Hash(object item)
        {
            HashCodeCombiner hashCodeCombiner = new HashCodeCombiner();
            hashCodeCombiner.Add(item);
            return hashCodeCombiner.GetHashCode();
        }
    }
}
