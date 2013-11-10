using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HOAPro.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for FullScreenImage.xaml
    /// </summary>
    public partial class FullScreenImage : Window
    {
        public FullScreenImage(string imageFilePath)
        {
            InitializeComponent();
            var uriSource = new Uri(imageFilePath, UriKind.Absolute);
            imgMain.Source = new BitmapImage(uriSource);
        }
    }
}
