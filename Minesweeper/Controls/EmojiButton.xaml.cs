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

namespace Minesweeper.Controls
{
    public partial class EmojiButton : UserControl
    {
        public static readonly DependencyProperty EmojiSymbolProperty =
            DependencyProperty.Register("EmojiSymbol", typeof(string), typeof(EmojiButton), new PropertyMetadata(string.Empty));

        public string EmojiSymbol
        {
            get { return (string)GetValue(EmojiSymbolProperty); }
            set { SetValue(EmojiSymbolProperty, value); }
        }

        public event EventHandler Click;

        public EmojiButton()
        {
            InitializeComponent();

            setSymbol("🙂");
        }

        public void setSymbol(string symbol)
        {
            EmojiSymbol = symbol;
        }

        private void buttonClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}
