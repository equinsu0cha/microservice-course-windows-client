using BaseClassesWorkStation;
using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;

namespace WMIdataCollector.WMILayer
{
    public class WMIdata
    {
        private IList<WMIdataCollector.Job.WmiClass> _listClasses;

        public WMIdata(IList<WMIdataCollector.Job.WmiClass> classes)
        {
            _listClasses = classes;
        }

        private string GetWQLClause()
        {
            string strWQLClauseRet = null;
            List<string> listClasses = _listClasses.Select(o => o.WmiclassDesc).ToList();
            try
            {
                string endStr = " OR";
                strWQLClauseRet = " WHERE ";

                foreach (var item in listClasses)
                {
                    strWQLClauseRet += " __CLASS = '" + item + "'" + endStr;
                }

                if (strWQLClauseRet.EndsWith(endStr))
                {
                    strWQLClauseRet = strWQLClauseRet.Remove(strWQLClauseRet.Length - endStr.Length, endStr.Length);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strWQLClauseRet;
        }

        private string GetWmiClassType(string className)
        {
            string classType = "";

            classType = (from cls in _listClasses where cls.WmiclassDesc.ToUpper().Equals(className.ToUpper()) select cls.WMIClassTypeId).FirstOrDefault();

            return classType;
        }

        private WorkStationModel GetWmiValues(string wqlClause, string IdWorkStation)
        {
            WorkStationModel objRet = null;
            ManagementObjectSearcher classSearcher = null;
            ManagementObjectSearcher propertiesSearcher = null;
            ManagementObjectCollection collClasses = null;
            ClassWMIModel classModelToAdd = null;
            PropWMIModel propToAdd = null;

            try
            {
                string query = "select * from meta_class " + wqlClause;
                classSearcher = new ManagementObjectSearcher(new ManagementScope("root\\CIMV2"), new WqlObjectQuery(query), null);

                collClasses = classSearcher.Get();

                if (collClasses != null && collClasses.Count > 0)
                {
                    objRet = new WorkStationModel();
                    objRet.Id = IdWorkStation;

                    foreach (ManagementClass wmiClass in collClasses)
                    {
                        classModelToAdd = new ClassWMIModel(wmiClass.ClassPath.ClassName);
                        classModelToAdd.workStation = IdWorkStation;
                        classModelToAdd.WmiClassType = GetWmiClassType(classModelToAdd.wmiClassName);
                        classModelToAdd.Time = DateTime.Now;

                        foreach (QualifierData qd in wmiClass.Qualifiers)
                        {
                            if ((qd.Name.Equals("dynamic") || qd.Name.Equals("static")) && (wmiClass.ClassPath.ClassName.ToUpper().Contains("WIN32")))
                            {
                                ObjectGetOptions op = new ObjectGetOptions(null, System.TimeSpan.MaxValue, true);
                                ManagementClass mc = new ManagementClass(wmiClass.ClassPath, op);
                                mc.Options.UseAmendedQualifiers = true;

                                foreach (PropertyData dataObject in mc.Properties)
                                {
                                    propToAdd = new PropWMIModel();
                                    propToAdd.propWMIName = dataObject.Name; ;
                                    propToAdd.propType = dataObject.Type.ToString();

                                    propertiesSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + wmiClass.ClassPath.ClassName);
                                    foreach (ManagementObject queryObj in propertiesSearcher.Get())
                                    {
                                        if (queryObj[dataObject.Name] != null)
                                        {
                                            propToAdd.listPropWMIValues.Add(queryObj[dataObject.Name]);
                                        }
                                    }

                                    classModelToAdd.listWMIProperties.Add(propToAdd);
                                }
                            }
                        }

                        if (classModelToAdd.listWMIProperties.Count > 0)
                        {
                            objRet.listWMIClasses.Add(classModelToAdd);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objRet;
        }

        public WorkStationModel GetWorkStationValues(string IdMac, string enterpriseId, string branchId)
        {
            WorkStationModel obj = null;
            string clauseWql = "";
            try
            {
                clauseWql = GetWQLClause();
                obj = GetWmiValues(clauseWql, IdMac);
                obj.EnterpriseId = enterpriseId;
                obj.BranchId = branchId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //getExecutionToken = null;
                //getClasses = null;
            }

            return obj;
        }
    }
}
