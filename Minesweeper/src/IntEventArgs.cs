using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Minesweeper.src
{
    public class IntEventArgs : EventSetter
    {
        public int value { get; }
        public IntEventArgs(int value)
        {
            this.value = value;
        }
    }
}
