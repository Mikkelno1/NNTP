using System;
using Microsoft.Extensions.DependencyInjection;
using NNTP.ViewModels;

namespace NNTP.Services;

/**
 * Implements INavigationService and updates MainViewModel.CurrentPage via dependency injection
 */
public class NavigationService : INavigationService
{
    //Encapsulated reference to the singleton MainViewModel
    private readonly MainViewModel _mainViewModel;
    //Encapsulated reference to the DI-container
    private readonly IServiceProvider _serviceProvider;
    
    /**
     * Injects MainViewModel and IServiceProvider to the constructor
     */
    public NavigationService(MainViewModel mainViewModel, IServiceProvider serviceProvider)
    {
        _mainViewModel = mainViewModel;
        _serviceProvider = serviceProvider;
    }

    /**
     * Gets a new instance of the requested ViewModel from the DI-container if it inherits from ViewModelBase
     * Sets the requested View as the CurrentPage
     */
    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewModel = _serviceProvider.GetService<TViewModel>();
        _mainViewModel.CurrentPage = viewModel;
    }
}