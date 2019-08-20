using System;
using System.Collections.Generic;
using System.Text;

namespace SBRB_DatabaseSeeder
{
    static class Configuration
    {
        const string _configFilePath = "config.json";
        public static bool Silent_logger { get; private set; }
        public static bool SkipWarnings { get; private set; }


        static Configuration()
        {
            // find and deserialize config file
            // Not found? generate one.
        }
    }
}
