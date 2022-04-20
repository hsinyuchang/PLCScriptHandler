using Mirle.AK1.PLCScriptHandler.Driver;
using Mirle.AK1.PLCScriptHandler.Model;
using Mirle.AK1.Utils.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mirle.AK1.PLCScriptHandler.ViewModel
{
    public class FireActionViewModel : ViewModelBase
    {
        #region Properties
        private Dictionary<string, PLCAction> plcActions = new Dictionary<string, PLCAction>();
        public Dictionary<string, PLCAction> PLCActions
        {
            get => plcActions;
            set
            {
                plcActions = value;
                OnPropertyChanged(nameof(PLCActions));
            }
        }

        private List<PLCEvent> plcEvents = new List<PLCEvent>();
        public List<PLCEvent> PLCEvents
        {
            get => plcEvents;
            set
            {
                plcEvents = value;
                OnPropertyChanged(nameof(PLCEvents));
            }
        }
        public string SelectedAction { get; set; }
        #endregion

        #region Commands
        public ICommand FireButtonCommand { get; }
        #endregion

        #region Constructors
        public FireActionViewModel()
        {
            FireButtonCommand = new CommandImplementation(_ => onFireButtonCommand());

            Dictionary<string, PLCAction> pLCActions = new Dictionary<string, PLCAction>();
            List<PLCEvent> pLCEvents = new List<PLCEvent>();
            ConfigReader configReader = new ConfigReader(ref pLCEvents, ref pLCActions);
            PLCActions = pLCActions;
            PLCEvents = pLCEvents;
        }
        #endregion

        private void onFireButtonCommand()
        {
            if (SelectedAction != null)
                PLCActions[SelectedAction]?.Fire();
        }
    }
}
