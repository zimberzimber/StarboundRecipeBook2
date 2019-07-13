using Jil;
using StarboundRecipeBook2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SBRB_DatabaseSeeder.DeserializedData
{
    class DeserializedRecipe
    {
        public DeserializedInputOutput[] input;
        public DeserializedInputOutput output;
        public string[] groups;
        public string filePath;
    }

    class DeserializedInputOutput
    {
        string _item; // Just the data holder, explanation further down...
        public int count;

        // For whatever reason, Starbounds recipes accept "name" and "item" in the input/output fields...
        // And despite "item" being used way more often than "name", the game prioritizes the latter
        public string name { set { _item = value; } }

        // Redirect the de/serializer here. Serialization will occur normally
        // Deserialization would prefer setting _item to name as per the 'set' in here
        // It is only relevant if a JSON has both 'item', and 'name, but 'name' was read before 'item'
        // So it would follow SBs name preference rules
        public string item
        {
            get { return _item; }
            set
            {
                if (_item == default)
                    _item = value;
            }
        }
    }
}
