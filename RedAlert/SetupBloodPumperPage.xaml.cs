using RedAlert.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RedAlert
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class SetupBloodPumperPage : Page
    {
        SetupBloodPumpVM VM => (SetupBloodPumpVM)DataContext;

        public SetupBloodPumperPage()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender.Equals(bedNameTextBox))
                VM.BloodPumper.BedName = bedNameTextBox.Text;
            else
            {
                if (int.TryParse(usesBeforeDeathTextBox.Text, out int value))
                    VM.BloodPumper.UsesBeforeDeath = value;
                else
                    VM.BloodPumper.UsesBeforeDeath = 1; // Assign 1 as default
            }
        }
    }
}
