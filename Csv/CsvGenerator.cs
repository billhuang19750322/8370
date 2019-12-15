using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _8370
{
    public class CsvGenerator
    {

        public int ExportCsv<T>(bool genColumn, string FilePath,List<T> data)
        {
            bool FileExit = File.Exists(FilePath);

            try
            {
                using (StreamWriter file = new StreamWriter(FilePath, true))
                {
                    Type t = typeof(T);
                    PropertyInfo[] propInfos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    //是否要輸出屬性名稱
                    if (FileExit == false | genColumn)
                    {
                        file.WriteLineAsync(string.Join(",", propInfos.Select(i => i.Name)));
                    }
                    foreach (var item in data)
                    {
                        file.WriteLineAsync(string.Join(",", propInfos.Select(i => i.GetValue(item))));

                    }
                }
                return 1;
            }
            catch
            {
                return 0;

            }

            
        }

        
};

    public class UserData
    {
        public string Barcode { set; get; }
        public float Qstp { set; get; }
        public float Pstp { set; get; }
        public float TimeStamp { set; get; }

        public int MachineID { set; get; }




    }

}

