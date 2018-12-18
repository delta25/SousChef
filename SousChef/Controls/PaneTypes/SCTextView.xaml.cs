using SousChef.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SousChef.Controls
{
    public sealed partial class SCTextView : RecipePane
    {
        public SCTextView()
        {
            this.InitializeComponent();

            genericPane.PaneClosingEvent += (sender, args) => PaneClosingEvent(this.paneId);
        }

        public override PaneCache GetCacheValues()
        {
            string text;
            richEditBox.Document.GetText(TextGetOptions.FormatRtf, out text);
            return new SCTextViewPaneCache()
            {
                Id = this.paneId,
                TextDocument = text
            };
        }

        public override void RestoreFromCache(PaneCache paneCache)
        {
            var currentPaneCache = (SCTextViewPaneCache)paneCache;
            this.paneId = paneCache.Id;

            richEditBox.Document.SetText(TextSetOptions.FormatRtf, currentPaneCache.TextDocument);

            PaneFinishedRestoringEvent(paneCache.Id);
        }

    }
}
