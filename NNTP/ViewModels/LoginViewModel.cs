using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NNTP.Models;
using NNTP.Services;

namespace NNTP.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly User _validUser;

    private readonly INavigationService _navigationService;

    [ObservableProperty] 
    private Host _host = new();

    [ObservableProperty] 
    private User _user = new();

    private IConnection _connection = new Connection();

    public LoginViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        _validUser = new()
        {
            Username = "miknor",
            Password = "1234"
        };
        
        
        _user.PropertyChanged += (sender, args) => NavigateCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private void Navigate()
    {
        _navigationService.NavigateTo<ArticleViewModel>();
    }
    
    /**
    * Returns true if textblock data matches hardcoded testcase
    */
    private bool CanLogin()
    {
        return User.Username == _validUser.Username && User.Password == _validUser.Password;
    }

    [RelayCommand]
    private void Connect()
    {
        Console.WriteLine(Host.Address);
        _connection.Connect(Host);
    }
    
}