using Mirle.AK1.PLCScriptHandler.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.AK1.PLCScriptHandler.Model
{
    public class PLCEvent
    {
        public string EventAddress { get; set; } = string.Empty;
        public ushort LowerBit { get; set; }
        public ushort UpperBit { get; set; }
        public List<PLCEventResponse> Responses { get; private set; } = new List<PLCEventResponse>();

        public void CheckValue()
        {
            var value = PLCDriver.Instance.ReadValue(EventAddress, LowerBit, UpperBit);
            foreach (var response in Responses)
            {
                if (value == response.ExpectedValue)
                {
                    bool isSuccess = true;
                    foreach (var func in response.WriteFuncs)
                    {
                        isSuccess &= func.Invoke();
                    }
                }
            }
        }
    }

    public class PLCEventResponse
    {
        public ushort ExpectedValue { get; set; }
        public List<Func<bool>> WriteFuncs { get; private set; } = new List<Func<bool>>();
    }
}
