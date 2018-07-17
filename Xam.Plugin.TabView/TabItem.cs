using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xam.Plugin.TabView
{
    [ContentProperty(nameof(Content))]
    public class TabItem : BindableObject
    {
        public TabItem()
        {
            //Parameterless constructor required for xaml instantiation.
        }

        public TabItem(string headerText, View content, ImageSource headerIcon = null)
        {
            HeaderText = headerText;
            Content = content;
            if (headerIcon != null)
                HeaderIcon = headerIcon;
        }

        public static readonly BindableProperty HeaderIconProperty = BindableProperty.Create(nameof(HeaderIcon), typeof(ImageSource), typeof(TabItem));
        public ImageSource HeaderIcon
        {
            get => (ImageSource)GetValue(HeaderIconProperty);
            set { SetValue(HeaderIconProperty, value); }
        }

        public readonly BindableProperty HeaderIconSizeProperty = BindableProperty.Create(nameof(HeaderIconSize), typeof(double), typeof(TabItem), 32.0);
        public double HeaderIconSize
        {
            get => (double)GetValue(HeaderIconSizeProperty);
            set { SetValue(HeaderIconSizeProperty, value); }
        }

        public static readonly BindableProperty HeaderTextProperty = BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(TabItem), string.Empty);
        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set { SetValue(HeaderTextProperty, value); }
        }

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(TabItem));
        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set { SetValue(ContentProperty, value); }
        }

        public static readonly BindableProperty IsCurrentProperty = BindableProperty.Create(nameof(IsCurrent), typeof(bool), typeof(TabItem), false);
        public bool IsCurrent
        {
            get => (bool)GetValue(IsCurrentProperty);
            set { SetValue(IsCurrentProperty, value); }
        }

        public static readonly BindableProperty HeaderTextColorProperty = BindableProperty.Create(nameof(HeaderTextColor), typeof(Color), typeof(TabItem), Color.White);
        public Color HeaderTextColor
        {
            get => (Color)GetValue(HeaderTextColorProperty);
            set { SetValue(HeaderTextColorProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineColorProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineColor), typeof(Color), typeof(TabItem), Color.White);
        public Color HeaderSelectionUnderlineColor
        {
            get => (Color)GetValue(HeaderSelectionUnderlineColorProperty);
            set { SetValue(HeaderSelectionUnderlineColorProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineThicknessProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineThickness), typeof(double), typeof(TabItem), (double)5);
        public double HeaderSelectionUnderlineThickness
        {
            get => (double)GetValue(HeaderSelectionUnderlineThicknessProperty);
            set { SetValue(HeaderSelectionUnderlineThicknessProperty, value); }
        }

        public static readonly BindableProperty HeaderSelectionUnderlineWidthProperty = BindableProperty.Create(nameof(HeaderSelectionUnderlineWidth), typeof(double), typeof(TabItem), (double)40);
        public double HeaderSelectionUnderlineWidth
        {
            get => (double)GetValue(HeaderSelectionUnderlineWidthProperty);
            set { SetValue(HeaderSelectionUnderlineWidthProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontSizeProperty = BindableProperty.Create(nameof(HeaderTabTextFontSize), typeof(double), typeof(TabItem), TabDefaults.DefaultTextSize);
        [TypeConverter(typeof(FontSizeConverter))]
        public double HeaderTabTextFontSize
        {
            get => (double)GetValue(HeaderTabTextFontSizeProperty);
            set { SetValue(HeaderTabTextFontSizeProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontFamilyProperty = BindableProperty.Create(nameof(HeaderTabTextFontFamily), typeof(string), typeof(TabItem));
        public string HeaderTabTextFontFamily
        {
            get => (string)GetValue(HeaderTabTextFontFamilyProperty);
            set { SetValue(HeaderTabTextFontFamilyProperty, value); }
        }

        public static readonly BindableProperty HeaderTabTextFontAttributesProperty = BindableProperty.Create(nameof(HeaderTabTextFontAttributes), typeof(FontAttributes), typeof(TabItem), FontAttributes.None);
        public FontAttributes HeaderTabTextFontAttributes
        {
            get => (FontAttributes)GetValue(HeaderTabTextFontAttributesProperty);
            set { SetValue(HeaderTabTextFontAttributesProperty, value); }
        }
    }
}
