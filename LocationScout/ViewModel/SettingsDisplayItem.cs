using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationScout.ViewModel
{
    public class SettingsDisplayItem : DisplayItemBase
    {
        #region attributes

        private bool _autoPasteFromClipboard = true;

        public bool AutoPasteFromClipboard
        {
            get => _autoPasteFromClipboard;
            set
            {
                _autoPasteFromClipboard = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region constructor
        public SettingsDisplayItem()
        {
        }
        #endregion

        #region methods
        public override void Reset()
        {
            base.Reset();

            AutoPasteFromClipboard = true;
        }
        #endregion
    }
}
