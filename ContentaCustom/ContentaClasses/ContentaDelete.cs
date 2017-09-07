using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace ContentaCustom.ContentaClasses
{
    public class ContentaDelete
    {
        PCMClientLib.PCMConnection conn;
        PCMClientLib.IPCMcommand cmd;
        string fileName;

        public ContentaDelete(PCMClientLib.PCMConnection conn, PCMClientLib.IPCMcommand cmd, string fileName)
        {
            this.conn = conn;
            this.cmd = cmd;
            this.fileName = fileName;
        }

        public void Delete()
        {
            cmd.Select("/#1/#2/#6");

            PCMClientLib.IPCMdata S1000Dcontainers = cmd.ListChildren();
            string wipId = PCMcommandCustom.getWhip(S1000Dcontainers);
            cmd.Select(wipId);
            PCMClientLib.IPCMdata WIPprojects = cmd.ListChildren();

            string projectListString = PCMcommandCustom.getWipProjects(WIPprojects, wipId);
            PCMcommandCustom custom = new PCMcommandCustom(conn);
            XmlDocument doc = custom.searchByFilePath(cmd, fileName, projectListString + "/#1/#2/#6");
            XmlNodeList records = doc.SelectNodes("//Record");
            string OptionalCkOutDir = @"\\rcmwessrv01\company\DeletedContentaFiles\";
            foreach (XmlNode record in records)
            {
                string id = record.SelectSingleNode("descendant::ID_PATH").InnerText;
                Export exportDoc = new Export(cmd, "nolock", "", "read", "binary", id);
                if (!File.Exists(OptionalCkOutDir + fileName + ".xml"))
                {
                    exportDoc.exportDoc(fileName);
                }
                cmd.Delete(id);
            }
        }
    }
}
