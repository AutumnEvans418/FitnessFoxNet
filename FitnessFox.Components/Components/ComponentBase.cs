using FitnessFox.Components.Components.Layout;
using FitnessFox.Components.Services;
using FitnessFox.Components.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Components.Components
{
    public class ViewModelComponentBase<TVm> : ComponentBase where TVm : ViewModelBase
    {
        [Inject]
        public TVm Vm { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Vm.StateHasChanged = StateHasChanged;
            await Vm.OnInitializedAsync();
        }

    }
}
