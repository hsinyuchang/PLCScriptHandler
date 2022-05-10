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

        public int WriteValue(PLCWriteClass plcWrite)
        {
            int resultCode = -1;
            if (mxPlc != null)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(plcWrite.DelayMilliSeconds);

                    if (plcWrite.Address.StartsWith("B"))
                        resultCode = WriteValueBit(plcWrite);
                    else
                        resultCode = WriteValueBlock(plcWrite);
                });
            }
            return resultCode;
        }

        public int ReadValue(string address, ushort lowerBit, ushort upperBit, out int result)
        {
            if (address.StartsWith("B"))
                return ReadValueBit(address, out result);
            else
                return ReadValueBlock(address, lowerBit, upperBit, out result);
        }

        private int ReadValueBit(string address, out int result)
        {
            int resultCode = -1;
            result = 0;
            if (mxPlc != null)
                resultCode = mxPlc.GetDevice(address, out result);
            return resultCode;
        }

        private int ReadValueBlock(string address, ushort lowerBit, ushort upperBit, out int result)
        {
            int resultCode = -1;
            result = 0;
            if (mxPlc != null)
            {
                resultCode = mxPlc.ReadDeviceBlock(address, 1, out int originalValue);
                uint mask = (uint)(Math.Pow(2, upperBit + 1) - 1) ^ (uint)(Math.Pow(2, lowerBit) - 1);
                result = (int)((originalValue & mask) >> lowerBit);
            }
            return resultCode;
        }

        private int WriteValueBit(PLCWriteClass plcWrite)
        {
            return mxPlc.SetDevice(plcWrite.Address, plcWrite.Value);
        }

        private int WriteValueBlock(PLCWriteClass plcWrite)
        {
            mxPlc.ReadDeviceBlock(plcWrite.Address, 1, out int originalValue);
            int originalLowerBits = originalValue & ((int)Math.Pow(2, plcWrite.LowerBit) - 1);
            int originalUpperBits = originalValue >> (plcWrite.UpperBit + 1) << (plcWrite.UpperBit + 1);
            int afterValue = originalUpperBits | (plcWrite.Value << plcWrite.LowerBit) | originalLowerBits;
            return mxPlc.WriteDeviceBlock(plcWrite.Address, 1, ref afterValue);
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
}
