using Mirle.AK1.PLCScriptHandler.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Mirle.AK1.PLCScriptHandler.Model
{
    public class PLCEvent
    {
        public string EventAddress { get; set; } = string.Empty;
        public ushort LowerBit { get; set; }
        public ushort UpperBit { get; set; }
        public List<PLCEventResponse> Responses { get; private set; } = new List<PLCEventResponse>();

        private Timer timer;

        public PLCEvent(int timerInterval = 1000)
        {
            timer = new System.Timers.Timer(timerInterval);
            timer.Elapsed += (sender, e) => CheckValue();
        }

        public void StartMonitorEvent()
        {
            timer.Start();
        }

        private void CheckValue()
        {
            int value;
            var resultCode = PLCDriver.Instance.ReadValue(EventAddress, LowerBit, UpperBit, out value);
            if (resultCode == 0)
            {
                foreach (var response in Responses)
                {
                    if (value == response.ExpectedValue)
                    {
                        bool isSuccess = true;
                        foreach (var func in response.WriteFuncs)
                            isSuccess &= (func.Invoke() == 0);
                    }
                }
            }
        }

        public class PLCEventResponse
        {
            public ushort ExpectedValue { get; set; }
            public List<Func<int>> WriteFuncs { get; private set; } = new List<Func<int>>();
        }
    }


}
