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

namespace ParentsListUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String[] str = {"/c start powershell.exe -NoExit", "ps1 location",
        "-csvPath", "csvPath",
        "-distributionListName", "\'CHANGE ME\'",
        "-addNames", "$TRUE",
        "-removeNames", "$FALSE",
        "-testing", "$FALSE"};

        public MainWindow() {
            InitializeComponent();
        }

        private void beginbutton_Click(object sender, RoutedEventArgs e) {
            string warning = WarningBuilder();
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(warning, "Are You Sure?", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) {
                string args = CreateArgs("");
                System.Diagnostics.Process runThis = new System.Diagnostics.Process();
                runThis.StartInfo.FileName = "cmd.exe";
                runThis.StartInfo.Arguments = args;
                runThis.StartInfo.Verb = "runas";
                runThis.Start();
            }
        }

        private string WarningBuilder() {
            string distlist = str[5].Remove(str[5].Length - 1, 1).Remove(0, 1);
            string[] addremove = new string[2] { "", ""};
            if (str[7] == "$TRUE" && str[9] == "$FALSE") addremove = new string[2] { "add", "to"};
            else if (str[7] == "$FALSE" && str[9] == "$TRUE") addremove = new string[2] { "remove", "from" };

            return "WARNING: You are about to " + addremove[0] + " the contents of:\n" + str[3] + 
                "\n" + addremove[1] + " the distribution list: " + distlist + ".\n\nAre you ABSOLUTELY POSITIVE you want to do this?";
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "csv files (*.csv)|*.csv";

            if (dlg.ShowDialog() == true) {
                str[3] = dlg.FileName;
                csvPathText.Text = str[3];
                str[1] = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "ListUpdater.ps1";
                UpdateCurrentCommand();
                BeginButton.IsEnabled = true;
            }
        }

        private void UpdateCurrentCommand() {
            currentPSCommandText.Text = CreateArgs("cmd.exe ");
        }

        private string CreateArgs(string startingString) {
            string args = startingString;
            foreach (string s in str) {
                args = args + s + " ";
            }
            return args;
        }

        private void addButton_Checked(object sender, RoutedEventArgs e) {
            str[7] = "$TRUE";
            str[9] = "$FALSE";
            UpdateCurrentCommand();
        }

        private void removeButton_Checked(object sender, RoutedEventArgs e) {
            str[7] = "$FALSE";
            str[9] = "$TRUE";
            UpdateCurrentCommand();
        }

        private void CustomListText_TextChanged(object sender, TextChangedEventArgs e) {
                str[5] = "\'" + CustomListText.Text + "\'";
                UpdateCurrentCommand();
        }

        private void whatIf_Checked(object sender, RoutedEventArgs e) {
            str[11] = "$TRUE";
            UpdateCurrentCommand();
        }

        private void whatIf_Unchecked(object sender, RoutedEventArgs e) {
            str[11] = "$FALSE";
            UpdateCurrentCommand();
        }
    }
}
