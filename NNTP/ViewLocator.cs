using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using NNTP.ViewModels;
using NNTP.Views;

namespace NNTP;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    
    private readonly IServiceProvider _services;

    public ViewLocator(IServiceProvider services)
    {
        _services = services;
    }
    
    
    public Control? Build(object? param)
    {
        //Pattern matching switch - checks which ViewModel is being passed to build
        //Creates the corresponding view
        return param switch
        {
            LoginViewModel => new LoginView(),
            ArticleViewModel => new ArticleView(),
            _ => new TextBlock { Text = $"No view for {param.GetType().Name}" }
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}