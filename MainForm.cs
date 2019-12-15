using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication.Enthernet;

namespace _8370
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Modbus.ModBusTcpProtocol mp = new Modbus.ModBusTcpProtocol();
            mp.TransActionId = 5;
            int u = mp.TransActionId;
            mp.ProtocolId = 0;
            mp.FucCode = Modbus.FuncCode.ReadInputRegisters;

           
        }
    }
}
