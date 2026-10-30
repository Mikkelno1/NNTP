using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NNTP.Models;
using NNTP.Services;
using System.Linq;

namespace NNTP.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly User _validUser;

    private readonly INavigationService _navigationService;

    private readonly IAuthService _authService;

    [ObservableProperty] private string _portString;

    [ObservableProperty] 
    private Host _host = new();

    [ObservableProperty] 
    private User _user = new();

    [ObservableProperty] 
    private string _statusMessage = "";

    private IConnection _connection;

    public LoginViewModel(INavigationService navigationService, IConnection connection, IAuthService authService)
    {
        _connection = connection;
        _navigationService = navigationService;
        _authService = authService;
        
        Console.WriteLine("username" + User.Username);
        Console.WriteLine("password" + User.Password);
        User.PropertyChanged += (sender, args) => NavigateCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private async Task Navigate()
    {
        if (CanLogin())
        {
            if (await _authService.AuthenticateUserName(User))
            {
                if (await _authService.AuthenticatePassword(User))
                {
                    _navigationService.NavigateTo<ArticleViewModel>();
                }
                else
                {
                    StatusMessage = "Invalid credentials, try again";
                }
            }
        }
    }

    /**
    * Returns true if textblock data matches hardcoded testcase
    */
    private bool CanLogin() => !string.IsNullOrEmpty(User.Username) && !string.IsNullOrEmpty(User.Password);

    [RelayCommand]
    private async Task Connect()
    {
        if (Int32.TryParse(PortString, out int number))
        {
            Host.Port = number;
        }
        else
        {
            StatusMessage = "Invalid characters, only digits allowed";
            return;
        }
        
        try
        {
            await _connection.Connect(Host);

            StatusMessage = "Connected to: " + Host.Address + " \nThrough port: " + Host.Port;
        }
        catch (Exception e)
        {
            StatusMessage = e.Message;
        }
        
    }

    [RelayCommand]
    private void EncryptInformation()
    {
        StatusMessage = _authService.EncryptInfo(User.Username, User.Password);
    }

    
    [RelayCommand]
    private void DecryptInformation()
    {
        List<string> list = _authService.DecryptInfo();

        
        User.Username = list[0];
        User.Password = list[1];
        StatusMessage = list[2];
        
    }

    [RelayCommand]
    private void SaveServer()
    {
        string serverPort = Host.Port.ToString();
        StatusMessage = _connection.SaveServer(Host.Address, serverPort);
    }

    [RelayCommand]
    private void LoadServer()
    {
        List<string> list = _connection.LoadServer();

        Host.Address = list[0];

        if (Int32.TryParse(list[1], out int parsed))
        {
            Host.Port = parsed;
            PortString = list[1];
        }
        StatusMessage = list[2];
    }
   
    
}