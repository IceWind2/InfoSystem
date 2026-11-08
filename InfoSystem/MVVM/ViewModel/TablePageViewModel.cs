namespace InfoSystem
{
    class TablePageViewModel : ObservableObject
    {
        public string SearchText { get; set; } = "";
        public int TotalCount { get; protected set; }

        public RelayCommand FilterCommand { get; set; } = null!;
        public RelayCommand RefreshCommand { get; set; } = null!;
        public AsyncRelayCommand AddCommand { get; set; } = null!;
        public AsyncRelayCommand EditCommand { get; set; } = null!;
        public AsyncRelayCommand RemoveCommand { get; set; } = null!;

        public bool CanAdd { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        public bool CanDelete { get; set; } = true;
    }
}
