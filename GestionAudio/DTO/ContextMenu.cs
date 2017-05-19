using System.Collections.ObjectModel;

namespace DTO
{
    public class ContextMenu
    {
        #region Public Properties

        public object Command { get; set; }
        public object CommandParameter { get; set; }
        public string Header { get; set; }
        public bool IsEnable { get; set; } = true;
        public ObservableCollection<ContextMenu> SubItems { get; set; }

        #endregion Public Properties
    }
}