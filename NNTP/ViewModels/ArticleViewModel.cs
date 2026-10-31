using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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
    [ObservableProperty] public NewsGroup selectedNewsGroup;
    [ObservableProperty] public string fromArticle;
    [ObservableProperty] public string newsgroupsArticle;
    [ObservableProperty] public string subjectArticle;
    [ObservableProperty] public string messageArticle;

    public ObservableCollection<Object> NewsGroupCollection { get; } = new();
    public ObservableCollection<Article> ArticleCollection { get; } = new();
    public ObservableCollection<NewsGroup> FavoriteCollection { get; } = new();
    

    public ArticleViewModel(INewsGroupService ngs)
    {
        _ngs = ngs;
        ReadFavorite();
    }

    [RelayCommand]
    private void AddToFavorite()
    {
        if (SelectedNewsGroup != null)
        {
            FavoriteCollection.Add(SelectedNewsGroup);
            WriteFavorite();
        }
        else
        {
            StatusMessagePrompt = "Please select a newsgroup";
        }
        
    }
    
    [RelayCommand]
    private void RemoveFromFavorite()
    {
        if (SelectedNewsGroup != null)
        {
            FavoriteCollection.Remove(SelectedNewsGroup);

            try
            {
                WriteFavorite();
                StatusMessagePrompt = "Favorites successfully updated";
            }
            
            catch (Exception e)
            {
                StatusMessagePrompt = e.Message;
            }
        }
        else
        {
            StatusMessagePrompt = "Please select a newsgroup";
        }
        
    }
    
    private void WriteFavorite()
    {
        try
        {
            using (FileStream fileStream = new(@"C:\Users\Bruger\Desktop\FavoriteNewsGroup.txt", FileMode.Create))
            {
                
                using (StreamWriter saveWriter = new(fileStream)) 
                {

                    foreach (var x in FavoriteCollection)
                    {
                        saveWriter.WriteLine(x);
                    }
                }
            }

            StatusMessagePrompt = "Newsgroup updated";
        }
        catch (Exception e)
        {
            StatusMessagePrompt = e.Message;
        }
    }

    private void ReadFavorite()
    {
        try
        {
            string path = @"C:\Users\Bruger\Desktop\FavoriteNewsGroup.txt";

            if (File.Exists(path))
            {
                string[] readNewsGroup = File.ReadAllLines(path);

                foreach (var x in readNewsGroup)
                {
                    NewsGroup newsGroup = new();
                    string[] delimiter = x.Split(" ");
                    newsGroup.name = delimiter[0];
                    newsGroup.lastArticleNo = delimiter[1];
                    newsGroup.firstArticleNo = delimiter[2]; 
                    newsGroup.postingStatus = delimiter[3];
                    FavoriteCollection.Add(newsGroup);
                }

                if (readNewsGroup.Length > 0)
                {
                    StatusMessagePrompt = "Favorites successfully loaded";
                }
            }
            
        }
        catch (Exception e)
        {
            StatusMessagePrompt = e.Message;
        }
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
        StatusCode = "";
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
        StatusCode = "";
        NewsGroupCollection.Clear();
        var (status, group) = await _ngs.GetGroupList();
        
        StatusCode = status;
        
        NewsGroupCollection.Add(group);
        
    }

    private async Task GetArticle()
    {
        StatusCode = "";
        NewsGroupCollection.Clear();

        var (status, article) = await _ngs.FetchArticleAsync();
        
        StatusCode = status;
        
        NewsGroupCollection.Add(article);
    }

    [RelayCommand]
    private async Task CreateArticle()
    {
        Article article = new();

        article.fromPoster = FromArticle;
        Console.WriteLine(article.fromPoster + " from poster");
        article.newsGroup = NewsgroupsArticle;
        Console.WriteLine(article.newsGroup + " to newsgroup");
        article.subject = SubjectArticle;
        Console.WriteLine(article.subject + " article subject");
        article.message = MessageArticle;
        Console.WriteLine(article.message + " article message");
        
        StatusMessagePrompt = await _ngs.SendCreatedArticle(article);
    }
    
    

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}