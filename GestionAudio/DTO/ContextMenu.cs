using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Entity;
using NAudio.Wave;

namespace DTO
{
    public class ContextMenu
    {
        public ObservableCollection<ContextMenu> SubItems { get; set; }
        public string Header { get; set; }
        public object CommandParameter { get; set; }
        public object Command { get; set; }
        public bool IsEnable { get; set; } = true;
    }
}
