﻿using SnowflakeID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject
{
    public static class SnowflakeGenerator
    {
        private static readonly DateTime Epoch = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        private static readonly Dictionary<int, SnowflakeIDGenerator> Generators = new();

        private static readonly object Lock = new();

        public static string Generate(int workerId = 0)
        {
            workerId &= 0x1F; // 5 bits
            lock (Lock)
            {
                if (Generators.TryGetValue(workerId, out SnowflakeIDGenerator? value))
                {
                    return value.GetCode().ToString();
                }

                value = new SnowflakeIDGenerator(workerId, Epoch);
                Generators.Add(workerId, value);

                return value.GetCode().ToString();
            }
        }
    }
}
