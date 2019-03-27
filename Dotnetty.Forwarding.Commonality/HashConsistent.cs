using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnetty.Forwarding.Commonality
{
    public class HashConsistent
    {
        /// <summary>
        /// 生成对应的哈希key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ulong Md5Hash(string key)
        {
            using (var hash = System.Security.Cryptography.MD5.Create())
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                var a = BitConverter.ToUInt64(data, 0);
                var b = BitConverter.ToUInt64(data, 8);
                ulong hashCode = a ^ b;
                return hashCode;
            }
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="keyValues"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TVal GetTargetValues<TVal>(IDictionary<ulong, TVal> keyValues,string key)
        {
            ulong hash = Md5Hash(key);
            ulong firstNode = ModifiedBinarySearch(keyValues.Keys.ToArray(), hash);
            return keyValues[firstNode];
        }

        /// <summary>
        /// 计算key的数值，得出空间归属。
        /// </summary>
        /// <param name="sortedArray"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private static ulong ModifiedBinarySearch(ulong[] sortedArray, ulong val)
        {
            int min = 0;
            int max = sortedArray.Length - 1;

            if (val < sortedArray[min] || val > sortedArray[max])
                return sortedArray[0];

            while (max - min > 1)
            {
                int mid = (max + min) / 2;
                if (sortedArray[mid] >= val)
                {
                    max = mid;
                }
                else
                {
                    min = mid;
                }
            }

            return sortedArray[max];
        }
    }
}
