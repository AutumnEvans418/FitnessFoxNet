
namespace FitnessFox.Components.Services
{
    public interface ILoadingService
    {
        bool IsLoading { get; set; }

        event Action OnChange;
    }
}