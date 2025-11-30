using OptiTrack.App.UI.ctrls;
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
            var addWindow = new AddEmployeeWindow();
            addWindow.Show();
        }

        private void ManageSetup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewLogs_Click(object sender, RoutedEventArgs e)
        {
            var win = new ViewAllLogs();
            win.Owner = this;
            win.ShowDialog();
        }

        private void ManageOvertime_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GenerateReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewEmployees_Click(object sender, RoutedEventArgs e)
        {
            var win = new ManageEmployees();
            win.Owner = this;
            win.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }

        private void ViewAllLogs_Click(object sender, RoutedEventArgs e)
        {
            var win = new ViewAllLogs();
            win.Owner = this;
            win.ShowDialog();
        }
    }
}
