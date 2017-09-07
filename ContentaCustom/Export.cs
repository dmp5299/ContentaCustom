using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentaCustom
{
    class Export
    {
        string loc;
        string lk_what;
        string mode;
        string translation;
        string id;
        PCMClientLib.IPCMcommand cmd;

        public Export(PCMClientLib.IPCMcommand cmd, string loc, string lk_what, string mode, string translation, string id)
        {
            this.cmd = cmd;
            this.id = id;
            this.loc = loc;
            this.lk_what = lk_what;
            this.mode = mode;
            this.translation = translation;
        }

        public void exportDoc(string fileName)
        {
            try
            {
                PCMClientLib.IPCMtree tree;
                tree = cmd.TreeOpen(id, loc, lk_what, mode, translation);
                PCMClientLib.IPCMcompound compoundO = tree.StartNode(1);
                int sliceCount = compoundO.SliceCount;
                for(int i=0;i<sliceCount;i++)
                {
                    PCMClientLib.IPCMslice sliceO = compoundO.SliceNext();
                    while ((sliceO != null) && (sliceO.IsContent.Equals(1)))
                    {
                        string objId = sliceO.ReadContent();
                        using (FileStream fs = File.Create(@"\\rcmwessrv01\company\DeletedContentaFiles\" + fileName + ".xml"))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes(objId);
                            // Add some information to the file.
                            fs.Write(info, 0, info.Length);
                        }
                        sliceO = compoundO.SliceNext();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("here " + e.Message);
            }
        }
    }
}
