using CommunityToolkit.Mvvm.ComponentModel;

namespace NNTP.Models;

/**
 * Defines the User object used to establish connections
 */
public partial class User : ObservableObject
{
    [ObservableProperty] 
    private string _username;

    [ObservableProperty] 
    private string _password;

}