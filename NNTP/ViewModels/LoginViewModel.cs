using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NNTP.Models;
using NNTP.Services;


namespace NNTP.ViewModels;

/**
 * Handles all business logic behind the UI related to initial server connectivity
 */
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
        User.PropertyChanged += (sender, args) => NavigateCommand.NotifyCanExecuteChanged(); //notifies once the user object has changed 
    }

    /**
     * Handles login chain
     */
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
    * Checks if textblocks contains valid data
    */
    private bool CanLogin() => !string.IsNullOrEmpty(User.Username) && !string.IsNullOrEmpty(User.Password);

    /**
     * Handles connection chain
     */
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

    /**
     * Displays to the user that their credentials have been encrypted
     */
    [RelayCommand]
    private void EncryptInformation()
    {
        StatusMessage = _authService.EncryptInfo(User.Username, User.Password);
    }

    /**
     * Displays to the user that their credentials have been decrypted
     */
    [RelayCommand]
    private void DecryptInformation()
    {
        List<string> list = _authService.DecryptInfo();

        
        User.Username = list[0];
        User.Password = list[1];
        StatusMessage = list[2];
        
    }

    /**
     * Displays to the user that their server has been saved to disk
     */
    [RelayCommand]
    private void SaveServer()
    {
        string serverPort = Host.Port.ToString();
        StatusMessage = _connection.SaveServer(Host.Address, serverPort);
    }

    /**
     * Displays to the user that their server has been loaded from disk
     */
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