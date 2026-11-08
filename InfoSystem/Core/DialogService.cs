using System;
using System.Diagnostics;
using System.Windows;

namespace InfoSystem
{
    internal interface IDialogService
    {
        public bool? ShowNewPatient(Window mainWindow, NewPatientViewModel vm);
        public bool? ShowNewMedicine(Window mainWindow, NewMedicineViewModel vm);
    }

    internal class DialogService : IDialogService
    {
        public bool? ShowNewPatient(Window mainWindow, NewPatientViewModel vm)
        {
            var modal = new NewPatientModal(mainWindow);
            modal.DataContext = vm;

            var ownerOpacity = mainWindow.Opacity;
            mainWindow.Opacity = 0.5;
            try
            {
                return modal.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                mainWindow.Opacity = ownerOpacity;
            }
        }

        public bool? ShowNewMedicine(Window mainWindow, NewMedicineViewModel vm)
        {
            var modal = new NewMedicineModal(mainWindow);
            modal.DataContext = vm;

            var ownerOpacity = mainWindow.Opacity;
            mainWindow.Opacity = 0.5;
            try
            {
                return modal.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                mainWindow.Opacity = ownerOpacity;
            }
        }
    }
}
