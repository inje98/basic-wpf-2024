using ex04_wpf_bikeshop;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ex04_wpf_bikeshop
{
    /// <summary>
    /// ContactPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ContactPage : Page
    {
        public ContactPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Bike myBike = new Bike();
            myBike.speed = 60;
            myBike.color = Colors.Red;

            TextBox text1 = new TextBox();
            StpBike.DataContext = myBike;
            
        }

        //private void SldValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    PrgValue.Value = (double)e.NewValue;
        //    LblValue.Content = Math.Round(PrgValue.Value, 1); // Math.Round() 소수점 몇째자리까지 나올건지 지정가능
        //}
    }
}
