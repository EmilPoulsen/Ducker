using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ducker.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int InsertBindingHere { get; set; } = 50;

        public MainWindow()
        {
            InitializeComponent();
            double scale = 0.3;
            this.Width = IPhoneXWidth(scale);
            this.Height = IPhoneXHeigh(scale);
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Title = string.Format("Ducker {0}", assemblyVersion);
            cmbColors.ItemsSource = typeof(Colors).GetProperties();
            this.pbStatus.Value = 50;
        }

        private int IPhoneXWidth(double scale)
        {
            double width =  1125 * scale;
            return (int) width;
        }

        private int IPhoneXHeigh(double scale)
        {
            double height = 2436 * scale;
            return (int)height;
        }

        private void btnSetPath_Click(object sender, RoutedEventArgs e)
        {
            // do something
        }
    }
}
