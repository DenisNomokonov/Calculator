using System.Windows;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(List<string> history)
        {
            InitializeComponent();
            HistoryListBox.ItemsSource = history;
        }
    }
}
