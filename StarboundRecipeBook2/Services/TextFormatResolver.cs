using StarboundRecipeBook2.Helpers;

namespace StarboundRecipeBook2.Services
{
    // NOTE:
    // The sole purpose of this service is to be inject into views.
    // The extensions this service is utilizing are to be used directly otherwise.

    public interface ITextColorResolver
    {
        /// <summary>Convert a string with color formatting into an HTML element with the applied colors.</summary>
        /// <param name="raw">The raw string to turn into an HTML element</param>
        /// <returns></returns>
        string ResolveColor(string raw);

        /// <summary>Remove all color formatting frfom the given string.</summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        string RemoveFormatting(string raw);
    }

    public class TextFormatResolver : ITextColorResolver
    {
        public string ResolveColor(string raw)
            => raw.ResolveColor();

        public string RemoveFormatting(string raw)
            => raw.RemoveFormatting();
    }
}
