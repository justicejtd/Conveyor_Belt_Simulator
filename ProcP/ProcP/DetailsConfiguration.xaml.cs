using ProcP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Xml.Serialization;

namespace ProcP
{

    /// <summary>
    /// Interaction logic for DetailsConfiguration.xaml
    /// </summary>
    public partial class DetailsConfiguration : Window
    {
        private InputPage inputPage;
        public DetailsConfiguration()
        {
            InitializeComponent();
            inputPage = new InputPage();
            mainFrame.Navigate(inputPage);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (inputPage.pageSimulation != null)
            {
                inputPage.pageSimulation.StopSimulation();
            }
        }

        private void CloseWindowClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
