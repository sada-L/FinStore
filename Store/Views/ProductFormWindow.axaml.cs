using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Store.ViewModels;

namespace Store.Views;

public partial class ProductFormWindow : ReactiveWindow<ProductFormWindowViewModel>
{
    public ProductFormWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.OkCommand.Subscribe(Close)));
    }
}