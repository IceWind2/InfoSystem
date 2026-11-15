using System.ComponentModel;
using System.Linq;

namespace InfoSystem
{
    internal abstract class TablePageViewModel : ObservableObject
    {
        private string _searchText = "";
        private bool _isFilterMenuOpen;

        protected ICollectionView _collectionView;

        protected TablePageViewModel()
        {
            ClearSearchCommand = new RelayCommand(_ =>
            {
                SearchText = "";
                SearchCommand.Execute(null);
            });
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public int TotalCount { get; protected set; }

        public RelayCommand SearchCommand { get; set; } = null!;
        public RelayCommand ClearSearchCommand { get; }
        public AsyncRelayCommand AddCommand { get; set; } = null!;
        public AsyncRelayCommand EditCommand { get; set; } = null!;
        public AsyncRelayCommand RemoveCommand { get; set; } = null!;

        public bool CanAdd { get; protected set; } = true;
        public bool CanEdit { get; protected set; } = true;
        public bool CanDelete { get; protected set; } = true;
        public bool HasAdvancedFilters { get; protected set; }

        public bool IsFilterMenuOpen
        {
            get => _isFilterMenuOpen;
            set => SetProperty(ref _isFilterMenuOpen, value);
        }

        public abstract void ApplySort();

        protected void RefreshCollectionView()
        {
            _collectionView.Refresh();
            TotalCount = _collectionView.Cast<object>().Count();
            OnPropertyChanged(nameof(TotalCount));
        }
    }
}
