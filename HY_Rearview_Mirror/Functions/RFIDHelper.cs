using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY_Rearview_Mirror.Functions
{
    public class RFIDHelper
    {
        public RFIDHelper()
        {
        }

        public string Read()
        {          
            //var read = global.ZS_ModbusRtuHelpe.ReadStringUtf8(2,8193, 200);
            var read = global.ZS_ModbusRtuHelpe.ReadStringUtf8("s=2;8193", 200);

            return read.Content;
        }

        public void  Write(string val)
        {
            //global.ZS_ModbusRtuHelpe.WriteStringUtf8(2,8193, val, 200);    
            global.ZS_ModbusRtuHelpe.WriteStringUtf8("s=2;8193", val, 200);
        }
    }
}
