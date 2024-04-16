using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Czogala4fKolekcje
{
    public partial class MainPage : ContentPage
    {
        public List<Collection> collections;

        public MainPage()
        {
            InitializeComponent();
            LoadCollections();
        }

        public void LoadCollections()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");
            if (File.Exists(filePath))
            {
                var collectionDictionary = new Dictionary<string, List<string>>();
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    var collectionName = parts[0];
                    var item = parts[1];
                    if (collectionDictionary.ContainsKey(collectionName))
                    {
                        collectionDictionary[collectionName].Add(item);
                    }
                    else
                    {
                        collectionDictionary.Add(collectionName, new List<string> { item });
                    }
                }

                collections = collectionDictionary.Select(c => new Collection(c.Key, c.Value)).ToList();
            }
            else
            {
                collections = new List<Collection>();
            }
            CollectionsListView.ItemsSource = collections;
        }

        public void SaveCollections()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");
            File.WriteAllLines(filePath, collections.SelectMany(c => c.Items.Select(item => $"{c.Name}|{item}")));
        }

        public async void OnAddCollectionClicked(object sender, EventArgs e)
        {
            string newCollectionName = await DisplayPromptAsync("Dodaj nową kolekcję", "Podaj nazwę kolekcji:");
            if (string.IsNullOrWhiteSpace(newCollectionName))
            {
                await DisplayAlert("Błąd", "Nazwa kolekcji nie może być pusta.", "OK");
                return;
            }

            string items = await DisplayPromptAsync("Nowa kolekcja", "Dodawaj elementy oddzielając je przecinkami:");
            if (string.IsNullOrWhiteSpace(items))
            {
                await DisplayAlert("Błąd", "Elementy kolekcji nie mogą być puste.", "OK");
                return;
            }

            if (items.Trim(',').Any(char.IsDigit))
            {
                await DisplayAlert("Błąd", "Elementy kolekcji nie mogą składać się tylko z cyfr.", "OK");
                return;
            }

            string normalizedItems = string.Join(",", items.Split(',').Where(s => !string.IsNullOrWhiteSpace(s.Trim())));

            collections.Add(new Collection(newCollectionName, normalizedItems.Split(',').Select(s => s.Trim()).ToList()));
            SaveCollections();

            RefreshCollections();
        }


        public void RefreshCollections()
        {
            CollectionsListView.ItemsSource = null;
            CollectionsListView.ItemsSource = collections;
        }

        public async void OnCollectionSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedCollection = (Collection)e.SelectedItem;

            await Navigation.PushModalAsync(new CollectionDetailsPage(selectedCollection));

            CollectionsListView.SelectedItem = null;
        }

        public async void OnCollectionTapped(object sender, EventArgs e)
        {
            var tappedItem = (TextCell)sender;
            var selectedCollection = collections.FirstOrDefault(c => c.Name == tappedItem.Text);
            if (selectedCollection != null)
            {
                await Navigation.PushModalAsync(new CollectionDetailsPage(selectedCollection));
            }
        }
        

    }

    public class Collection
    {
        public string Name { get; set; }
        public List<string> Items { get; set; }

        public Collection(string name, List<string> items)
        {
            Name = name;
            Items = items;
        }

        public override string ToString()
        {
            return $"{Name}|{string.Join(",", Items)}";
        }
    }
}
