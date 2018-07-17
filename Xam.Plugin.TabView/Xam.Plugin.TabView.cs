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
    public delegate void PositionChangingEventHandler(object sender, PositionChangingEventArgs e);
    public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);

    public class PositionChangingEventArgs : EventArgs
    {
        public bool Canceled { get; set; }
        public int NewPosition { get; set; }
        public int OldPosition { get; set; }
    }

    public class PositionChangedEventArgs : EventArgs
    {
        public int NewPosition { get; set; }
        public int OldPosition { get; set; }
    }

    public class TabViewControl : ContentView
    {
        private StackLayout _mainContainerSL;
        private Grid _headerContainerGrid;
        private CarouselViewControl _carouselView;
        private int _position = 0;
        public static TabViewControl Context;

        public ObservableCollection<TabItem> ItemSource { get; set; }

        public event PositionChangingEventHandler PositionChanging;
        public event PositionChangedEventHandler PositionChanged;

        protected virtual void OnPositionChanging(ref PositionChangingEventArgs e)
        {
            PositionChangingEventHandler handler = PositionChanging;
            handler?.Invoke(this, e);
        }

        protected virtual void OnPositionChanged(PositionChangedEventArgs e)
        {
            PositionChangedEventHandler handler = PositionChanged;
            handler?.Invoke(this, e);
        }

        public TabViewControl()
        {
            //Parameterless constructor required for xaml instantiation.
        }
        
        public TabViewControl(IList<TabItem> tabItems, int selectedTabIndex = 0)
        {
            Initialize(tabItems, selectedTabIndex);
        }

        private void Initialize(IList<TabItem> tabItems, int selectedTabIndex = 0)
        {
            ItemSource = new ObservableCollection<TabItem>();

            foreach (var tab in tabItems)
            {
                tab.HeaderTextColor = HeaderTabTextColor;
                tab.HeaderSelectionUnderlineColor = HeaderSelectionUnderlineColor;
                tab.HeaderSelectionUnderlineThickness = HeaderSelectionUnderlineThickness;
                tab.HeaderSelectionUnderlineWidth = HeaderSelectionUnderlineWidth;
                tab.HeaderTabTextFontSize = HeaderTabTextFontSize;
                tab.HeaderTabTextFontFamily = HeaderTabTextFontFamily;
                tab.HeaderTabTextFontAttributes = HeaderTabTextFontAttributes;

                ItemSource.Add(tab);
            }

            Init();

            ItemSource.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                foreach (var tab in ItemSource)
                {
                    tab.HeaderTextColor = HeaderTabTextColor;
                    tab.HeaderSelectionUnderlineColor = HeaderSelectionUnderlineColor;
                    tab.HeaderSelectionUnderlineThickness = HeaderSelectionUnderlineThickness;
                    tab.HeaderSelectionUnderlineWidth = HeaderSelectionUnderlineWidth;
                    tab.HeaderTabTextFontSize = HeaderTabTextFontSize;
                    tab.HeaderTabTextFontFamily = HeaderTabTextFontFamily;
                    tab.HeaderTabTextFontAttributes = HeaderTabTextFontAttributes;
                }
                InitTabs();
            };

            _position = selectedTabIndex;
            InitTabs();

            Context = this;

            _carouselView.PropertyChanged += _carouselView_PropertyChanged;
            _carouselView.PropertyChanged += _carouselView_PropertyChanged;
        }

        private bool _supressCarouselViewPositionChangedEvent = false;
        private void _carouselView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_carouselView.Position) && !_supressCarouselViewPositionChangedEvent)
            {
                var positionChangingArgs = new PositionChangingEventArgs()
                {
                    Canceled = false,
                    NewPosition = _carouselView.Position,
                    OldPosition = _position
                };

                OnPositionChanging(ref positionChangingArgs);

                if (positionChangingArgs != null && positionChangingArgs.Canceled)
                {
                    _supressCarouselViewPositionChangedEvent = true;
                    _carouselView.PositionSelected -= _carouselView_PositionSelected;
                    _carouselView.PropertyChanged -= _carouselView_PropertyChanged;
                    _carouselView.Position = _position;
                    _carouselView.PositionSelected += _carouselView_PositionSelected;
                    _carouselView.PropertyChanged += _carouselView_PropertyChanged;
                    _supressCarouselViewPositionChangedEvent = false;
                }
            }
        }

        private void Init()
        {
            _headerContainerGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = HeaderBackgroundColor,
                MinimumHeightRequest = 50
            };

            _carouselView = new CarouselViewControl
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = ContentHeight,
                ShowArrows = false,
                ShowIndicators = false,
                BindingContext = this
            };

            _carouselView.PositionSelected += _carouselView_PositionSelected;

            _mainContainerSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { _headerContainerGrid, _carouselView },
                Spacing = 0
            };

            Content = _mainContainerSL;
        }

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			if (BindingContext != null)
			{
				foreach (var tab in ItemSource)
				{
					if (tab is TabItem view)
					{
						view.Content.BindingContext = BindingContext;
					}
				}
			}
		}

        private void _carouselView_PositionSelected(object sender, PositionSelectedEventArgs e)
        {
            SetPosition(e.NewValue);
        }

        private void InitTabs()
        {
            _headerContainerGrid.Children.Clear();
            _headerContainerGrid.ColumnDefinitions.Clear();
            _headerContainerGrid.RowDefinitions.Clear();

            var tabSize = (TabSizeOption.IsAbsolute && TabSizeOption.Value.Equals(0)) ? new GridLength(1, GridUnitType.Star) : TabSizeOption;

            for (int i = 0; i < ItemSource.Count; i++)
            {
                _headerContainerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                var tab = ItemSource[i];
                tab.IsCurrent = i == _position;

                var headerLabel = new Label
                {
                    Margin = new Thickness(5, 10, 5, 0),
                    BindingContext = tab,
                    VerticalTextAlignment = TextAlignment.Start,
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center
                };
                headerLabel.SetBinding(Label.TextProperty, "HeaderText");
                headerLabel.SetBinding(Label.TextColorProperty, "HeaderTextColor");
                headerLabel.SetBinding(Label.FontSizeProperty, "HeaderTabTextFontSize");
                headerLabel.SetBinding(Label.FontFamilyProperty, "HeaderTabTextFontFamily");
                headerLabel.SetBinding(Label.FontAttributesProperty, "HeaderTabTextFontAttributes");

                var selectionBarBoxView = new BoxView
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.EndAndExpand,
                    BindingContext = tab,
                    HeightRequest = HeaderSelectionUnderlineThickness,
                    WidthRequest = HeaderSelectionUnderlineWidth
                };
                selectionBarBoxView.SetBinding(BoxView.IsVisibleProperty, "IsCurrent");
                selectionBarBoxView.SetBinding(BoxView.ColorProperty, "HeaderSelectionUnderlineColor");
                selectionBarBoxView.SetBinding(BoxView.WidthProperty, "HeaderSelectionUnderlineWidth");
                selectionBarBoxView.SetBinding(BoxView.HeightProperty, "HeaderSelectionUnderlineThickness");

                selectionBarBoxView.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == "IsCurrent")
                    {
                        SetPosition(ItemSource.IndexOf((TabItem)((BoxView)sender).BindingContext));
                    }
                };

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
                    _supressCarouselViewPositionChangedEvent = true;
                    SetPosition(capturedIndex);
                    _supressCarouselViewPositionChangedEvent = false;
                };
                headerItemSL.GestureRecognizers.Add(tapRecognizer);

                _headerContainerGrid.Children.Add(headerItemSL, i, 0);
            }

            _carouselView.ItemsSource = ItemSource.Select(t => t.Content);
        }

        #region HeaderBackgroundColor
        public Color HeaderBackgroundColor
        {
            get { return (Color)GetValue(HeaderBackgroundColorProperty); }
            set { SetValue(HeaderBackgroundColorProperty, value); }
        }
        public static void HeaderBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Context._headerContainerGrid.BackgroundColor = (Color)newValue;
        }
        public readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(nameof(HeaderBackgroundColor), typeof(Color), typeof(TabViewControl), Color.Black, BindingMode.Default, null, HeaderBackgroundColorChanged);
        #endregion

        #region HeaderTabTextColor
        public Color HeaderTabTextColor
        {
            get { return (Color)GetValue(HeaderTabTextColorProperty); }
            set { SetValue(HeaderTabTextColorProperty, value); }
        }
        public static void HeaderTabTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            foreach (var tab in Context.ItemSource)
            {
                tab.HeaderTextColor = (Color)newValue;
            }
        }
        public readonly BindableProperty HeaderTabTextColorProperty =
            BindableProperty.Create(nameof(HeaderTabTextColor), typeof(Color), typeof(TabViewControl), Color.White, BindingMode.OneWay, null, HeaderTabTextColorChanged);
        #endregion

        #region ContentHeight
        public double ContentHeight
        {
            get { return (double)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }
        public static void ContentHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Context._carouselView.HeightRequest = (double)newValue;
        }
        public readonly BindableProperty ContentHeightProperty = BindableProperty.Create(nameof(ContentHeight), typeof(double), typeof(TabViewControl), (double)200, BindingMode.Default, null, ContentHeightChanged);
        #endregion

        #region HeaderSelectionUnderlineColor
        public Color HeaderSelectionUnderlineColor
        {
            get { return (Color)GetValue(HeaderSelectionUnderlineColorProperty); }
            set { SetValue(HeaderSelectionUnderlineColorProperty, value); }
        }
        public static void HeaderSelectionUnderlineColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            foreach (var tab in Context.ItemSource)
            {
                tab.HeaderSelectionUnderlineColor = (Color)newValue;
            }
        }
        public readonly BindableProperty HeaderSelectionUnderlineColorProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineColor), typeof(Color), typeof(TabViewControl), Color.White, BindingMode.Default, null, HeaderSelectionUnderlineColorChanged);
        #endregion

        #region HeaderSelectionUnderlineThickness
        public double HeaderSelectionUnderlineThickness
        {
            get { return (double)GetValue(HeaderSelectionUnderlineThicknessProperty); }
            set { SetValue(HeaderSelectionUnderlineThicknessProperty, value); }
        }
        public static void HeaderSelectionUnderlineThicknessChanged(BindableObject bindable, object oldValue, object newValue)
        {
            foreach (var tab in Context.ItemSource)
            {
                tab.HeaderSelectionUnderlineThickness = (double)newValue;
            }
        }
        public readonly BindableProperty HeaderSelectionUnderlineThicknessProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineThickness), typeof(double), typeof(TabViewControl), (double)5, BindingMode.Default, null, HeaderSelectionUnderlineThicknessChanged);
        #endregion

        #region HeaderSelectionUnderlineWidth
        public double HeaderSelectionUnderlineWidth
        {
            get { return (double)GetValue(HeaderSelectionUnderlineWidthProperty); }
            set { SetValue(HeaderSelectionUnderlineWidthProperty, value); }
        }
        public static void HeaderSelectionUnderlineWidthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            foreach (var tab in Context.ItemSource)
            {
                tab.HeaderSelectionUnderlineWidth = (double)newValue;
            }
        }
        public readonly BindableProperty HeaderSelectionUnderlineWidthProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineWidth), typeof(double), typeof(TabViewControl), (double)40, BindingMode.Default, null, HeaderSelectionUnderlineWidthChanged);
        #endregion

        #region HeaderTabTextFontSize
        public double HeaderTabTextFontSize
        {
            get { return (double)GetValue(HeaderTabTextFontSizeProperty); }
            set { SetValue(HeaderTabTextFontSizeProperty, value); }
        }
        public static void HeaderTabTextFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            foreach (var tab in Context.ItemSource)
            {
                tab.HeaderTabTextFontSize = (double)newValue;
            }
        }
        public readonly BindableProperty HeaderTabTextFontSizeProperty = BindableProperty.Create(nameof(HeaderTabTextFontSize), typeof(double), typeof(TabViewControl), (double)14, BindingMode.Default, null, HeaderTabTextFontSizeChanged);
        #endregion

        #region HeaderTabTextFontFamily
        public string HeaderTabTextFontFamily
        {
            get { return (string)GetValue(HeaderTabTextFontFamilyProperty); }
            set { SetValue(HeaderTabTextFontFamilyProperty, value); }
        }
        public static void HeaderTabTextFontFamilyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            foreach (var tab in Context.ItemSource)
            {
                tab.HeaderTabTextFontFamily = (string)newValue;
            }
        }
        public readonly BindableProperty HeaderTabTextFontFamilyProperty = BindableProperty.Create(nameof(HeaderTabTextFontFamily), typeof(string), typeof(TabViewControl), null, BindingMode.Default, null, HeaderTabTextFontFamilyChanged);
        #endregion

        #region HeaderTabTextFontAttributes
        public FontAttributes HeaderTabTextFontAttributes
        {
            get { return (FontAttributes)GetValue(HeaderTabTextFontAttributesProperty); }
            set { SetValue(HeaderTabTextFontAttributesProperty, value); }
        }
        public static void HeaderTabTextFontAttributesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            foreach (var tab in Context.ItemSource)
            {
                tab.HeaderTabTextFontAttributes = (FontAttributes)newValue;
            }
        }
        public readonly BindableProperty HeaderTabTextFontAttributesProperty = BindableProperty.Create(nameof(HeaderTabTextFontAttributes), typeof(FontAttributes), typeof(TabViewControl), FontAttributes.None, BindingMode.Default, null, HeaderTabTextFontAttributesChanged);
        #endregion

		#region TabItems
        public static BindableProperty TabItemsProperty = BindableProperty.Create(nameof(TabItems), typeof(IList<TabItem>), typeof(TabViewControl), null, propertyChanged:OnTabItemsChanged);
        private static void OnTabItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabControl)
            {
                tabControl.Initialize(tabControl.TabItems);
            }
        }
        public IList<TabItem> TabItems
        {
            get => (IList<TabItem>)GetValue(TabItemsProperty);
            set { SetValue(TabItemsProperty, value); }
        }
        #endregion

        #region TabSizeOption
		public static BindableProperty TabSizeOptionProperty = BindableProperty.Create(nameof(TabSizeOption), typeof(GridLength), typeof(TabViewControl), default(GridLength), propertyChanged: OnTabSizeOptionChanged);
		private static void OnTabSizeOptionChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (bindable is TabViewControl tabViewControl)
			{
				if (tabViewControl._headerContainerGrid != null && tabViewControl.ItemSource != null)
				{
					foreach (var tabContainer in tabViewControl._headerContainerGrid.ColumnDefinitions)
					{
						tabContainer.Width = (GridLength)newValue;
					}
				}
			}
		}
		public GridLength TabSizeOption
		{
			get => (GridLength)GetValue(TabSizeOptionProperty);
			set { SetValue(TabSizeOptionProperty, value); }
		}
		#endregion

        public void SetPosition(int position)
        {
            int oldPosition = _position;

            var positionChangingArgs = new PositionChangingEventArgs()
            {
                Canceled = false,
                NewPosition = position,
                OldPosition = oldPosition
            };
            OnPositionChanging(ref positionChangingArgs);

            if (positionChangingArgs != null && positionChangingArgs.Canceled)
            {
                return;
            }

            if (position >= 0 && position < ItemSource.Count)
            {
                for (int i = 0; i < ItemSource.Count; i++)
                {
                    ItemSource[i].IsCurrent = i == position;
                }

                _carouselView.PositionSelected -= _carouselView_PositionSelected;
                _carouselView.Position = position;
                _carouselView.PositionSelected += _carouselView_PositionSelected;

                _position = position;
            }

            var positionChangedArgs = new PositionChangedEventArgs()
            {
                NewPosition = _position,
                OldPosition = oldPosition
            };
            OnPositionChanged(positionChangedArgs);
        }

        public void SelectNext()
        {
            SetPosition(_position + 1);
        }

        public void SelectPrevious()
        {
            SetPosition(_position - 1);
        }

        public void SelectFirst()
        {
            SetPosition(0);
        }

        public void SelectLast()
        {
            SetPosition(ItemSource.Count - 1);
        }

        public void AddTab(TabItem tab, int position = 0, bool selectNewPosition = false)
        {
            if (selectNewPosition)
            {
                _position = position;
            }
            ItemSource.Insert(position, tab);
        }

        public void RemoveTab(int position = 0)
        {
            ItemSource.RemoveAt(position);

            if (position > 0)
            {
                _position = position - 1;
            }
        }
    }

    public class TabItem : BindableObject
    {
        public TabItem()
        {
            //Parameterless constructor required for xaml instantiation.
        }

        public TabItem(string headerText, View content)
        {
            HeaderText = headerText;
            Content = content;
        }

        public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(TabItem), string.Empty);
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(TabItem));
        public View Content
        {
            get { return (View)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly BindableProperty IsCurrentProperty = BindableProperty.Create(nameof(IsCurrent), typeof(bool), typeof(TabItem), false);
        public bool IsCurrent
        {
            get { return (bool)GetValue(IsCurrentProperty); }
            set { SetValue(IsCurrentProperty, value); }
        }

        public static readonly BindableProperty HeaderTextColorProperty = BindableProperty.Create(nameof(HeaderTextColor), typeof(Color), typeof(TabItem), Color.Black);
        public Color HeaderTextColor
        {
            get { return (Color)GetValue(HeaderTextColorProperty); }
            set { SetValue(HeaderTextColorProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineColorProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineColor), typeof(Color), typeof(TabItem), Color.White);
        public Color HeaderSelectionUnderlineColor
        {
            get { return (Color)GetValue(HeaderSelectionUnderlineColorProperty); }
            set { SetValue(HeaderSelectionUnderlineColorProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineThicknessProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineThickness), typeof(double), typeof(TabItem), (double)5);
        public double HeaderSelectionUnderlineThickness
        {
            get { return (double)GetValue(HeaderSelectionUnderlineThicknessProperty); }
            set { SetValue(HeaderSelectionUnderlineThicknessProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineWidthProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineWidth), typeof(double), typeof(TabItem), (double)40);
        public double HeaderSelectionUnderlineWidth
        {
            get { return (double)GetValue(HeaderSelectionUnderlineWidthProperty); }
            set { SetValue(HeaderSelectionUnderlineWidthProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontSizeProperty = BindableProperty.Create(nameof(HeaderTabTextFontSize), typeof(double), typeof(TabItem), (double)14);
        public double HeaderTabTextFontSize
        {
            get { return (double)GetValue(HeaderTabTextFontSizeProperty); }
            set { SetValue(HeaderTabTextFontSizeProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontFamilyProperty = BindableProperty.Create(nameof(HeaderTabTextFontFamily), typeof(string), typeof(TabItem));
        public string HeaderTabTextFontFamily
        {
            get { return (string)GetValue(HeaderTabTextFontFamilyProperty); }
            set { SetValue(HeaderTabTextFontFamilyProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontAttributesProperty = BindableProperty.Create(nameof(HeaderTabTextFontAttributes), typeof(FontAttributes), typeof(TabItem), FontAttributes.None);
        public FontAttributes HeaderTabTextFontAttributes
        {
            get { return (FontAttributes)GetValue(HeaderTabTextFontAttributesProperty); }
            set { SetValue(HeaderTabTextFontAttributesProperty, value); }
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
