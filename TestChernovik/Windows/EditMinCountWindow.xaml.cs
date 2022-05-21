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

namespace TestChernovik.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditMinCountWindow.xaml
    /// </summary>
    public partial class EditMinCountWindow : Window
    {
        public EditMinCountWindow(int max)
        {
            InitializeComponent();
            txtMinCount.Text = max.ToString();
        }

        public int Result;
        private void btnEditMinCount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Result = Convert.ToInt32(txtMinCount.Text);
                
                DialogResult = true;

            }
            catch (Exception)
            {
                MessageBox.Show("Минимальное количество должно быть целым числом!");
            }
        }
    }
}
