using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TabViewSample
{
    public partial class XamlSamplePage : ContentPage
    {
        public XamlSamplePage()
        {
            InitializeComponent();
        }

        async void GoBackClicked(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
