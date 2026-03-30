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

namespace Pr15
{
    /// <summary>
    /// Логика взаимодействия для AssembliesUserControl.xaml
    /// </summary>
    public partial class AssembliesUserControl : UserControl
    {
        public AssembliesUserControl()
        {
            InitializeComponent();
            LoadAssemblies();
        }
        public class PartDisplay
        {
            public string name { get; set; }
            public string manufacturer { get; set; }
            public decimal price { get; set; }
        }
        private void LoadAssemblies()
        {
                // Загружаем все сборки с комплектующими
                var assemblies = Core.Context.assembly_
                    .Include("partassembly_")
                    .Include("partassembly_.basepart_")
                    .Include("partassembly_.basepart_.manufacturer_")
                    .ToList();

                lvAssemblies.ItemsSource = assemblies;
        }
        private void lvAssemblies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvAssemblies.SelectedItem == null)
            {
                lvAssemblyParts.ItemsSource = null;
                return;
            }

            assembly_ selectedAssembly = lvAssemblies.SelectedItem as assembly_;
            if (selectedAssembly == null) return;

            // Загружаем комплектующие выбранной сборки
            var parts = Core.Context.partassembly_
                .Include("basepart_")
                .Include("basepart_.manufacturer_")
                .Where(pa => pa.assemblyid == selectedAssembly.id)
                .ToList();

            // Для отображения в ListView создаём удобный список
            var displayList = new List<PartDisplay>();
            foreach (var pa in parts)
            {
                if (pa.basepart_ != null)
                {
                    displayList.Add(new PartDisplay
                    {
                        name = pa.basepart_.name,
                        manufacturer = pa.basepart_.manufacturer_ != null ? pa.basepart_.manufacturer_.name : "—",
                        price = pa.basepart_.price
                    });
                }
            }
            lvAssemblyParts.ItemsSource = displayList;
        }
    }
}
