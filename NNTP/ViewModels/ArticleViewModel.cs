using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NNTP.Models;
using NNTP.Services;

namespace NNTP.ViewModels;

public partial class ArticleViewModel : ViewModelBase, INotifyCollectionChanged
{
    private INewsGroupService _ngs;
    
    [ObservableProperty] public string nntpCommand;
    [ObservableProperty] public string statusMessagePrompt;
    [ObservableProperty] public string statusCode;

    public ObservableCollection<NewsGroup> NewsGroups { get; } = new();

    public ArticleViewModel(INewsGroupService ngs)
    {
        _ngs = ngs;
    }

    [RelayCommand]
    private async Task Prompt()
    {
        var lowerCommand = NntpCommand.ToLower();
        try
        {
           await _ngs.CallServer(lowerCommand);
           NntpCommand = "";
           await GetNewsList();
        }
        catch (Exception e)
        {
            StatusMessagePrompt = e.Message;
        }
    }
    
    private async Task GetNewsList()
    {
        NewsGroups.Clear();
        var (status, list) = _ngs.ReturnNewsGroupList();

        StatusCode = status;
        
        foreach (var x in list)
        {
            NewsGroups.Add(x);
        }
        
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}