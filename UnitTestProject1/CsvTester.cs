using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using _8370;
using _8370.Modbus;
namespace CsvTest
{








    [TestClass]



    public class CsvTester
    {
        [TestMethod]
        public void CsvGenerator()
        {
            CsvGenerator csvGenerator = new _8370.CsvGenerator();
            List<UserData> users = new List<UserData>() {
            new UserData(){Barcode = "Test"},
            new UserData(){Barcode="Demo"},
            };
              int i=0;
            
                 i = csvGenerator.ExportCsv<UserData>(false, "D:\\f.csv", users);

          
                Assert.AreEqual(1, i);

            

           

        }
       
      

       
     




    }


}
