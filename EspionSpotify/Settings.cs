using System.ComponentModel;
using System.Configuration;

namespace EspionSpotify.Properties
{
    // Cette classe vous permet de gérer des événements spécifiques dans la classe de paramètres :
    //  L'événement SettingChanging est déclenché avant la modification d'une valeur de paramètre.
    //  L'événement PropertyChanged est déclenché après la modification d'une valeur de paramètre.
    //  L'événement SettingsLoaded est déclenché après le chargement des valeurs de paramètre.
    //  L'événement SettingsSaving est déclenché avant l'enregistrement des valeurs de paramètre.
    internal sealed partial class Settings
    {
        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
            // Ajouter du code pour gérer l'événement SettingChangingEvent ici.
        }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
            // Ajouter du code pour gérer l'événement SettingsSaving ici.
        }
    }
}