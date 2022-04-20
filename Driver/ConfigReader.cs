using Mirle.AK1.PLCScriptHandler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Mirle.AK1.PLCScriptHandler.Driver
{
    public class ConfigReader
    {
        public ConfigReader(ref List<PLCEvent> plcEvents, ref Dictionary<string, PLCAction> plcActions)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("PLCFunction.xml");

            PLCDriver.LogicalStationNumber = Convert.ToInt32(xmlDoc?.DocumentElement?.Attributes?["LogicalStationNumber"]?.Value);
            XmlNodeList eventNodeList = xmlDoc?.SelectNodes("MXConfig/Event");
            if (eventNodeList != null)
            {
                foreach (XmlNode eventNode in eventNodeList)
                {
                    string eventAddr = eventNode?.Attributes?["Address"]?.Value ?? string.Empty;
                    if (string.IsNullOrEmpty(eventAddr))
                        continue;

                    PLCEvent newPLCEvent = new PLCEvent();
                    newPLCEvent.EventAddress = eventAddr;
                    newPLCEvent.LowerBit = Convert.ToUInt16(eventNode?.Attributes?["LowerBit"]?.Value);
                    newPLCEvent.UpperBit = Convert.ToUInt16(eventNode?.Attributes?["UpperBit"]?.Value);
                    if (eventNode?.ChildNodes != null)
                    {
                        foreach (XmlNode responseNode in eventNode.ChildNodes)
                        {
                            PLCEventResponse eventResponse = new PLCEventResponse();
                            eventResponse.ExpectedValue = Convert.ToUInt16(responseNode?.Attributes?["ExpectValue"]?.Value);
                            if (responseNode?.ChildNodes != null)
                            {
                                foreach (XmlNode writeNode in responseNode.ChildNodes)
                                {
                                    string writeAddr = writeNode?.Attributes?["WriteAddr"]?.Value ?? string.Empty;
                                    if (string.IsNullOrEmpty(writeAddr))
                                        continue;
                                    ushort lowerBit = Convert.ToUInt16(writeNode?.Attributes?["LowerBit"]?.Value);
                                    ushort upperBit = Convert.ToUInt16(writeNode?.Attributes?["UpperBit"]?.Value);
                                    int writeValue = Convert.ToInt32(writeNode?.Attributes?["WriteValue"]?.Value);
                                    int delayTime = Convert.ToInt32(writeNode?.Attributes?["DelayTime"]?.Value);
                                    eventResponse.WriteFuncs.Add(() =>
                                    {
                                        return PLCDriver.Instance.WriteValue(new PLCWriteClass()
                                        {
                                            Address = writeAddr,
                                            LowerBit = lowerBit,
                                            UpperBit = upperBit,
                                            Value = writeValue,
                                            DelayMilliSeconds = delayTime,
                                        });
                                    });
                                }
                            }
                            newPLCEvent.Responses.Add(eventResponse);
                        }
                    }
                    plcEvents.Add(newPLCEvent);
                }
            }


            XmlNodeList actionNodeList = xmlDoc?.SelectNodes("MXConfig/Action");
            if (actionNodeList != null)
            {
                foreach (XmlNode actionNode in actionNodeList)
                {
                    string actionName = actionNode?.Attributes?["Name"]?.Value ?? string.Empty;
                    if (string.IsNullOrEmpty(actionName))
                        continue;

                    PLCAction newPLCAction = new PLCAction();
                    newPLCAction.Name = actionName;
                    if (actionNode?.ChildNodes != null)
                    {
                        foreach (XmlNode writeNode in actionNode.ChildNodes)
                        {
                            string writeAddr = writeNode?.Attributes?["WriteAddr"]?.Value ?? string.Empty;
                            if (string.IsNullOrEmpty(writeAddr))
                                continue;

                            ushort lowerBit = Convert.ToUInt16(writeNode?.Attributes?["LowerBit"]?.Value);
                            ushort upperBit = Convert.ToUInt16(writeNode?.Attributes?["UpperBit"]?.Value);
                            int writeValue = Convert.ToInt32(writeNode?.Attributes?["WriteValue"]?.Value);
                            int delayTime = Convert.ToInt32(writeNode?.Attributes?["DelayTime"]?.Value);
                            newPLCAction.WriteFuncs.Add(() =>
                            {
                                return PLCDriver.Instance.WriteValue(new PLCWriteClass()
                                {
                                    Address = writeAddr,
                                    LowerBit = lowerBit,
                                    UpperBit = upperBit,
                                    Value = writeValue,
                                    DelayMilliSeconds = delayTime,
                                });
                            });
                        }
                    }
                    plcActions.Add(newPLCAction.Name, newPLCAction);
                }
            }
        }
    }
}
