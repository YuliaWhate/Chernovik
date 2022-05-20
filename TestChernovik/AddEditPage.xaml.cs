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

namespace TestChernovik
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        Materials materials = new Materials();
        public AddEditPage(Materials selectedMaterials)
        {
            InitializeComponent();

            if (selectedMaterials != null)
            {
                materials = selectedMaterials;

                btnDeleteMaterial.Visibility = Visibility.Visible;
                DGSupplier.ItemsSource = materials.Supplier.ToList();
            }
                
            DataContext = materials;
            comboBoxType.ItemsSource = bazaEntities.GetContext().MaterialType.ToList();
            comboBoxUnit.ItemsSource = UnitList;
            comboBoxSupplier.ItemsSource = bazaEntities.GetContext().Supplier.ToList();
        }

        private string[] UnitList =
        {
            "кг",
            "л",
            "м"
        };
        private void btnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            DGSupplier.Items.Add(comboBoxSupplier.SelectedItem);
        }
        private void btnDeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            DGSupplier.Items.Remove(DGSupplier.SelectedItem);
        }
        private void btnSaveMaterial_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(materials.Title))
                errors.AppendLine("Укажите название материала");
            if (materials.MinCount < 0)
                errors.AppendLine("Минимальное количество не может быть отрицательной");
            if (materials.Cost < 0)
                errors.AppendLine("Стоимость не может быть отрицательной");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (materials.ID == 0)
                bazaEntities.GetContext().Materials.Add(materials);
            try
            {
                bazaEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        private void btnDeleteMaterial_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"Вы действительно хотите удалить {materials.Title}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    bazaEntities.GetContext().Materials.Remove(materials);
                    bazaEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    NavigationService.GoBack();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
