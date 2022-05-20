using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.IO;
using TestChernovik.Windows;

namespace TestChernovik
{
    class Core
    {
        public static bazaEntities DB = new bazaEntities();
    }
    public partial class Materials
    {

        public Uri ImagePreview
        {
            get
            {
                string[] paths = { @"C:\Users\yulia\Desktop\TestChernovik\TestChernovik\bin\Debug", Image };
                var image = string.Concat(paths);
                return File.Exists(image) ? new Uri(image) : new Uri("pack://application:,,,/Images/picture.png");
            }
        }

        public string SupplierList
        {
            get
            {
                var Result = "";
                foreach (var ms in Supplier)
                {
                    Result += (Result == "" ? "" : ", ") + ms.Title;
                }
                return Result;
            }
        }

        public string BackgroundColor
        {
            get
            {
                if(CountInStock < MinCount)
                    return "#f19292";
                if (CountInStock == MinCount * 3)
                    return "#ffba01";
                return "#fff";
            }
        }
    }

    /// <summary>
    /// Логика взаимодействия для MaterialsPage.xaml
    /// </summary>
    public partial class MaterialsPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private IEnumerable<Materials> _MaterialsList;

        private int SortType = 0;
        public string[] SortList { get; set; } =
        {
            "Без сортировки",
            "Наименование по возрастанию",
            "Наименование по убыванию",
            "Остаток на складе по возрастанию",
            "Остаток на складе по убыванию",
            "Стоимость по возрастанию",
            "Стоимость по убыванию"
        };

        private int FilterType = 0;
        public string[] FilterList { get; set; } =
        {
            "Все типы",
            "Гранулы",
            "Пресс",
            "Нарезка",
            "Рулон"
        };

        private int _currentPage = 1;
        public int CurrentPage
        {
            get 
            {
                return _currentPage; 
            }
            set
            {

                 _currentPage = value;
                 Invalidate();
            }
        }

        private void Invalidate()
        {
            /*if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(ComponentName));*/
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("MaterialsList"));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentPage"));
        }

        public IEnumerable<Materials> MaterialsList
        {
            get
            {
                var Result = _MaterialsList;
                
                switch(SortType)
                {
                    case 1:
                        Result = Result.OrderBy(p => p.Title);
                        break;
                    case 2:
                        Result = Result.OrderByDescending(p => p.Title);
                        break;
                    case 3:
                        Result = Result.OrderBy(p =>p.CountInStock);
                        break;
                    case 4:
                        Result = Result.OrderByDescending(p => p.CountInStock);
                        break;
                    case 5:
                        Result = Result.OrderBy(p => p.Cost);
                        break;
                    case 6:
                        Result = Result.OrderByDescending(p => p.Cost);
                        break;
                }
                
                switch(FilterType)
                {
                    case 1:
                        Result = Result.Where(p => p.MaterialType.Title == "Гранулы");
                        break;
                    case 2:
                        Result = Result.Where(p => p.MaterialType.Title == "Пресс");
                        break;
                    case 3:
                        Result = Result.Where(p => p.MaterialType.Title == "Нарезка");
                        break;
                    case 4:
                        Result = Result.Where(p => p.MaterialType.Title == "Рулон");
                        break;
                }

                Result = Result.Where(p => p.Title.ToLower().Contains(TextBoxSearch.Text.ToLower())).ToArray();
                if(Result.Count() == 0)
                {
                    MessageBox.Show("Результаты поиска отсутствуют!");
                }

                Paginator.Children.Clear();

                Paginator.Children.Add(new TextBlock { Text = " < " });
                for (int i = 1; i < (Result.Count() / 15)+1; i++)
                    Paginator.Children.Add(new TextBlock { Text = " " + i.ToString() + " " });
                Paginator.Children.Add(new TextBlock { Text = " > " });
                foreach (TextBlock tb in Paginator.Children)
                    tb.PreviewMouseDown += btnBack_PreviewMouseDown;

                if(CurrentPage > Result.Count() / 15)
                    CurrentPage = Result.Count() / 15;

                txtResultCount.Text = Result.ToList().Count.ToString();
                return Result.Skip((CurrentPage-1)*15).Take(15);
                
            }
            set
            {
                _MaterialsList = value;
                //Invalidate();
            }
        }

        public MaterialsPage()
        {
            InitializeComponent();
            DataContext = this;
            MaterialsList = Core.DB.Materials.ToList();    
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPage(null));
        }

        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterType = ComboBoxFilter.SelectedIndex;
            Invalidate();
        }

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortType = ComboBoxSort.SelectedIndex;
            Invalidate();
        }

        private void TextBoxSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Invalidate();
        }

        private void btnBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch((sender as TextBlock).Text)
            {
                case " < ":
                    if (CurrentPage > 1) CurrentPage--;
                    return;
                case " > ":
                    if (CurrentPage < _MaterialsList.Count() / 15) CurrentPage++;
                    return;
                default:
                    CurrentPage = Convert.ToInt32((sender as TextBlock).Text.Trim());
                    return;
            }
        }

        public int MaterialsSelectedCount = 0;
        private void LViewMaterials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MaterialsSelectedCount = LViewMaterials.SelectedItems.Count;
            //Invalidate("MinCountBtnVisisble");
        }

        public string MinCountBtnVisisble
        {
            get
            {
                if (MaterialsSelectedCount > 1) return "Visible";
                return "Collapsed";
            }
        }

        private void btnEditMinCount_Click(object sender, RoutedEventArgs e)
        {
            int max = 0;
            List<int> idList = new List<int>();
            foreach(Materials item in LViewMaterials.SelectedItems)
            {
                if(item.MinCount > max)
                    max = item.MinCount;
                idList.Add(item.ID);
            }

            var NewWindow = new EditMinCountWindow(max);
            NewWindow.ShowDialog();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                bazaEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                MaterialsList = bazaEntities.GetContext().Materials.ToList();

                txtMaterialCount.Text = _MaterialsList.Count().ToString();
                
            }
        }

        private void LViewMaterials_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new AddEditPage(LViewMaterials.SelectedItem as Materials));
        }
    }
}
