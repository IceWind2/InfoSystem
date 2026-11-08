using System;
using System.Windows.Input;

namespace InfoSystem
{
    internal class NewLocationViewModel : ObservableObject
    {
        private string _name = "";
        private string _nameError = "";
        private bool _submitted;
        private readonly Location? _location;

        public NewLocationViewModel(Location? location = null)
        {
            _location = location;
            _name = location?.Name ?? "";
            Title = location == null ? "Новый адрес" : "Изменить адрес";

            SaveCommand = new RelayCommand(_ =>
            {
                _submitted = true;
                if (!Validate())
                {
                    return;
                }

                if (_location == null)
                {
                    Result = new Location
                    {
                        Name = Name.Trim()
                    };
                }
                else
                {
                    _location.Name = Name.Trim();
                }

                CloseRequested?.Invoke(true);
            });
        }

        public string Title { get; }
        public ICommand SaveCommand { get; }
        public Location? Result { get; private set; }
        public event Action<bool>? CloseRequested;

        public string Name
        {
            get => _name;
            set { if (SetProperty(ref _name, value)) Revalidate(); }
        }

        public string NameError
        {
            get => _nameError;
            private set => SetProperty(ref _nameError, value);
        }

        private void Revalidate()
        {
            if (_submitted)
            {
                Validate();
            }
        }

        private bool Validate()
        {
            NameError = string.IsNullOrWhiteSpace(Name) ? "Введите название адреса" : "";
            return NameError.Length == 0;
        }
    }
}
