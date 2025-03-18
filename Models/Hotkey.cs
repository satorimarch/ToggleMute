using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToggleMute.Models
{
    public class Hotkey(Key key, ModifierKeys modifiers)
    {
        public Key Key { get; } = key;

        public ModifierKeys Modifiers { get; } = modifiers;
    }
}
