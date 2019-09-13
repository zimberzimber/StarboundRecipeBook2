using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SBRB.Models
{
    public class CompositePatchId : ModDependant
    {
        public string FilePath;
        public string KeyName;
    }

    public class PatchBase
    {
        [BsonId]
        public CompositePatchId ID;

    }

    public class PatchString : PatchBase
    {
        public string Value;
    }

    public class PatchNumber : PatchBase
    {
        public double Value;
    }

    public class PatchArray : PatchBase
    {
        public object[] Value;
    }

    public class PatchObject : PatchBase
    {
        public object Value;
    }
}
