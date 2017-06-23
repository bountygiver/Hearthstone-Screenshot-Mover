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

namespace HSScreenshotMover
{
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {
        public String NewName
        {
            private set;
            get;
        }

        public RenameDialog()
        {
            InitializeComponent();
        }

        public void InitFile(string OldName)
        {
            lblLabel.Content = "Rename: " + OldName;
        }

        private void OK_Clicked(object sender, RoutedEventArgs e)
        {
            NewName = txtNewName.Text;
            if (!NewName.EndsWith(".png"))
            {
                NewName = NewName + ".png";
            }
            this.DialogResult = true;
            Close();
        }

        private void Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            NewName = "";
            this.DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtNewName.Focus();
        }
    }
}
