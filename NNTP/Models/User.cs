using CommunityToolkit.Mvvm.ComponentModel;

namespace NNTP.Models;

public partial class User : ObservableObject
{
    [ObservableProperty] 
    private string _username;

    [ObservableProperty] 
    private string _password;

}