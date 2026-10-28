using System;
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

    [ObservableProperty] private string _address;

    [ObservableProperty] private string _port;

    [ObservableProperty] 
    private Host _host = new();

    [ObservableProperty] 
    private User _user = new();

    [ObservableProperty] 
    private string _statusMessage = "";

    private IConnection _connection;

    public LoginViewModel(INavigationService navigationService, IConnection connection)
    {
        _connection = connection;
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
    private async Task Connect()
    {
        Host.Address = Address;
        
        int number;
        
        if (int.TryParse(Port, out number))
        {
            Host.Port = number;
        }
        else
        {
            StatusMessage = "Error, please enter digits only";
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
    
}