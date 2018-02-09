using CarouselView.FormsPlugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Xamarin.Forms;

namespace Xam.Plugin.TabView
{
    public class TabViewControl : ContentView
    {
        private StackLayout _mainContainerSL;
        private static Grid _headerContainerGrid;
        private CarouselViewControl _carouselView;

        private int _position = 0;
        private ObservableCollection<TabItem> TabItems = new ObservableCollection<TabItem>();

        public Color HeaderTextColor
        {
            get { return (Color)GetValue(WedgeRatingProperty); }
            set { SetValue(WedgeRatingProperty, value); }
        }

        public static void HeaderTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //foreach (StackLayout sl in _headerContainerSL.Children)
            //{
            //    ((Label)sl.Children.First()).TextColor = (Color)newValue;
            //}
        }

        public static readonly BindableProperty WedgeRatingProperty =
            BindableProperty.Create("HeaderTextColor", typeof(Color), typeof(TabViewControl), Color.White, BindingMode.OneWay, null, HeaderTextColorChanged);

        public Color HeaderBackgroundColor = Color.Black;
        public Color HeaderSelectionBarColor = Color.White;
        public double HeaderSelectionBarWidth = 50;
        public double HeaderSelectionBarThickness = 5;
        public double HeaderTextFontSize = 14;
        public double ContentHeightRequest = 200;

        public TabViewControl(List<TabItem> tabItems, int selectedTabIndex = 0)
        {
            Init();
            TabItems = new ObservableCollection<TabItem>(tabItems);
            _position = selectedTabIndex;

            InitTabs();
        }

        private void Init()
        {
            _headerContainerGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = HeaderBackgroundColor
            };

            _carouselView = new CarouselViewControl
            {
                HeightRequest = ContentHeightRequest,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ShowArrows = false,
                ShowIndicators = false
            };
            _carouselView.PositionSelected += (object sender, PositionSelectedEventArgs e) =>
            {
                SetPosition(e.NewValue);
            };

            _mainContainerSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { _headerContainerGrid, _carouselView },
                Spacing = 0
            };

            Content = _mainContainerSL;
        }

        private void InitTabs()
        {
            _headerContainerGrid.Children.Clear();
            _headerContainerGrid.ColumnDefinitions.Clear();
            _headerContainerGrid.RowDefinitions.Clear();

            for (int i = 0; i < TabItems.Count; i++)
            {
                _headerContainerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                var tab = TabItems[i];
                tab.IsCurrent = i == _position;

                var headerLabel = new Label
                {
                    Margin = new Thickness(5, 10, 5, 0),
                    FontSize = HeaderTextFontSize,
                    TextColor = HeaderTextColor,
                    BindingContext = tab,
                    VerticalTextAlignment = TextAlignment.Start,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center
                };
                headerLabel.SetBinding(Label.TextProperty, "HeaderText");

                var selectionBarBoxView = new BoxView
                {
                    Color = HeaderSelectionBarColor,
                    WidthRequest = HeaderSelectionBarWidth,
                    HeightRequest = HeaderSelectionBarThickness,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End,
                    BindingContext = tab
                };
                selectionBarBoxView.SetBinding(BoxView.IsVisibleProperty, "IsCurrent");

                var headerItemSL = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children = { headerLabel, selectionBarBoxView }
                };

                var tapRecognizer = new TapGestureRecognizer();
                int capturedIndex = i;
                tapRecognizer.Tapped += (object s, EventArgs e) =>
                {
                    SetPosition(capturedIndex);
                };
                headerItemSL.GestureRecognizers.Add(tapRecognizer);

                _headerContainerGrid.Children.Add(headerItemSL, i, 0);
            }

            _carouselView.ItemsSource = TabItems.Select(t => t.Content);
        }

        public void SetPosition(int position)
        {
            if (position >= 0 && position < TabItems.Count)
            {
                for (int i = 0; i < TabItems.Count; i++)
                {
                    TabItems[i].IsCurrent = i == position;
                }
            }
            _carouselView.Position = position;
            _position = position;
        }

        public class TabItem : ObservableBase
        {
            private string _headerText;
            public string HeaderText
            {
                get { return _headerText; }
                set { SetProperty(ref _headerText, value); }
            }

            private View _view;
            public View Content
            {
                get { return _view; }
                set { SetProperty(ref _view, value); }
            }

            private bool _isCurrent;
            public bool IsCurrent
            {
                get { return _isCurrent; }
                set { SetProperty(ref _isCurrent, value); }
            }
        }

        public class ObservableBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
            {
                if (object.Equals(storage, value)) return false;

                storage = value;
                this.OnPropertyChanged(propertyName);
                return true;
            }

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}