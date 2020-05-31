using Neemacademy.CustomControls.Xam.Plugin.TabView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Xam.Plugin.TabView;
using Xamarin.Forms;

namespace TabViewSample.ViewModels
{
    public class TemplatedItemSourceSamplePageViewModel : ObservableBase
    {
        private ObservableCollection<AnimalCategory> _animalCategories;
        public ObservableCollection<AnimalCategory> AnimalCategories
        {
            get { return _animalCategories; }
            set { SetProperty(ref _animalCategories, value); }
        }

        private int _selectedCategoryIndex;
        public int SelectedCategoryIndex
        {
            get { return _selectedCategoryIndex; }
            set { SetProperty(ref _selectedCategoryIndex, value); }
        }

        public TemplatedItemSourceSamplePageViewModel()
        {
            SelectedCategoryIndex = 1;

            AnimalCategories = new ObservableCollection<AnimalCategory>();
            AnimalCategories.Add(new AnimalCategory()
            {
                TabViewControlTabItemTitle = "Mammals",
                TabViewControlTabItemIconSource = "icon.png",
                Animals  = new ObservableCollection<string>()
                {
                    "Elephant",
                    "Rabbit",
                    "Monkey",
                    "Whale"
                }
            });
            AnimalCategories.Add(new AnimalCategory()
            {
                TabViewControlTabItemTitle = "Birds",
                TabViewControlTabItemIconSource = "icon.png",
                Animals = new ObservableCollection<string>()
                {
                    "Duck",
                    "Crow",
                    "Sparrow",
                    "Eagle"
                }
            });
            AnimalCategories.Add(new AnimalCategory()
            {
                TabViewControlTabItemTitle = "Fish",
                TabViewControlTabItemIconSource = "icon.png",
                Animals = new ObservableCollection<string>()
                {
                    "Carp",
                    "Salmon",
                    "Swordfish",
                    "Eel"
                }
            });
        }

        public class AnimalCategory : ObservableBase, ITabViewControlTabItem
        {
            public string TabViewControlTabItemTitle { get; set; }
            public ImageSource TabViewControlTabItemIconSource { get; set; }
            public ObservableCollection<string> Animals { get; set; }

            public void TabViewControlTabItemFocus()
            {
                Debug.WriteLine($"{TabViewControlTabItemTitle} tab received focus.");
            }
        }
    }
}
