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

    public ObservableCollection<Object> NewsGroupCollection { get; } = new();
    public ObservableCollection<Article> ArticleCollection { get; } = new();

    public ArticleViewModel(INewsGroupService ngs)
    {
        _ngs = ngs;
    }

    [RelayCommand]
    private async Task Prompt()
    {
        var lowerCommand = NntpCommand.Trim().ToLower();
        NntpCommand = "";
        StatusMessagePrompt = "";
        string[] command = lowerCommand.Split(" ");
        try
        {
            switch (command[0])
            {
                case "list" :
                    await _ngs.CallServerAsync(lowerCommand);
                    await GetNewsGroupList();
                    break;
                case "group" : 
                    await _ngs.CallServerAsync(lowerCommand);
                    await GetGroup();
                    break;
                case "article" :
                    await _ngs.CallServerAsync(lowerCommand);
                    await GetArticle();
                    break;
            }
        }
        catch (Exception e)
        {
            StatusMessagePrompt = e.Message;
        }
    }
    
    private async Task GetNewsGroupList()
    {
        NewsGroupCollection.Clear();
        var (status, list) = await _ngs.FetchNewsGroupListAsync();

        StatusCode = status;
        
        foreach (var x in list)
        {
            NewsGroupCollection.Add(x);
        }
        
    }
    
    private async Task GetGroup()
    {
        NewsGroupCollection.Clear();
        var (status, group) = await _ngs.GetGroupList();
        
        StatusCode = status;
        
        NewsGroupCollection.Add(group);
        
    }

    private async Task GetArticle()
    {
        ArticleCollection.Clear();

        var article = await _ngs.FetchArticleAsync();
        
        ArticleCollection.Add(article);
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}