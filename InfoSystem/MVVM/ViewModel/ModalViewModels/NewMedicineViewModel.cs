using System;
using System.Windows.Input;

namespace InfoSystem
{
    internal class NewMedicineViewModel : ObservableObject
    {
        private string _name = "";

        private string _nameError = "";
        private bool _submitted;
        private bool _edit;

        public NewMedicineViewModel(Medicine? medicine = null)
        {
            if (medicine != null)
            {
                _edit = true;
                _name = medicine.Name;
            }

            Title = _edit ? "Изменить лекарство" : "Новое лекарство";

            SaveCommand = new RelayCommand(_ =>
            {
                _submitted = true;
                if (!Validate())
                {
                    return;
                }

                if (_edit)
                {
                    medicine!.Name = Name.Trim();
                }
                else
                {
                    Result = new Medicine
                    {
                        Name = Name.Trim()
                    };
                }

                CloseRequested?.Invoke(true);
            });
        }

        public string Title { get; }
        public ICommand SaveCommand { get; }
        public Medicine? Result { get; private set; }
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
            NameError = string.IsNullOrWhiteSpace(Name) ? "Введите название лекарства" : "";
            return NameError.Length == 0;
        }
    }
}
