using ProcP.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;

namespace ProcP
{
    /// <summary>
    /// Interaction logic for InputPage.xaml
    /// </summary>
    public partial class InputPage : Page
    {
        public Simulation pageSimulation;
        private CurrentState state = new CurrentState();
        private List<Belt> belts = new List<Belt>();
        private string ex_file_name = "";

        public InputPage()
        {
            InitializeComponent();
            loadConfig();
        }
        private void loadConfig()
        {
            string[] args = Environment.GetCommandLineArgs(); // if the application has been started by a file then we have arguments
            if (args.Length == 2)
            {
                if (System.IO.File.Exists(args[1])) // check whether we actually have a file
                {
                    string filePath = args[1];
                    XmlSerializer ser = new XmlSerializer(typeof(CurrentState));
                    using (FileStream fs = File.OpenRead(System.IO.Path.GetFileName(filePath)))
                    {
                        ex_file_name = System.IO.Path.GetFileName(filePath).ToString();
                        state = (CurrentState)ser.Deserialize(fs);
                    }
                    tbxFlights.Text = state.Flights;
                    tbEmployees.Text = state.Employees;
                    tbCarts.Text = state.Carts;
                    belts = state.Belts;
                }
            }
        }

        private void writeConfig()
        {
            state.Flights = tbxFlights.Text;
            state.Carts = tbCarts.Text;
            state.Employees = tbEmployees.Text;
        }

        private void LoadSimulation(object sender, RoutedEventArgs e)
        {
            //int nrOfFlights = (int)this.tbxFlights.Text
            try
            {
                if (tbCarts.Text != "" && tbEmployees.Text != "" && tbxFlights.Text != "")
                {
                    int carts = int.Parse(tbCarts.Text);
                    int nrOfEmployees = int.Parse(tbEmployees.Text);
                    int nrOfFlights = int.Parse(tbxFlights.Text);
                    writeConfig();
                    //  var c = this.Content;
                    pageSimulation = new Simulation(carts, nrOfEmployees, nrOfFlights, belts, state, ex_file_name);
                    NavigationService.Navigate(pageSimulation);
                }
                else
                {
                    MessageBox.Show("Select number of items");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Please enter only numbers!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
