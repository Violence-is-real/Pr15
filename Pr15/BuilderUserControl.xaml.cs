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
            lstCategories.ItemsSource = _categories.Keys.ToList();

            //Загрузка данных
            FilterParts();
        }
        private void FilterParts()
        {
            var query = Core.Context.basepart_
                .Include("manufacturer_")
                .Include("parttype_")
                .AsQueryable();

            //Фильтрация (категория)
            if (lstCategories.SelectedItem is string selectedCategory &&
                _categories.TryGetValue(selectedCategory, out int typeId))
            {
                query = query.Where(p => p.parttypeid == typeId);
            }
            //Фильтрация (Производитель)
            if (cmbManufacturer.SelectedItem is manufacturer_ man && man.id != 0)
            { 
                query = query.Where(p => p.manufacturerid == man.id);
            }
            // Поиск по названию
            string search = txtSearch.Text?.Trim().ToLower();
            if (!string.IsNullOrEmpty(search)) 
            { 
                query = query.Where(p => p.name.ToLower().Contains(search)); 
            }
            lvParts.ItemsSource = query.ToList();
        }
        //Обработка фильтров 
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => FilterParts();
        private void cmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e) => FilterParts();
        private void lstCategories_SelectionChanged(object sender, SelectionChangedEventArgs e) => FilterParts();
        private string CheckCompatibility()
        {
            if (_selectedParts.Count == 0) return "Выберите комплектующие для проверки";

            var cpu = _selectedParts.FirstOrDefault(p => p.parttypeid == 1);
            var mb = _selectedParts.FirstOrDefault(p => p.parttypeid == 4);
            var cooler = _selectedParts.FirstOrDefault(p => p.parttypeid == 7);
            var ram = _selectedParts.FirstOrDefault(p => p.parttypeid == 3);
            var casep = _selectedParts.FirstOrDefault(p => p.parttypeid == 5);
            var psu = _selectedParts.FirstOrDefault(p => p.parttypeid == 6);
            var gpu = _selectedParts.FirstOrDefault(p => p.parttypeid == 2);

            List<string> issues = new List<string>();

            // Сокет CPU + Мать + Кулер
            if (cpu != null && mb != null)
            {
                var cpuDb = Core.Context.cpu_.Find(cpu.id);
                var mbDb = Core.Context.motherboard_.Find(mb.id);
                if (cpuDb?.socketid != mbDb?.socketid)
                    issues.Add("Сокет процессора не совпадает с сокетом материнской платы");
            }

            if (cpu != null && cooler != null)
            {
                var cpuDb = Core.Context.cpu_.Find(cpu.id);
                bool compatible = Core.Context.socketprocessorcooler_
                    .Any(s => s.socketid == cpuDb.socketid && s.processorcoolerid == cooler.id);
                if (!compatible)
                    issues.Add("Кулер не поддерживает сокет процессора");
            }

            // Форм-фактор матери + Корпус
            if (mb != null && casep != null)
            {
                var mbDb = Core.Context.motherboard_.Find(mb.id);
                bool ok = Core.Context.boardformfactorcase_
                    .Any(b => b.caseid == casep.id && b.formfactorid == mbDb.formfactorid);
                if (!ok)
                    issues.Add("Форм-фактор материнской платы не подходит к корпусу");
            }

            // Тип памяти
            if (mb != null && ram != null)
            {
                var mbDb = Core.Context.motherboard_.Find(mb.id);
                var ramDb = Core.Context.ram_.Find(ram.id);
                if (mbDb?.memorytypeid != ramDb?.memorytypeid)
                    issues.Add("Тип памяти не совпадает с поддержкой материнской платы");
            }

            // Мощность бл. питания
            if (gpu != null && psu != null)
            {
                var gpuDb = Core.Context.gpu_.Find(gpu.id);
                var psuDb = Core.Context.powersupply_.Find(psu.id);
                if (psuDb.power < (gpuDb.recommendpower ?? 0))
                    issues.Add($"Блок питания слишком слабый (рекомендуется минимум {gpuDb.recommendpower} Вт)");
            }

            return issues.Count == 0
                ? "Комплектующие полностью совместимы"
                : $"Обнаружены проблемы совместимости:\n• {string.Join("\n• ", issues)}";
        }
        //Обновление интерфейса
        private void UpdateUI()
        {
            decimal total = _selectedParts.Sum(p => p.price);
            txtTotalPrice.Text = $"{total:N0} ₽";

            txtCompatibility.Text = CheckCompatibility();
            txtCompatibility.Foreground = txtCompatibility.Text.Contains("✅")
                ? System.Windows.Media.Brushes.DarkGreen
                : System.Windows.Media.Brushes.Red;

            btnSave.IsEnabled = _selectedParts.Count > 0;
        }
        private void lvParts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            basepart_ part = lvParts.SelectedItem as basepart_;
            if (part == null) return;

            bool alreadyAdded = false;
            foreach (var p in _selectedParts)
            {
                if (p.id == part.id)
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (alreadyAdded)
            {
                MessageBox.Show("Это комплектующее уже добавлено в сборку!", "Внимание");
                return;
            }

            _selectedParts.Add(part);
            UpdateUI();
        }

        private void lvSelected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvSelected.SelectedItem is basepart_ part)
            {
                _selectedParts.Remove(part);
                UpdateUI();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lvSelected.SelectedItem is basepart_ part)
            {
                _selectedParts.Remove(part);
                UpdateUI();
            }
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _selectedParts.Clear();
            UpdateUI();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAssemblyName.Text))
            {
                MessageBox.Show("Введите название сборки!", "Ошибка");
                return;
            }
                var assembly = new assembly_
                {
                    name = txtAssemblyName.Text.Trim(),
                    author = txtAuthor.Text.Trim()
                }
                ;

                Core.Context.assembly_.Add(assembly);
                Core.Context.SaveChanges();

                foreach (var part in _selectedParts)
                {
                    Core.Context.partassembly_.Add(new partassembly_
                    {
                        partid = part.id,
                        assemblyid = assembly.id
                    });
                }

                Core.Context.SaveChanges();

                MessageBox.Show($"Сборка «{assembly.name}» успешно сохранена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                _selectedParts.Clear();
                txtAssemblyName.Clear();
                UpdateUI();              
        }
    }
}

