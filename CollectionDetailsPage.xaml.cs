using System;
using Microsoft.Maui.Controls;

namespace Czogala4fKolekcje
{
    public partial class CollectionDetailsPage : ContentPage
    {
        private Collection collection;

        public CollectionDetailsPage(Collection collection)
        {
            InitializeComponent();
            this.collection = collection;
            BindingContext = collection;
            InitializeItems();
        }

        private void InitializeItems()
        {
            foreach (var item in collection.Items)
            {
                var label = new Label { Text = item, FontSize = 18 };
                var editButton = new Button { Text = "Edytuj" };
                editButton.Clicked += (sender, e) => OnEditItemClicked(item);
                var deleteButton = new Button { Text = "Usun" };
                deleteButton.Clicked += (sender, e) => OnDeleteItemClicked(item);

                var itemLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
                itemLayout.Children.Add(editButton);
                itemLayout.Children.Add(deleteButton);
                itemLayout.Children.Add(label);

                ItemsStackLayout.Children.Add(itemLayout);
            }
        }

        private async void OnDeleteCollectionClicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Usun kolekcje", $"Czy jestes pewien, ze chcesz usunac kolekcje: '{collection.Name}' ?", "Tak", "Nie");
            if (result)
            {
                App.MainPageInstance.collections.Remove(collection);
                App.MainPageInstance.SaveCollections();
                await Navigation.PopModalAsync();
                App.MainPageInstance.RefreshCollections();
            }
        }

       private async void OnChangeCollectionNameClicked(object sender, EventArgs e)
{
    string newName = await DisplayPromptAsync("Zmien nazwe kolekcji", "Podaj nowa nazwe:", "Zatwierdz", "Anuluj", collection.Name);
    if (!string.IsNullOrWhiteSpace(newName))
    {
        collection.Name = newName;
        App.MainPageInstance.SaveCollections();
        App.MainPageInstance.RefreshCollections();

        CollectionNameLabel.Text = newName;
    }
}


        private async void OnAddNewItemClicked(object sender, EventArgs e)
        {
            string newItem = await DisplayPromptAsync("Dodaj nowy element", "Podaj dane elementu:");
            if (!string.IsNullOrWhiteSpace(newItem))
            {
                collection.Items.Add(newItem);
                App.MainPageInstance.SaveCollections();
                RefreshItems();
            }
        }

        private async void OnEditItemClicked(string item)
        {
            string editedItem = await DisplayPromptAsync("Edytuj element", "Podaj nowe dane elementu:", "Zatwierdz", "Anuluj", item);
            if (!string.IsNullOrWhiteSpace(editedItem))
            {
                var index = collection.Items.IndexOf(item);
                collection.Items[index] = editedItem;
                App.MainPageInstance.SaveCollections();
                RefreshItems();
            }
        }

        private async void OnDeleteItemClicked(string item)
        {
            var result = await DisplayAlert("Usun element", $"Czy jestes pewien, ze chcesz usunac element: '{item}' ?", "Tak", "Nie");
            if (result)
            {
                collection.Items.Remove(item);
                App.MainPageInstance.SaveCollections();
                RefreshItems();
            }
        }

        private void RefreshItems()
        {
            ItemsStackLayout.Children.Clear();
            InitializeItems();
        }

        private async void OnBackToMainPageClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
