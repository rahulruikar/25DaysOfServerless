using Day5.Models;
using System.Threading.Tasks;

namespace Day5.Interfaces
{
    public interface ISentimentAnalysisService
    {
        string PerformSentimentAnalysis(TextTranslationResult input);
    }
}
