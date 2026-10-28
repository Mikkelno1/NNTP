using NNTP.ViewModels;

namespace NNTP.Services;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
}