using CommunityToolkit.Mvvm.ComponentModel;

namespace NNTP.Models;

public partial class Host : ObservableObject
{
    [ObservableProperty] 
    private string _address;

    [ObservableProperty] 
    private int _port;

    public Host(string address, int port)
    {
        _address = address;
        _port = port;
    }

    public Host()
    {
        
    }
}