using System;
using NNTP.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using NNTP.ViewModels;

namespace NNTP.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase _currentPage;
    
    private IConnection _connection;

    public MainViewModel(IConnection connection)
    {
        _connection = connection;
    }
}