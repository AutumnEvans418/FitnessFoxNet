
namespace FitnessFox.Components.Services
{
    public interface IGoogleSheetsServices
    {
        Task AddWorksheets(string[] sheetNames);
        Task<List<List<string>>> GetSheetRows(string name);
        Task UpdateSheet(string range, IList<IList<object>> rows);
    }
}