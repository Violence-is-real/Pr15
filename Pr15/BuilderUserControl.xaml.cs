using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Pr15
{
    /// <summary>
    /// Логика взаимодействия для BuilderUserControl.xaml
    /// </summary>
    public partial class BuilderUserControl : UserControl
    {
        private ObservableCollection<basepart_> _selectedParts = new ObservableCollection<basepart_>();

        private readonly Dictionary<string, int> _categories = new Dictionary<string, int>
        {
            { "Процессор", 1 },
            { "Видеокарта", 2 },
            { "Оперативная память", 3 },
            { "Материнская плата", 4 },
            { "Корпус", 5 },
            { "Блок питания", 6 },
            { "Кулер для процессора", 7 },
            { "Накопитель", 8 }
        };  
        public BuilderUserControl()
        {
            InitializeComponent();
            LoadData();

        }
        private void LoadData()
        {
            //Производители
            cmbManufacturer.ItemsSource = Core.Context.manufacturer_.OrderBy(m => m.name).ToList();
            cmbManufacturer.SelectedIndex = 0;

            //Категории
            lstCategories.ItemsSource = _categories.Keys.ToList();;
        }
    }
}
