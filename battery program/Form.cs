using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Teamproject_sever
{
    public partial class Form1 : Form
    {
        //string[] items = { "전체", "A", "B", "C", "D" };
        OracleConnection conn;
        OracleCommand cmd;
        OracleDataReader rdr;
        Forms.UserControl2 page2 = new Forms.UserControl2();
        Forms.UserControl3 page3 = new Forms.UserControl3();
        Forms.UserControl4 page4 = new Forms.UserControl4();
        Forms.UserControl5 page5 = new Forms.UserControl5();
        private TcpListener server;
        public class ListViewItemData
        {
            public string Product_Name { get; set; }
            public string Client_Name { get; set; }
            public string Product_Count { get; set; }
            public List<string> SubItems { get; set; }
        }
        private void StartServer()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            int port = 13003;
            server = new TcpListener(localAddr, port);
            server.Start();

            Thread serverThread = new Thread(ListenForClients);
            serverThread.IsBackground = true;
            serverThread.Start();
        }
        private void ListenForClients()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread clientThread = new Thread(HandleData);
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
        }
        
        private void HandleData(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length); //의심
            string jsonData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            List<ListViewItemData> receiveData = JsonSerializer.Deserialize<List<ListViewItemData>>(jsonData);
            Invoke(new Action(() =>
            {
                foreach (var item in receiveData)
                {
                    ListViewItem lvItem = new ListViewItem(item.Product_Name);
                    lvItem.SubItems.Add(item.Client_Name);
                    lvItem.SubItems.Add(item.Product_Count);
                    foreach (var subItem in item.SubItems)
                    {
                        lvItem.SubItems.Add(subItem);
                    }
                    if (!comboBox1.Items.Contains(item.Client_Name))
                    {
                        comboBox1.Items.Add(item.Client_Name);
                    }

                }
            }));
        }        
        public Form1()
        {
            InitializeComponent();
            StartServer();

        }
        public void Connect()
        {
            string strConn = "Data Source=(DESCRIPTION=" +
               "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)" +
               "(HOST=localhost)(PORT=1521)))" +
               "(CONNECT_DATA=(SERVER=DEDICATED)" +
               "(SERVICE_NAME=xe)));" +
               "User Id=hr;Password=hr;";

            conn = new OracleConnection(strConn);
            conn.Open();
            cmd = new OracleCommand();
            cmd.Connection = conn; //연결객체와 연동
        }
        public void Select()
        {
            //cmd.CommandText = "SELECT * FROM ORDER_INFO";
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int id = int.Parse(rdr["ORDER_ID"].ToString());
                string client = rdr["CLIENT_NAME"] as string;
                string product = rdr["PRODUCT_NAME"] as string;
                int count = int.Parse(rdr["PRODUCT_COUNT"].ToString());
                DateTime datetime = Convert.ToDateTime(rdr["ORDER_DATE"]);
                string delivery = rdr["DELIVERY_STATUS"].ToString();

                string listItem = $"{id}\t{client}\t{product}\t{count}\t{datetime}\t{delivery}";
                listBox1.Items.Add(listItem);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> items = new List<string>();
            items.Add("-전체-");
            Connect();
            cmd.CommandText = "SELECT DISTINCT CLIENT_NAME FROM ORDER_INFO";
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                items.Add(rdr["CLIENT_NAME"] as string);
            }
            comboBox1.Items.AddRange(items.ToArray());
            cmd.CommandText = "SELECT * FROM ORDER_INFO ORDER BY ORDER_ID";
            Select();           
        }
        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Controls.Remove(page2);
            panel1.Controls.Remove(page3);
            panel1.Controls.Remove(page4);
            panel1.Controls.Remove(page5);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Controls.Remove(page3);
            panel1.Controls.Remove(page4);
            panel1.Controls.Remove(page5);
            panel1.Controls.Add(page2);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Controls.Remove(page2);
            panel1.Controls.Remove(page4);
            panel1.Controls.Remove(page5);
            panel1.Controls.Add(page3);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Controls.Remove(page2);
            panel1.Controls.Remove(page3);
            panel1.Controls.Remove(page5);
            panel1.Controls.Add(page4);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel1.Controls.Remove(page2);
            panel1.Controls.Remove(page3);
            panel1.Controls.Remove(page4);
            panel1.Controls.Add(page5);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != MessageBox.Show("프로그램을 종료합니다."))
                Environment.Exit(0);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            Connect();
            int count = 0;
            if (listBox1.SelectedItem != null)
            {
                string[] order = listBox1.SelectedItem.ToString().Split('\t');
                if (order[5] == "결제완료" || order[5] == "X")
                {
                    cmd.CommandText = $"SELECT INVENTORY_COUNT FROM INVENTORY WHERE INVENTORY_NAME = '{order[2]}'";
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        count = Convert.ToInt32(rdr["INVENTORY_COUNT"]);
                    }
                    if (int.Parse(order[3]) <= count)
                    {
                        cmd.CommandText = $"UPDATE ORDER_INFO SET DELIVERY_STATUS='발송완료' WHERE ORDER_ID={order[0]}";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = $"UPDATE INVENTORY SET INVENTORY_COUNT=INVENTORY_COUNT-{int.Parse(order[3])} WHERE INVENTORY_NAME='{order[2]}'";
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("발송되었습니다.");
                        listBox1.Items.Clear();
                        cmd.CommandText = "SELECT * FROM ORDER_INFO ORDER BY ORDER_ID";
                        Select();
                        conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("완재품 재고가 부족합니다.");
                    }
                }                 
                else
                    MessageBox.Show("이미 발송되었습니다.");
            }
            else
                MessageBox.Show("주문을 선택해주세요.");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Connect();
            listBox1.Items.Clear();
            if (comboBox1.SelectedIndex == 0)
            {
                cmd.CommandText = $"SELECT * FROM ORDER_INFO ORDER BY ORDER_ID";
            }
            else
            {
                cmd.CommandText = $"SELECT * FROM ORDER_INFO WHERE CLIENT_NAME='{comboBox1.SelectedItem}' ORDER BY ORDER_ID";
            }
            Select();
            conn.Close();
        }
     
    }
}
