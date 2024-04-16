using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace Czogala4fKolekcje
{
    public partial class App : Application
    {
        public static MainPage MainPageInstance { get; private set; }

        public App()
        {
            InitializeComponent();

#if DEBUG
            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            System.Diagnostics.Debug.WriteLine("Sciezka do danych aplikacji: " + appDataFolderPath);
            string collectionsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");
            System.Diagnostics.Debug.WriteLine("Sciezka do pliku z kolekcjami: " + collectionsFilePath);
#endif

            MainPage = new MainPage();
            MainPageInstance = MainPage as MainPage;
        }
    }
}
