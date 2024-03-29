﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace _8370.Modbus
{

    public enum FuncCode
    {
        ReadCoils = 1,
        ReadDiscreteInputs = 2,
        ReadMultipleHoldingRegisters = 3,
        ReadInputRegisters = 4,
        WriteSingleCoil = 5,
        WriteSingleRegister = 6,
        Diagnostics = 8,
        WriteMultipleCoils = 0x15,
        WriteMultipleRegisters = 0x16,
        ReadWriteMultipleRegisters = 0x17,


    }

    public enum FuncCodeError
    {
        ReadCoils = 0x81,
        ReadDiscreteInputs = 0x82,
        ReadMultipleHoldingRegisters = 0x83,
        ReadInputRegisters = 0x84,
        WriteSingleCoil = 0x85,
        WriteSingleRegister = 0x86,
        Diagnostics = 0x88,
        WriteMultipleCoils = 0x95,
        WriteMultipleRegisters = 0x96,
        ReadWriteMultipleRegisters = 0x97,


    }


    public enum CommunicationType
    {
        Tcp,
        RTU,


    }


    public class Mddata
   {
        byte[] gooddata = new byte[13];
        public Mddata()
        {




            gooddata[5] = 0x07;
            gooddata[6] = 0x01;
            gooddata[7] = 0x03;
            gooddata[8] = 0x04;
        }



     }






    public class ModBusPdu
    {
        public FuncCode FucCode
        {
            set => fuccode = (byte)value;


            get => (FuncCode)fuccode;


        }
        public int StartAdr
        {

            set => _startadr= ReveseBytes(value);


            get => TurnBcckInt(_startadr);


        }
        public int DataLen
        {

            set => _datalen=ReveseBytes(value);


            get => TurnBcckInt(_datalen);



        }
        public byte[] Data { set; get; } = null;
        public byte[] Pdu { 
            set {
                _pdu = (byte[])value;
            }
            get {
                _pdu = Getpdu();
                return _pdu;


            }
        }


        public byte[] Getpdu()
        {
            List<byte> o = new List<byte>();

            o.Add(fuccode);
            o.AddRange(_startadr);
            o.AddRange(_datalen);
            if (Data!=null)
            {
                o.Concat(Data);
            }
            

            return o.ToArray();



        }


        private byte[] _pdu = null;
        protected byte fuccode;
        private byte[] _startadr = new byte[2];
        private byte[] _datalen = new byte[2];
        protected int TurnBcckInt(byte[] o)
        {
            Array.Reverse(o);
            return BitConverter.ToInt16(o, 0);

        }
        protected byte[] ReveseBytes(int y)
        {
            byte[] ox = BitConverter.GetBytes((Int16)y);

            Array.Reverse(ox);
            return ox;

        }
    }

    public class ModBusTcpProtocol:ModBusPdu
    {
        public int TransActionId {
            set => TracId=ReveseBytes(value);


            get => TurnBcckInt(TracId);




        }
        public int ProtocolId
        {
            set => ProId=ReveseBytes(value);


            get => TurnBcckInt(ProId);



        }
        public int MessageLength
        {
            set => MsgLen=ReveseBytes(value);


            get => TurnBcckInt(MsgLen);


        }
        public int DeviceId
        {
            set => DevId=(byte)value;


            get => (int)DevId;


        }
        public int ErrCode { set; get; }
       // public byte[] ADU { set => Setadu(); get => GetADU(); } = null;
        public byte[] ADU
        {
            set
            {
                _adu = (byte[])value;
            }
            get
            {
                _adu = Getadu();
                return _adu;


            }


        }

        public void Dispatcher(byte[] FeedbackData)
        {
            if (FeedbackData!=null)
            {
                Buffer.BlockCopy(FeedbackData, 0, TracId,0, 2);
                Buffer.BlockCopy(FeedbackData, 2, ProId, 0, 2);
                Buffer.BlockCopy(FeedbackData, 4, MsgLen, 0, 2);
                DevId = FeedbackData[6];
                fuccode= FeedbackData[7];
                fbdataLenght= FeedbackData[8];
                if ((int)fbdataLenght>0)
                {

                    Buffer.BlockCopy(FeedbackData, 9, Data, 0, (int)fbdataLenght);
                }
                if ((int)FucCode>0x80)
                {
                    ErrCode = (int)FeedbackData[8];

                }
                else
                {

                    ErrCode = 0;


                }


            }

            ;
        }


        private byte[] TracId = new byte[2];
        private byte[] ProId = new byte[2];
        private byte[] MsgLen = new byte[2];
        private byte DevId;
        private byte[] _adu = null;
        private byte fbdataLenght;
        public byte[] Getadu()
        {
            List<byte> o = new List<byte>();

            o.AddRange(TracId);
            o.AddRange(ProId);

            MsgLen =ReveseBytes((Int16)(Pdu.Length+1));

            o.AddRange(MsgLen);
            o.Add(DevId);
            o.AddRange(Pdu);
            return o.ToArray();

        }
      
        
    }

  
    



  

    public class ModbusClient 
    {

        TcpClient tcpClient;
        ModBusTcpProtocol MdTcpClient;
        ModBusTcpProtocol MdTcpClientFeedback;
        public ModbusClient(CommunicationType comtype )
        {
            startup( comtype);
        }
       


     private  void startup(CommunicationType comtype)
        {




            if (comtype==CommunicationType.Tcp)
            {
                tcpClient = new TcpClient();
                MdTcpClient = new ModBusTcpProtocol();
                MdTcpClientFeedback = new ModBusTcpProtocol();
                MdTcpClient.ProtocolId = 0;

            }

        }


    



        public void SendMsg(byte[] msg, TcpClient tmpTcpClient)
        {

            NetworkStream ns = tmpTcpClient.GetStream();
            if (ns.CanWrite)
            {

                ns.Write(msg, 0, msg.Length);
            }

        }

        public byte[] ReceiveMsg(TcpClient tmpTcpClient)
        {

            byte[] receiveBytes = new byte[256];
            int numberOfBytesRead = 0;
            NetworkStream ns = tmpTcpClient.GetStream();
            tmpTcpClient.ReceiveBufferSize = 256;
            if (ns.CanRead)
            {
                do
                {
                    numberOfBytesRead = ns.Read(receiveBytes, 0, tmpTcpClient.ReceiveBufferSize);

                }
                while (ns.DataAvailable);
            }
            return receiveBytes;
        }

       
       

        byte[] AssignADU(FuncCode fcode, int Uid, int StartAdr, int Qantity, byte[] data)
        {
            MdTcpClient.FucCode = fcode;
            MdTcpClient.StartAdr = StartAdr;
            MdTcpClient.DataLen = Qantity;
            MdTcpClient.DeviceId = Uid;
            MdTcpClient.Data = data;
            return MdTcpClient.ADU;



        }


        byte[] TcpFeedbackChecker(ModBusTcpProtocol ObjFeedback, ModBusTcpProtocol ObjSender, out int ErrCode)
        {
           
            
          //  GetTransactionId_ProtocolId(feedbackMsg, out fbTransId, out fbProId);
           
            byte[] data = null;
            int Err = 0;
            
            if (ObjFeedback.TransActionId== ObjSender.TransActionId)
            {


                if (ObjSender.DeviceId == ObjFeedback.DeviceId)
                {

                    if (ObjSender.FucCode == ObjFeedback.FucCode)
                    {
                        data = ObjFeedback.Data;
                       
                        Err = 0;

                    }
                    else
                    {
                        if ((int)ObjFeedback.FucCode > 0x80)
                        {

                            Err = ObjFeedback.ErrCode;


                        }



                    }


                }

            }

            ErrCode = Err;


            return data;
        }



        public byte[] SendDataWaitResponse(FuncCode fcode, int uid, int StartAdr, int Quatity, byte[] WriteData, out int ErrCode)
        {

            byte[] gg = null;

            byte[] ADU = AssignADU(fcode, uid, StartAdr, Quatity, WriteData);
            try
            {
               // SendMsg(ADU, tcpClient);
              //  MdTcpClientFeedback.Dispatcher( ReceiveMsg(tcpClient));

                MdTcpClientFeedback.Dispatcher(ADU);
                gg = TcpFeedbackChecker(MdTcpClientFeedback, MdTcpClient, out ErrCode);
            }
            catch
            {

                throw new NotImplementedException();

            }





            return gg;
        }


        public void ConnectToServer(string hostIP, int portNo)
        {


            //先建立IPAddress物件,IP為欲連線主機之IP
            IPAddress ipa = IPAddress.Parse(hostIP);

            //建立IPEndPoint
            IPEndPoint ipe = new IPEndPoint(ipa, portNo);

            //先建立一個TcpClient;


            //開始連線
            try
            {
                Console.WriteLine("主機IP=" + ipa.ToString());
                Console.WriteLine("連線至主機中...\n");
                tcpClient.Connect(ipe);
                if (tcpClient.Connected)
                {
                    Console.WriteLine("連線成功!");

                    // byte[] x = new byte[100];
                    //cb.SendMsg(x, tcpClient);
                    //  Console.WriteLine(cb.ReceiveMsg(tcpClient));
                }
                else
                {
                    Console.WriteLine("連線失敗!");
                }
                //  Console.Read();
            }
            catch (Exception ex)
            {
                tcpClient.Close();
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
