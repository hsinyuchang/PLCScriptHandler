using ActUtlTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mirle.AK1.PLCScriptHandler.Driver
{
    public class PLCDriver
    {
        private ActUtlType mxPlc;
        private static PLCDriver instance = null;
        public static PLCDriver Instance { get { return instance ?? new PLCDriver(); } }
        public static int LogicalStationNumber { get; set; } = 0;


        private PLCDriver()
        {
            mxPlc = new ActUtlType();
            mxPlc.ActLogicalStationNumber = LogicalStationNumber;
            int iReturnCode = mxPlc.Open();
            if (iReturnCode == 0)
                instance = this;
        }

        public bool WriteValue(PLCWriteClass plcWrite)
        {
            bool result = false;
            if (mxPlc != null)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(plcWrite.DelayMilliSeconds);

                    int originalValue = 0;
                    mxPlc.ReadDeviceBlock(plcWrite.Address, 1, out originalValue);
                    int originalLowerBits = originalValue & ((int)Math.Pow(2, plcWrite.LowerBit) - 1);
                    int originalUpperBits = originalValue >> (plcWrite.UpperBit + 1) << (plcWrite.UpperBit + 1);
                    int afterValue = originalUpperBits | (plcWrite.Value << plcWrite.LowerBit) | originalLowerBits;
                    int resultCode = mxPlc.WriteDeviceBlock(plcWrite.Address, 1, ref afterValue);
                    result = (resultCode == 0);
                });
            }
            return result;
        }

        public int ReadValue(string address, ushort lowerBit, ushort upperBit)
        {
            int result = -1;
            if (mxPlc != null)
            {
                int originalValue = 0;
                mxPlc.ReadDeviceBlock(address, 1, out originalValue);
                uint mask = (uint)(Math.Pow(2, upperBit + 1) - 1) ^ (uint)(Math.Pow(2, lowerBit) - 1);
                result = (int)((originalValue & mask) >> lowerBit);
            }
            return result;
        }
    }

    public class PLCWriteClass
    {
        public string Address = string.Empty;
        public ushort LowerBit;
        public ushort UpperBit;
        public int Value;
        public int DelayMilliSeconds;
    }
}
