using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabViewSample.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TabViewSample.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TemplatedItemSourceSamplePage : ContentPage
    {
        public TemplatedItemSourceSamplePageViewModel PageViewModel;
        public TemplatedItemSourceSamplePage()
        {
            InitializeComponent();

            PageViewModel = new TemplatedItemSourceSamplePageViewModel();

            BindingContext = PageViewModel;
        }
    }
}