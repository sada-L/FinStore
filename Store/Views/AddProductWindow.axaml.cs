using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Store.ViewModels;

namespace Store.Views;

public partial class AddProductWindow : ReactiveWindow<AddProductWindowViewModel>
{
    public AddProductWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.OkCommand.Subscribe(Close)));
    }
}