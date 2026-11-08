using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace InfoSystem
{
    internal class MainViewModel : ObservableObject
    {
        public sealed record NavItem(string Title, string Icon, Func<object> Create);

        private readonly Dictionary<NavItem, object> _pages = new();

        private NavItem? _selectedNavItem;
        private object? _currentViewModel;

        public ObservableCollection<NavItem> NavItems { get; }

        public NavItem? SelectedNavItem
        {
            get => _selectedNavItem;
            set
            {
                _selectedNavItem = value;
                OnPropertyChanged();
                ShowPage(value);
            }
        }

        public object? CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(Window mainWindow)
        {
            NavItems =
            [
                new("Пациенты",  "\uE77B", () => new PatientsViewModel(mainWindow)),
                new("Лекарства", "\uE8F1", () => new MedicineViewModel(mainWindow)),
                new("Диагнозы",  "\uE9D9", () => new DiagnosisViewModel(mainWindow)),
                new("Адреса",    "\uE80F", () => new LocationsViewModel(mainWindow)),
                new("Архив",     "\uE7B8", () => new ArchiveViewModel(mainWindow)),
            ];

            SelectedNavItem = NavItems[0];
        }

        private void ShowPage(NavItem? item)
        {
            if (item is null) return;

            if (!_pages.TryGetValue(item, out var page))
                _pages[item] = page = item.Create();

            CurrentViewModel = page;
        }
    }
}
