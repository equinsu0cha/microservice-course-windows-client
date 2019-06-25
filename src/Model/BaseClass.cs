using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using WMIdataCollector.ServicesHandler;

namespace WMIdataCollector.Model
{
    [Serializable]
    public abstract class BaseClass
    {
        //private string _macAddress;
        //public string MacAddress
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(_macAddress))
        //            _macAddress = GetIdMacAddress();
        //        return _macAddress;
        //    }
        //}

       
        //public string GetIdMacAddress()
        //{
        //    // ManagementObjectSearcher objSearcher = null;
        //    string ret = "";
        //    //Placa de rede Ethernet
        //    //string wql = @"SELECT * FROM Win32_NetworkAdapter WHERE AdapterTypeId = 0 AND NOT Description Like 'Wireles' AND NOT Description Like 'vEthernet'";

        //    //try
        //    //{
        //    //    objSearcher = new ManagementObjectSearcher("root\\CIMV2", wql);
        //    //    foreach (ManagementObject objWql in objSearcher.Get())
        //    //    {
        //    //        ret = (objWql["MACAddress"] != null && !String.IsNullOrEmpty(objWql["MACAddress"].ToString())) ? objWql["MACAddress"].ToString() : "";
        //    //    }
        //    //}
        //    //catch (ManagementException ex)
        //    //{
        //    //    throw ex;
        //    //}
        //    //finally
        //    //{
        //    //    objSearcher.Dispose();
        //    //    wql = null;
        //    //}
        //    ret = "5C-C9-D3-A2-89-37";
        //    return ret;
        //}

        public string GetIdMacAddress()
        {
            string sMacAddress = string.Empty;
            if (String.IsNullOrEmpty(ExecutionCfg.MacAddressFromArgs))
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface nic in nics)
                {
                    if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                    {
                        if (nic.GetPhysicalAddress().ToString() != "")
                        {
                            sMacAddress = nic. GetPhysicalAddress().ToString();
                            break;
                        }
                    }
                }
            }
            else
            {
                sMacAddress = ExecutionCfg.MacAddressFromArgs;
            }
            
            return sMacAddress;
        }


    }   
}
