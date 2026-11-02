using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NNTP.Models;
using NNTP.Services;

namespace NNTP.ViewModels;

/**
 * Handles all business logic behind the UI related to articles, individual groups and list of groups
 */
public partial class ArticleViewModel : ViewModelBase, INotifyCollectionChanged
{
    private INewsGroupService _ngs;
    private IConnection _connection;
    
    [ObservableProperty] public string nntpCommand;
    [ObservableProperty] public string statusMessagePrompt;
    [ObservableProperty] public string statusCode;
    [ObservableProperty] public NewsGroup selectedNewsGroup;
    [ObservableProperty] public string fromArticle;
    [ObservableProperty] public string newsgroupsArticle;
    [ObservableProperty] public string subjectArticle;
    [ObservableProperty] public string messageArticle;

    private Article _article;

    public ObservableCollection<Object> NewsGroupCollection { get; } = new();
    public ObservableCollection<NewsGroup> FavoriteCollection { get; } = new();
    

    public ArticleViewModel(INewsGroupService ngs, IConnection connection)
    {
        _ngs = ngs;
        _connection = connection;
        ReadFavorite(); //loads favorites on startup
    }

    /**
     * Adds the selected item from the listbox to the favorites list
     */
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
    
    /**
     * Removes the selected item from the favorite list and listbox
     */
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
    
    /**
     * Saves the favorite list to the harddrive as plaintext
     */
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

    /**
     * Reads the favorite list from the harddrive
     */
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

    /**
     * Passes NNTP commands to the service layer and calls their associated methods
     */
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
                case "list":
                    await _ngs.CallServerAsync(lowerCommand);
                    await GetNewsGroupList();
                    break;
                case "group":
                    await _ngs.CallServerAsync(lowerCommand);
                    await GetGroup();
                    break;
                case "article":
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
    
    /**
     * Handles adding the fetched newsgroup list to the listbox
     */
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
    
    /**
     * Handles adding the fetched group to the listbox
     */
    private async Task GetGroup()
    {
        StatusCode = "";
        NewsGroupCollection.Clear();
        var (status, group) = await _ngs.GetGroup();
        
        StatusCode = status;

        if (group != null)
        {
            NewsGroupCollection.Add(group);
        }
        
    }

    /**
     * Handles adding the fetched article to the listbox
     */
    private async Task GetArticle()
    {
        StatusCode = "";
        NewsGroupCollection.Clear();

        var (status, article) = await _ngs.FetchArticleAsync();
        
        StatusCode = status;
        
        NewsGroupCollection.Add(article);
    }

    /**
     * Fetches data from the textblocks in the UI and creates a corresponding article object and updates the status message
     */
    [RelayCommand]
    private async Task CreateArticle()
    {
        _article = new();

        _article.fromPoster = FromArticle;
        Console.WriteLine(_article.fromPoster + " from poster");
        _article.newsGroup = NewsgroupsArticle;
        Console.WriteLine(_article.newsGroup + " to newsgroup");
        _article.subject = SubjectArticle;
        Console.WriteLine(_article.subject + " article subject");
        _article.message = MessageArticle;
        Console.WriteLine(_article.message + " article message");

        StatusMessagePrompt = await _ngs.SendCreatedArticle(_article);

    }
    
    

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}