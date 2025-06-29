﻿// FILE: PowerBIViewer.App/ViewModels/ViewModelBase.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PowerBIViewer.App.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}