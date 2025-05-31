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
using System.IO;

using WeightedRandomSelection;
using System.ComponentModel;


namespace RandPick {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        readonly ExtractModel extractModel;
        public Class ClassSix { get; set; } = new(75);
        public Class ClassFive { get; set; } = new(83);

        public MainWindow() {

            InitializeComponent();
            extractModel=new ExtractModel();
            this.DataContext = extractModel;
        }

        
        private void Class_Six_Rand_5(object sender, RoutedEventArgs e) {

            var selectedClassSix = ClassSix.SelectMultipleStudentIds(5).Order().Select(n => n.ToString("00"));
            extractModel.ExtractionResult = string.Join(",", selectedClassSix);
        }

        private void Class_Six_Rand_1(object sender, RoutedEventArgs e) {
            var selectedClassSix = ClassSix.SelectMultipleStudentIds(1).Select(n => n.ToString("00")).ToList();
            extractModel.ExtractionResult = selectedClassSix[0];
        }

        private void Class_Five_Rand_5(object sender, RoutedEventArgs e) {
        }

        private void Class_Five_Rand_1(object sender, RoutedEventArgs e) {
        }
    }

    class ExtractModel : INotifyPropertyChanged {


        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private string _ExtractionResult = "别紧张";

        public string ExtractionResult
        {
            get {
                return _ExtractionResult;
            }
            set {
                _ExtractionResult = value;
                RaisePropertyChanged(nameof(ExtractionResult));
            }
        }
    }
}