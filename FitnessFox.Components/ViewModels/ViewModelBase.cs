using FitnessFox.Components.Services;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.ViewModels
{
    public class ViewModelBase
    {
        protected readonly IDialogService dialogService;
        private readonly ILoggingService loggingService;
        private readonly ILoadingService loadingService;

        public bool IsLoading
        {
            get => loadingService.IsLoading; 
            set => loadingService.IsLoading = value;
        }
        public Action StateHasChanged { get; set; } = () => { };

        public ViewModelBase(
            IDialogService mudDialogInstance,
            ILoggingService loggingService,
            ILoadingService loadingService)
        {
            this.dialogService = mudDialogInstance;
            this.loggingService = loggingService;
            this.loadingService = loadingService;
        }

        public async Task Load(Func<Task> action)
        {
            try
            {
                IsLoading = true;
                StateHasChanged();
                await action();
            }
            catch (Exception ex)
            {
                loggingService.Error(ex);
                await dialogService.ShowMessageBox("Error", ex.ToString());
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        public virtual Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
