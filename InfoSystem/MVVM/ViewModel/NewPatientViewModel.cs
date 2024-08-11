using System.Collections.ObjectModel;

namespace InfoSystem
{
    internal class NewPatientViewModel : ObservableObject
    {
        public ObservableCollection<Medicine> Medicine { get; set; }
        public Medicine SelectedMedicine { get; set; }

        public NewPatientViewModel()
        {
            Medicine =
            [
                new Medicine
                {
                    Id = 1,
                    Name = "rwer"
                },
                new Medicine
                {
                    Id = 2,
                    Name = "jeiwhkf"
                }
            ];
        }
    }
}
