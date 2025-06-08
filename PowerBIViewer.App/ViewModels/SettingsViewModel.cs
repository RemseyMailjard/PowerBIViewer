// FILE: ViewModels/SettingsViewModel.cs
using PowerBIViewer.App.Commands;
using PowerBIViewer.App.Models;
using PowerBIViewer.App.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PowerBIViewer.App.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IWidgetRepository _widgetRepository;

        // --- Properties voor DataBinding ---
        public ObservableCollection<ReportDefinition> Reports { get; set; }
        public ObservableCollection<Widget> Widgets { get; set; }

        private ReportDefinition? _selectedReport;
        public ReportDefinition? SelectedReport
        {
            get => _selectedReport;
            set { _selectedReport = value; OnPropertyChanged(); (RemoveReportCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        private Widget? _selectedWidget;
        public Widget? SelectedWidget
        {
            get => _selectedWidget;
            set { _selectedWidget = value; OnPropertyChanged(); (RemoveWidgetCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
        }

        // --- Commands ---
        public ICommand AddReportCommand { get; }
        public ICommand RemoveReportCommand { get; }
        public ICommand AddWidgetCommand { get; }
        public ICommand RemoveWidgetCommand { get; }
        public ICommand SaveCommand { get; }

        // Event om aan te geven dat de save is voltooid.
        public event System.Action? OnSaveComplete;

        public SettingsViewModel(IReportRepository reportRepository, IWidgetRepository widgetRepository)
        {
            _reportRepository = reportRepository;
            _widgetRepository = widgetRepository;

            Reports = new ObservableCollection<ReportDefinition>(_reportRepository.GetAll());
            Widgets = new ObservableCollection<Widget>(_widgetRepository.LoadWidgets());

            AddReportCommand = new RelayCommand(ExecuteAddReport);
            RemoveReportCommand = new RelayCommand(ExecuteRemoveReport, CanExecuteRemoveReport);
            AddWidgetCommand = new RelayCommand(ExecuteAddWidget);
            RemoveWidgetCommand = new RelayCommand(ExecuteRemoveWidget, CanExecuteRemoveWidget);
            SaveCommand = new RelayCommand(ExecuteSave);
        }

        private void ExecuteAddReport(object? p)
        {
            var newReport = new ReportDefinition { Name = "Nieuw Rapport", Key="new_key", Emoji="📝", Url="https://..." };
            Reports.Add(newReport);
            SelectedReport = newReport;
        }

        private void ExecuteRemoveReport(object? p)
        {
            if (SelectedReport != null) Reports.Remove(SelectedReport);
        }
        private bool CanExecuteRemoveReport(object? p) => SelectedReport != null;

        private void ExecuteAddWidget(object? p)
        {
            var newWidget = new Widget { Title = "Nieuwe Widget", Icon="💡", Url="https://..." };
            Widgets.Add(newWidget);
            SelectedWidget = newWidget;
        }

        private void ExecuteRemoveWidget(object? p)
        {
            if (SelectedWidget != null) Widgets.Remove(SelectedWidget);
        }
        private bool CanExecuteRemoveWidget(object? p) => SelectedWidget != null;

        private void ExecuteSave(object? p)
        {
            _reportRepository.SaveReports(Reports.ToList());
            _widgetRepository.SaveWidgets(Widgets.ToList());
            OnSaveComplete?.Invoke();
        }
    }
}