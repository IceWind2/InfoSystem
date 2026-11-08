using System;
using System.Windows.Input;

namespace InfoSystem
{
    internal class NewDiagnosisViewModel : ObservableObject
    {
        private string _name = "";
        private string _nameError = "";
        private bool _submitted;
        private readonly Diagnosis? _diagnosis;

        public NewDiagnosisViewModel(Diagnosis? diagnosis = null)
        {
            _diagnosis = diagnosis;
            _name = diagnosis?.Name ?? "";
            Title = diagnosis == null ? "Новый диагноз" : "Изменить диагноз";

            SaveCommand = new RelayCommand(_ =>
            {
                _submitted = true;
                if (!Validate())
                {
                    return;
                }

                if (_diagnosis == null)
                {
                    Result = new Diagnosis
                    {
                        Name = Name.Trim()
                    };
                }
                else
                {
                    diagnosis!.Name = Name.Trim();
                }

                CloseRequested?.Invoke(true);
            });
        }

        public string Title { get; }
        public ICommand SaveCommand { get; }
        public Diagnosis? Result { get; private set; }
        public event Action<bool>? CloseRequested;

        public string Name
        {
            get => _name;
            set { if (SetProperty(ref _name, value)) Revalidate();  }
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
            NameError = string.IsNullOrWhiteSpace(Name) ? "Введите название диагноза" : "";
            return NameError.Length == 0;
        }
    }
}
