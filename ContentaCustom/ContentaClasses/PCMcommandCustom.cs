using PCMtoolsAPILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;


namespace ContentaCustom.ContentaClasses
{
    public class PCMcommandCustom
    {
        PCMClientLib.PCMConnection conn;

        public PCMcommandCustom(PCMClientLib.PCMConnection conn)
        {
            this.conn = conn;
        }

        public static string getWhip(PCMClientLib.IPCMdata containers)
        {
            string wipId = "";
            for (int i = 0; i < containers.RecordCount; i++)
            {
                string name = containers.GetValueByLabel(i, "NAME");
                if (name == "WIP")
                {
                    string desktop = containers.GetValueByLabel(i, "DESKTOP");
                    string config = containers.GetValueByLabel(i, "CONFIGURATION_ID");
                    string objId = containers.GetValueByLabel(i, "OBJECT_ID");
                    wipId = desktop + @"/" + config + "/" + objId;
                }
            }
            return wipId;
        }

        public static string getWipProjects(PCMClientLib.IPCMdata projects, string wipId)
        {
            string projectIdString="";
            for (int i = 0; i < projects.RecordCount; i++)
            {
                string projectId = projects.GetValueByLabel(i, "OBJECT_ID");
                projectIdString = (wipId + @"/" + projectId) + @"|" + projectIdString;
            }
            return projectIdString;
        }

        public static void DeleteObjects(PCMClientLib.IPCMdata2 objectsToDelete, PCMClientLib.IPCMcommand cmd)
        {
            try
            {
                for (int i = 0; i < objectsToDelete.RecordCount; i++)
                {
                    string idPath = objectsToDelete.GetValueByLabel(i, "ID_PATH");
                    cmd.Delete(idPath);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public XmlDocument searchByFilePath(PCMClientLib.IPCMcommand cmd, string id, string paths)
        {
            string command = "find -relpath ";
            string[] pathList = paths.Split('|');
            foreach(string path in pathList)
            {
                string configId = fileParse(path);
                command += "\""+ configId + "\"|\""+id+"\" ";
            }
            PCMClientLib.IPCMdata data = cmd.ExecCmd(command);

            PCMClientLib.IPCMdata names = cmd.ExecCmd("translate " + paths.Replace('|', ' '));

            int records = data.RecordCount;
            List<string> rows = new List<string>();
            for (int i=0;i< records; i++)
            {
                string namePath = data.GetValueByLabel(i, "NAME_PATH");
                string idPath = data.GetValueByLabel(i, "ID_PATH");
               
                ComposeNameIdPath(ref idPath, ref namePath, names);
                string config = fileParse(idPath);
                string object1 = fileParse(namePath);
                string row = "||" + config + "|" + object1 + "||" + idPath + "|" + namePath + "|";
                rows.Add(row);
            }
            return LoadDataFromArray("SCORE|OBJECT_ID|NAME|TYPE|ID_PATH|NAME_PATH", rows);
        }

        public XmlDocument LoadDataFromArray(string labels, List<string> rows)
        {
            string record = "|" + labels + "| ";

            for(int i = 0; i < rows.Count; i++)
            {
                record += rows[i] + " ";
            }

            string string1 = " |" + string.Format("{0:D6}", rows.Count + 2) + "|" + string.Format("{0:D6}", record.Length + 9) + "| |" + string.Format("{0:D6}", 6) + "| " + record;
            string response = setPortalData(string1);
            response = "<Data>" + response + "</Data>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);
            return doc;
        }

        public string setPortalData(string string1)
        {
            string xml_str = "";
            
            string[] record = string1.Split('|');
            
            int size = Int32.Parse(record[4].Substring(0, 6));

            //string[] keys = new List<string>(record).GetRange(6, (size+5)).ToArray();

            string[] keys = record.ToList().GetRange(6, 6).ToArray();
        
           string[] values = new List<string>(record).GetRange((size + 7), (record.Length-(size+7))).ToArray();

            int j = 0;

            while(j < values.Length)
            {
                xml_str += "<Record>\n";

                for(int i=0;i<keys.Length;i++)
                {
                    xml_str += '<' + keys[i] + '>' + values[i + j] + @"</" + keys[i] + @">";
                }
                j += (size + 1);
                xml_str += "</Record>";
            }
            return xml_str;
        }

        public void ComposeNameIdPath(ref string idPath, ref string namePath, PCMClientLib.IPCMdata names)
        {
            string id = idPath.Substring(0, idPath.IndexOf('/'));
            int count = names.RecordCount;

            for(int i=0;i< count; i++)
            {
                string tempIdPath = names.GetValueByLabel(i,"ID_PATH");
                int pos = tempIdPath.LastIndexOf('/');
                string tempId = tempIdPath.Substring(pos + 1);

                if(tempId == id)
                {
                    idPath = tempIdPath.Substring(0, pos + 1) + idPath;

                    string tempNamePath = names.GetValueByLabel(i, "NAME_PATH");
                    pos = tempNamePath.LastIndexOf('/');
                    namePath = tempNamePath.Substring(0, pos + 1) + namePath;
                }
            }
        }

        private string fileParse(string path)
        {
            if(path.Contains('/'))
            {
                string[] subPaths = path.Split('/');
                return subPaths[subPaths.Length - 1];
            }
            else
                return path;
        }
    }

}
