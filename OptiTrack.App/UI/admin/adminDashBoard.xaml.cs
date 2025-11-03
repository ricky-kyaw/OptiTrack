using OptiTrack.App.UI.general;
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
using System.Windows.Shapes;

namespace OptiTrack.App.UI.admin
{
    /// <summary>
    /// Interaction logic for adminDashBoard.xaml
    /// </summary>
    public partial class adminDashBoard : Window
    {
        public adminDashBoard()
        {
            InitializeComponent();
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ManageSetup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewLogs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ManageOvertime_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GenerateReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewEmployees_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
        }

        private void ViewAllLogs_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement logic to view all logs
            MessageBox.Show("View All Logs clicked!");
        }
    }
}
