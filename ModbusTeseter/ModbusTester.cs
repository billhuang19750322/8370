using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _8370.Modbus;

namespace ModbusTeseter
{


 public class  Mddata

    [TestClass]
 

       
        

        [TestMethod]
        public void TestMethod1()
        {
            //MyModBus.ConnectToServer("192.168.0.225", 502);
            ModbusClient md = new ModbusClient(CommunicationType.Tcp);
            byte[] wd = null;
            int err = 0;
            byte[]  xx = md.SendDataWaitResponse(FuncCode.ReadMultipleHoldingRegisters,1, 30, 2,wd,out err);
            byte[] oo = new byte[8];
            Array.Reverse(xx, 0, 2);
            Array.Reverse(xx, 2, 2);

            oo = xx;



            Single dd = BitConverter.ToSingle(xx, 0);


            Assert.AreEqual(0x0258,xx );


        }


    }
}
