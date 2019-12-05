using Day5.Models;
using System.Threading.Tasks;

namespace Day5.Interfaces
{
    public interface ITextTranslationService
    {
        Task<TextTranslationResult> TranslateText(string inputText, string language);
    }
}
