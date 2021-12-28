using CarouselView.FormsPlugin.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xam.Plugin.TabView.Converters;
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

    static class TabDefaults
    {
        public static readonly Color DefaultColor = Color.White;
        public const double DefaultThickness = 5;
        public const double DefaultTextSize = 14;
    }

    public class TabViewControl : ContentView
    {
        private StackLayout _mainContainerSL;
        private Grid _headerContainerGrid;
        private CarouselViewControl _carouselView;
        private ScrollView _tabHeadersContainerSv;

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
            Init();
        }

        public TabViewControl(IList<TabItem> tabItems, int selectedTabIndex = 0)
        {
            Init();
            foreach (var tab in tabItems)
            {
                ItemSource.Add(tab);
            }
            if (selectedTabIndex > 0)
            {
                SelectedTabIndex = selectedTabIndex;
            }
        }

        //refactor
        void ItemSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var tab in e.NewItems)
                {
                    if (tab is TabItem newTab)
                    {
                        SetTabProps(newTab);
                        AddTabToView(newTab);
                    }
                }
            }
        }

        private void SetTabProps(TabItem tab)
        {
            //Set the tab properties from the main control only if not set in xaml at the individual tab level.
            if (tab.HeaderTextColor == TabDefaults.DefaultColor && HeaderTabTextColor != TabDefaults.DefaultColor)
                tab.HeaderTextColor = HeaderTabTextColor;
            if (tab.HeaderSelectionUnderlineColor == TabDefaults.DefaultColor && HeaderSelectionUnderlineColor != TabDefaults.DefaultColor)
                tab.HeaderSelectionUnderlineColor = HeaderSelectionUnderlineColor;
            if (tab.HeaderSelectionUnderlineThickness.Equals(TabDefaults.DefaultThickness) && !HeaderSelectionUnderlineThickness.Equals(TabDefaults.DefaultThickness))
                tab.HeaderSelectionUnderlineThickness = HeaderSelectionUnderlineThickness;
            if (tab.HeaderSelectionUnderlineWidth > 0)
                tab.HeaderSelectionUnderlineWidth = HeaderSelectionUnderlineWidth;
            if (tab.HeaderTabTextFontSize.Equals(TabDefaults.DefaultTextSize) && !HeaderTabTextFontSize.Equals(TabDefaults.DefaultTextSize))
                tab.HeaderTabTextFontSize = HeaderTabTextFontSize;
            if (tab.HeaderTabTextFontFamily is null && !string.IsNullOrWhiteSpace(HeaderTabTextFontFamily))
                tab.HeaderTabTextFontFamily = HeaderTabTextFontFamily;
            if (tab.HeaderTabTextFontAttributes == FontAttributes.None && HeaderTabTextFontAttributes != FontAttributes.None)
                tab.HeaderTabTextFontAttributes = HeaderTabTextFontAttributes;
        }

        private bool _supressCarouselViewPositionChangedEvent;
        private void _carouselView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_carouselView.Position) && !_supressCarouselViewPositionChangedEvent)
            {
                var positionChangingArgs = new PositionChangingEventArgs()
                {
                    Canceled = false,
                    NewPosition = _carouselView.Position,
                    OldPosition = SelectedTabIndex
                };

                OnPositionChanging(ref positionChangingArgs);

                if (positionChangingArgs != null && positionChangingArgs.Canceled)
                {
                    _supressCarouselViewPositionChangedEvent = true;
                    _carouselView.PositionSelected -= _carouselView_PositionSelected;
                    _carouselView.PropertyChanged -= _carouselView_PropertyChanged;
                    _carouselView.Position = SelectedTabIndex;
                    _carouselView.PositionSelected += _carouselView_PositionSelected;
                    _carouselView.PropertyChanged += _carouselView_PropertyChanged;
                    _supressCarouselViewPositionChangedEvent = false;
                }

                SetPosition(positionChangingArgs.NewPosition);
            }
        }

        private void Init()
        {
            ItemSource = new ObservableCollection<TabItem>();

            _headerContainerGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                // BackgroundColor = Color.White, // tab sepeartor color
                MinimumHeightRequest = 50,
                ColumnSpacing = 0, // seperator thickness
            };

            _tabHeadersContainerSv = new ScrollView()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                Orientation = ScrollOrientation.Horizontal,
                Content = _headerContainerGrid,
                BackgroundColor = HeaderBackgroundColor,
                HorizontalOptions = LayoutOptions.FillAndExpand
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
            _carouselView.PropertyChanged += _carouselView_PropertyChanged;
            _carouselView.PositionSelected += _carouselView_PositionSelected;

            _mainContainerSL = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { _tabHeadersContainerSv, _carouselView },
                Spacing = 0
            };

            Content = _mainContainerSL;
            ItemSource.CollectionChanged += ItemSource_CollectionChanged;
            SetPosition(SelectedTabIndex, true);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null && ItemSource != null && ItemTemplate == null)
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
            if (_carouselView.Position != e.NewValue || SelectedTabIndex != e.NewValue)
            {
                SetPosition(e.NewValue);
            }
        }

        private void AddTabToView(TabItem tab)
        {
            var tabSize = (TabSizeOption.IsAbsolute && TabSizeOption.Value.Equals(0)) ? new GridLength(1, GridUnitType.Star) : TabSizeOption;

            _headerContainerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = tabSize });

            tab.IsCurrent = _headerContainerGrid.ColumnDefinitions.Count - 1 == SelectedTabIndex;

            var headerIcon = new Image
            {
                Margin = new Thickness(0, 8, 0, 0),
                BindingContext = tab,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = tab.HeaderIconSize,
                HeightRequest = tab.HeaderIconSize
            };
            headerIcon.SetBinding(Image.SourceProperty, nameof(TabItem.HeaderIcon));
            headerIcon.SetBinding(IsVisibleProperty, nameof(TabItem.HeaderIcon), converter: new NullToBoolConverter());

            var headerLabel = new Label
            {
                BindingContext = tab,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(5, 8, 5, 5)
            };

            headerLabel.SetBinding(Label.TextProperty, nameof(TabItem.HeaderText));
            headerLabel.SetBinding(Label.TextColorProperty, nameof(TabItem.HeaderTextColor));
            headerLabel.SetBinding(Label.FontSizeProperty, nameof(TabItem.HeaderTabTextFontSize));
            headerLabel.SetBinding(Label.FontFamilyProperty, nameof(TabItem.HeaderTabTextFontFamily));
            headerLabel.SetBinding(Label.FontAttributesProperty, nameof(TabItem.HeaderTabTextFontAttributes));
            headerLabel.SetBinding(IsVisibleProperty, nameof(TabItem.HeaderText), converter: new NullToBoolConverter());

            var selectionBarBoxView = new BoxView
            {
                VerticalOptions = LayoutOptions.End,
                BindingContext = tab,
                HeightRequest = HeaderSelectionUnderlineThickness,
                WidthRequest = HeaderSelectionUnderlineWidth
            };
            //selectionBarBoxView.SetBinding(IsVisibleProperty, nameof(TabItem.IsCurrent));
            var underlineColorBinding = new Binding(nameof(TabItem.IsCurrent), BindingMode.OneWay, new SelectedTabHeaderToTabBackgroundColorConverter(), this);
            //selectionBarBoxView.SetBinding(BoxView.ColorProperty, nameof(TabItem.HeaderSelectionUnderlineColor), BindingMode.OneWay, new SelectedTabHeaderToTabBackgroundColorConverter(), );
            selectionBarBoxView.SetBinding(BoxView.BackgroundColorProperty, underlineColorBinding);

            selectionBarBoxView.SetBinding(WidthRequestProperty, nameof(TabItem.HeaderSelectionUnderlineWidth));
            selectionBarBoxView.SetBinding(HeightRequestProperty, nameof(TabItem.HeaderSelectionUnderlineThickness));
            selectionBarBoxView.SetBinding(HorizontalOptionsProperty,
                                           nameof(TabItem.HeaderSelectionUnderlineWidth),
                                           converter: new DoubleToLayoutOptionsConverter());

            selectionBarBoxView.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == nameof(TabItem.IsCurrent))
                {
                    SetPosition(ItemSource.IndexOf((TabItem)((BoxView)sender).BindingContext));
                }
                if (e.PropertyName == nameof(WidthRequest))
                {
                    selectionBarBoxView.HorizontalOptions = tab.HeaderSelectionUnderlineWidth > 0 ? LayoutOptions.CenterAndExpand : LayoutOptions.FillAndExpand;
                }
            };

            View headerItemView;
            if (TabHeaderItemTemplate != null)
            {
                headerItemView = (View)TabHeaderItemTemplate.CreateContent();
                headerItemView.BindingContext = tab;
            }
            else
            {
                headerItemView = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children = { headerIcon, headerLabel, selectionBarBoxView },
                    BackgroundColor = HeaderBackgroundColor
                };
            }

            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += (object s, EventArgs e) =>
            {
                _supressCarouselViewPositionChangedEvent = true;
                var capturedIndex = _headerContainerGrid.Children.IndexOf((View)s);
                SetPosition(capturedIndex);
                _supressCarouselViewPositionChangedEvent = false;
            };
            headerItemView.GestureRecognizers.Add(tapRecognizer);
            _headerContainerGrid.Children.Add(headerItemView, _headerContainerGrid.ColumnDefinitions.Count - 1, 0);
            _carouselView.ItemsSource = ItemSource.Select(t => t.Content);
        }

        #region ItemTemplate
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        private static void ItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl)
            {
                tabViewControl.setTabViewItems();
            }
        }
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(TabViewControl), null, BindingMode.Default, null, ItemTemplateChanged);
        #endregion

        #region TabHeaderItemTemplate
        public DataTemplate TabHeaderItemTemplate
        {
            get { return (DataTemplate)GetValue(TabHeaderItemTemplateProperty); }
            set { SetValue(TabHeaderItemTemplateProperty, value); }
        }
        private static void TabHeaderItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl)
            {
                tabViewControl.setTabViewItems();
            }
        }
        public static readonly BindableProperty TabHeaderItemTemplateProperty = BindableProperty.Create(nameof(TabHeaderItemTemplate), typeof(DataTemplate), typeof(TabViewControl), null, BindingMode.Default, null, TabHeaderItemTemplateChanged);
        #endregion

        #region TemplatedItemSource
        public IEnumerable<ITabViewControlTabItem> TemplatedItemSource
        {
            get => (IEnumerable<ITabViewControlTabItem>)GetValue(TemplatedItemSourceProperty);
            set { SetValue(TemplatedItemSourceProperty, value); }
        }
        private static void TemplatedItemSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl)
            {
                tabViewControl.setTabViewItems();
            }
        }
        public static readonly BindableProperty TemplatedItemSourceProperty = BindableProperty.Create(nameof(TemplatedItemSource), typeof(IEnumerable<ITabViewControlTabItem>), typeof(TabViewControl), null, BindingMode.Default, null, TemplatedItemSourceChanged);
        #endregion

        private void setTabViewItems()
        {
            if (ItemTemplate == null || TemplatedItemSource == null || ItemSource == null)
            {
                return;
            }

            _headerContainerGrid.Children.Clear();
            _headerContainerGrid.ColumnDefinitions.Clear();
            ItemSource.Clear();

            foreach (var itm in TemplatedItemSource)
            {
                setTabViewItem(itm);
            }
        }

        private void setTabViewItem(ITabViewControlTabItem itm)
        {
            DataTemplate itemTemplate = ItemTemplate;

            if (itemTemplate == null)
                return;

            View carouselViewItem = (View)itemTemplate.CreateContent();
            carouselViewItem.BindingContext = itm;

            var newTabItem = new TabItem(itm.TabViewControlTabItemTitle, carouselViewItem, itm.TabViewControlTabItemIconSource);
            ItemSource.Add(newTabItem);
        }

        #region IsSwipeEnabled
        public bool IsSwipeEnabled
        {
            get { return (bool)GetValue(IsSwipeEnabledProperty); }
            set { SetValue(IsSwipeEnabledProperty, value); }
        }
        private static void IsSwipeEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl)
            {
                tabViewControl._carouselView.IsSwipeEnabled = (bool)newValue;
            }
        }
        public static readonly BindableProperty IsSwipeEnabledProperty = BindableProperty.Create(nameof(IsSwipeEnabled), typeof(bool), typeof(TabViewControl), true, BindingMode.Default, null, IsSwipeEnabledChanged);
        #endregion

        #region HeaderBackgroundColor
        public Color HeaderBackgroundColor
        {
            get { return (Color)GetValue(HeaderBackgroundColorProperty); }
            set { SetValue(HeaderBackgroundColorProperty, value); }
        }
        private static void HeaderBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl)
            {
                tabViewControl._tabHeadersContainerSv.BackgroundColor = (Color)newValue;
            }
        }
        public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(nameof(HeaderBackgroundColor), typeof(Color), typeof(TabViewControl), Color.SkyBlue, BindingMode.Default, null, HeaderBackgroundColorChanged);
        #endregion

        #region HeaderTabTextColor
        public Color HeaderTabTextColor
        {
            get { return (Color)GetValue(HeaderTabTextColorProperty); }
            set { SetValue(HeaderTabTextColorProperty, value); }
        }
        private static void HeaderTabTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl.ItemSource != null)
            {
                foreach (var tab in tabViewControl.ItemSource)
                {
                    tab.HeaderTextColor = (Color)newValue;
                }
            }
        }
        public static readonly BindableProperty HeaderTabTextColorProperty =
            BindableProperty.Create(nameof(HeaderTabTextColor), typeof(Color), typeof(TabViewControl), TabDefaults.DefaultColor, BindingMode.OneWay, null, HeaderTabTextColorChanged);
        #endregion

        #region ContentHeight
        public double ContentHeight
        {
            get { return (double)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }
        private static void ContentHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl._carouselView != null)
            {
                tabViewControl._carouselView.HeightRequest = (double)newValue;
            }
        }
        public static readonly BindableProperty ContentHeightProperty = BindableProperty.Create(nameof(ContentHeight), typeof(double), typeof(TabViewControl), (double)200, BindingMode.Default, null, ContentHeightChanged);
        #endregion

        #region HeaderSelectionUnderlineColor
        public Color HeaderSelectionUnderlineColor
        {
            get { return (Color)GetValue(HeaderSelectionUnderlineColorProperty); }
            set { SetValue(HeaderSelectionUnderlineColorProperty, value); }
        }
        private static void HeaderSelectionUnderlineColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl.ItemSource != null)
            {
                foreach (var tab in tabViewControl.ItemSource)
                {
                    tab.HeaderSelectionUnderlineColor = (Color)newValue;
                    tab.TriggerPropertyChange(nameof(tab.IsCurrent));
                }
            }
        }
        public static readonly BindableProperty HeaderSelectionUnderlineColorProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineColor), typeof(Color), typeof(TabViewControl), Color.White, BindingMode.Default, null, HeaderSelectionUnderlineColorChanged);
        #endregion

        #region HeaderSelectionUnderlineThickness
        public double HeaderSelectionUnderlineThickness
        {
            get { return (double)GetValue(HeaderSelectionUnderlineThicknessProperty); }
            set { SetValue(HeaderSelectionUnderlineThicknessProperty, value); }
        }
        private static void HeaderSelectionUnderlineThicknessChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl.ItemSource != null)
            {
                foreach (var tab in tabViewControl.ItemSource)
                {
                    tab.HeaderSelectionUnderlineThickness = (double)newValue;
                }
            }
        }
        public static readonly BindableProperty HeaderSelectionUnderlineThicknessProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineThickness), typeof(double), typeof(TabViewControl), TabDefaults.DefaultThickness, BindingMode.Default, null, HeaderSelectionUnderlineThicknessChanged);
        #endregion

        #region HeaderSelectionUnderlineWidth
        public double HeaderSelectionUnderlineWidth
        {
            get { return (double)GetValue(HeaderSelectionUnderlineWidthProperty); }
            set { SetValue(HeaderSelectionUnderlineWidthProperty, value); }
        }
        private static void HeaderSelectionUnderlineWidthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl.ItemSource != null)
            {
                foreach (var tab in tabViewControl.ItemSource)
                {
                    tab.HeaderSelectionUnderlineWidth = (double)newValue;
                }
            }
        }
        public static readonly BindableProperty HeaderSelectionUnderlineWidthProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineWidth), typeof(double), typeof(TabViewControl), (double)0, BindingMode.Default, null, HeaderSelectionUnderlineWidthChanged);
        #endregion

        #region HeaderTabTextFontSize
        [Xamarin.Forms.TypeConverter(typeof(FontSizeConverter))]
        public double HeaderTabTextFontSize
        {
            get { return (double)GetValue(HeaderTabTextFontSizeProperty); }
            set { SetValue(HeaderTabTextFontSizeProperty, value); }
        }
        private static void HeaderTabTextFontSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl.ItemSource != null)
            {
                foreach (var tab in tabViewControl.ItemSource)
                {
                    tab.HeaderTabTextFontSize = (double)newValue;
                }
            }
        }
        public static readonly BindableProperty HeaderTabTextFontSizeProperty = BindableProperty.Create(nameof(HeaderTabTextFontSize), typeof(double), typeof(TabViewControl), (double)14, BindingMode.Default, null, HeaderTabTextFontSizeChanged);
        #endregion

        #region TabHeaderSpacing
        [Xamarin.Forms.TypeConverter(typeof(FontSizeConverter))]
        public double TabHeaderSpacing
        {
            get { return (double)GetValue(TabHeaderSpacingProperty); }
            set { SetValue(TabHeaderSpacingProperty, value); }
        }
        private static void TabHeaderSpacingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl)
            {
                double colSpacing;
                double.TryParse(oldValue?.ToString(), out colSpacing);
                tabViewControl._headerContainerGrid.ColumnSpacing = colSpacing;
            }
        }
        public static readonly BindableProperty TabHeaderSpacingProperty = BindableProperty.Create(nameof(TabHeaderSpacing), typeof(double), typeof(TabViewControl), (double)14, BindingMode.Default, null, TabHeaderSpacingChanged);
        #endregion

        #region HeaderTabTextFontFamily
        public string HeaderTabTextFontFamily
        {
            get { return (string)GetValue(HeaderTabTextFontFamilyProperty); }
            set { SetValue(HeaderTabTextFontFamilyProperty, value); }
        }
        private static void HeaderTabTextFontFamilyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl.ItemSource != null)
            {
                foreach (var tab in tabViewControl.ItemSource)
                {
                    tab.HeaderTabTextFontFamily = (string)newValue;
                }
            }
        }
        public static readonly BindableProperty HeaderTabTextFontFamilyProperty = BindableProperty.Create(nameof(HeaderTabTextFontFamily), typeof(string), typeof(TabViewControl), null, BindingMode.Default, null, HeaderTabTextFontFamilyChanged);
        #endregion

        #region HeaderTabTextFontAttributes
        public FontAttributes HeaderTabTextFontAttributes
        {
            get { return (FontAttributes)GetValue(HeaderTabTextFontAttributesProperty); }
            set { SetValue(HeaderTabTextFontAttributesProperty, value); }
        }
        private static void HeaderTabTextFontAttributesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl.ItemSource != null)
            {
                foreach (var tab in tabViewControl.ItemSource)
                {
                    tab.HeaderTabTextFontAttributes = (FontAttributes)newValue;
                }
            }
        }
        public static readonly BindableProperty HeaderTabTextFontAttributesProperty = BindableProperty.Create(nameof(HeaderTabTextFontAttributes), typeof(FontAttributes), typeof(TabViewControl), FontAttributes.None, BindingMode.Default, null, HeaderTabTextFontAttributesChanged);
        #endregion

        #region ItemSource
        public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(nameof(ItemSource), typeof(ObservableCollection<TabItem>), typeof(TabViewControl));
        public ObservableCollection<TabItem> ItemSource
        {
            get => (ObservableCollection<TabItem>)GetValue(ItemSourceProperty);
            set { SetValue(ItemSourceProperty, value); }
        }
        #endregion

        #region TabSizeOption
        public static readonly BindableProperty TabSizeOptionProperty = BindableProperty.Create(nameof(TabSizeOption), typeof(GridLength), typeof(TabViewControl), default(GridLength), propertyChanged: OnTabSizeOptionChanged);
        private static void OnTabSizeOptionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl._headerContainerGrid != null && tabViewControl.ItemSource != null)
            {
                foreach (var tabContainer in tabViewControl._headerContainerGrid.ColumnDefinitions)
                {
                    tabContainer.Width = (GridLength)newValue;
                }
            }
        }
        public GridLength TabSizeOption
        {
            get => (GridLength)GetValue(TabSizeOptionProperty);
            set { SetValue(TabSizeOptionProperty, value); }
        }
        #endregion

        #region SelectedTabIndex
        public static readonly BindableProperty SelectedTabIndexProperty = BindableProperty.Create(nameof(SelectedTabIndex), typeof(int), typeof(TabViewControl), 0, propertyChanged: OnSelectedTabIndexChanged);
        private static void OnSelectedTabIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabViewControl tabViewControl && tabViewControl.ItemSource != null &&
                tabViewControl._carouselView.Position != (int)newValue)
            {
                tabViewControl.SetPosition((int)newValue, true);
            }
        }
        public int SelectedTabIndex
        {
            get => (int)GetValue(SelectedTabIndexProperty);
            set { SetValue(SelectedTabIndexProperty, value); }
        }
        #endregion

        public void SetPosition(int position, bool forced = false)
        {
            if (SelectedTabIndex == position && !forced || ItemSource.Count == 0)
            {
                return;
            }
            int oldPosition = SelectedTabIndex;

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

            if ((position >= 0 && position < ItemSource.Count) || forced)
            {
                if (_carouselView.Position != position || forced)
                {
                    _carouselView.PositionSelected -= _carouselView_PositionSelected;
                    _carouselView.Position = position;
                    _carouselView.PositionSelected += _carouselView_PositionSelected;
                }
                if (oldPosition != position || forced)
                {
                    for (int i = 0; i < ItemSource.Count; i++)
                    {
                        ItemSource[i].IsCurrent = i == position;
                    }

                    if (ItemSource[position].Content?.BindingContext is ITabViewControlTabItem tabViewItem)
                    {
                        tabViewItem.TabViewControlTabItemFocus();
                    }
                    SelectedTabIndex = position;

                    Device.BeginInvokeOnMainThread(async () => await _tabHeadersContainerSv.ScrollToAsync(_headerContainerGrid.Children[position], ScrollToPosition.MakeVisible, false));
                }
            }

            var positionChangedArgs = new PositionChangedEventArgs()
            {
                NewPosition = SelectedTabIndex,
                OldPosition = oldPosition
            };
            OnPositionChanged(positionChangedArgs);
        }

        public void SelectNext()
        {
            SetPosition(SelectedTabIndex + 1);
        }

        public void SelectPrevious()
        {
            SetPosition(SelectedTabIndex - 1);
        }

        public void SelectFirst()
        {
            SetPosition(0);
        }

        public void SelectLast()
        {
            SetPosition(ItemSource.Count - 1);
        }

        public void AddTab(TabItem tab, int position = -1, bool selectNewPosition = false)
        {
            if (position > -1)
            {
                ItemSource.Insert(position, tab);
            }
            else
            {
                ItemSource.Add(tab);
            }
            if (selectNewPosition)
            {
                SelectedTabIndex = position;
            }
        }

        public void RemoveTab(int position = -1)
        {
            if (position > -1)
            {
                ItemSource.RemoveAt(position);
                _headerContainerGrid.Children.RemoveAt(position);
                _headerContainerGrid.ColumnDefinitions.RemoveAt(position);
            }
            else
            {
                ItemSource.Remove(ItemSource.Last());
                _headerContainerGrid.Children.RemoveAt(_headerContainerGrid.Children.Count - 1);
                _headerContainerGrid.ColumnDefinitions.Remove(_headerContainerGrid.ColumnDefinitions.Last());
            }
            _carouselView.ItemsSource = ItemSource.Select(t => t.Content);
            SelectedTabIndex = position >= 0 && position < ItemSource.Count ? position : ItemSource.Count - 1;
        }
    }
}
