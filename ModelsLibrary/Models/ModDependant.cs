namespace SBRB.Models
{
    /// <summary>
    /// Class indication the piece of data is tied to a mod ID.
    /// All entries added from a mod must have their ID be composed out of a mods ID and another unique identified.
    /// </summary>
    public class ModDependant
    {
        public uint SourceModId;
    }
}
