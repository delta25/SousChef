using CsvHelper;
using Newtonsoft.Json;
using SousChef.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SousChef.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConvertersPage : Page
    {
        private string defaultImage = "ms-appx:///Assets/convert.png";
        private string selectedConverterKey;
        private Converter selectedConverter;
        private List<Converter> availableConvertersForUnitGroup;
        private Dictionary<string, List<Converter>> availableUnitGroupedConverters;

        public ConvertersPage()
        {
            this.InitializeComponent();

            //PopulateConverterOptions();
            LoadConvertersAsync();
        }

        private async Task LoadConvertersAsync()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resources/Converters.csv"));
            using (var inputStream = await file.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                var csv = new CsvReader(streamReader);
                csv.Read();
                csv.ReadHeader();
                var records = csv.GetRecords<Converter>();
                availableUnitGroupedConverters = records.GroupBy(x => x.UnitGroup).ToDictionary(x => x.Key, x => x.ToList());
                converterListing.ItemsSource = new List<string>(this.availableUnitGroupedConverters.Keys);
            }
        }

        private void SelectedUnitGroupChanged(object sender, SelectionChangedEventArgs e)
        {
            availableConvertersForUnitGroup = availableUnitGroupedConverters[selectedConverterKey];
            selectedConverter = availableConvertersForUnitGroup.FirstOrDefault();
            defaultImage = "ms-appx:///Assets/weight.png";

            leftImage.UriSource = new Uri(selectedConverter.LeftIcon);
            rightImage.UriSource = new Uri(selectedConverter.RightIcon);

        }
    }

}