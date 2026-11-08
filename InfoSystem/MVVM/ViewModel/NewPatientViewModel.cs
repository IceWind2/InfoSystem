using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace InfoSystem
{
    internal class NewPatientViewModel : ObservableObject
    {
        private string _fullName = "";
        private DateTime? _birthDate;
        private Sex _sex;
        private Location? _selectedLocation;
        private string _note = "";

        private string _fullNameError = "", _birthDateError = "", _sexError = "", _locationsError = "";
        private bool _submitted;

        public NewPatientViewModel(IEnumerable<Location> locations)
        {
            Locations = new List<Location>(locations);
            SaveCommand = new RelayCommand(o =>
            {
                _submitted = true;
                if (!Validate())
                {
                    return;
                }

                Result = new CreateUpdatePatientDTO()
                {
                    Name = FullName.Trim(),
                    BirthDate = BirthDate!.Value.Date,
                    Sex = Sex,
                    LocationId = SelectedLocation!.Id,
                    Note = string.IsNullOrWhiteSpace(Note) ? null : Note.Trim(),
                };

                CloseRequested?.Invoke(true);
            });
        }

        public IReadOnlyList<Location> Locations { get; }
        public ICommand SaveCommand { get; }
        public CreateUpdatePatientDTO? Result { get; private set; }
        public event Action<bool>? CloseRequested;

        public string FullName
        {
            get => _fullName;
            set { if (SetProperty(ref _fullName, value)) Revalidate(); }
        }
        public DateTime? BirthDate
        {
            get => _birthDate;
            set { if (SetProperty(ref _birthDate, value)) Revalidate(); }
        }
        public Sex Sex
        {
            get => _sex;
            set { if (SetProperty(ref _sex, value)) Revalidate(); }
        }
        public Location? SelectedLocation
        {
            get => _selectedLocation;
            set { if (SetProperty(ref _selectedLocation, value)) Revalidate(); }
        }
        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }

        public string FullNameError
        { 
            get => _fullNameError;
            private set => SetProperty(ref _fullNameError, value);
        }
        public string BirthDateError
        {
            get => _birthDateError;
            private set => SetProperty(ref _birthDateError, value);
        }
        public string SexError
        {
            get => _sexError;
            private set => SetProperty(ref _sexError, value);
        }
        public string LocationsError
        {
            get => _locationsError;
            private set => SetProperty(ref _locationsError, value);
        }

        private void Revalidate()
        {
            if (_submitted) Validate();
        }

        private bool Validate()
        {
            FullNameError = string.IsNullOrWhiteSpace(FullName) ? "Введите ФИО"
                          : FullName.Trim().Length > 200 ? "Слишком длинное ФИО"
                          : "";

            BirthDateError = BirthDate is null ? "Укажите дату рождения"
                           : BirthDate.Value.Date > DateTime.Today ? "Дата не может быть в будущем"
                           : BirthDate.Value.Year < 1900 ? "Проверьте год рождения"
                           : "";

            SexError = Sex != Sex.None ? "" : "Выберите пол";
            LocationsError = SelectedLocation is null ? "Выберите адрес" : "";

            return FullNameError.Length == 0 && BirthDateError.Length == 0
                && SexError.Length == 0 && LocationsError.Length == 0;
        }
    }
}
