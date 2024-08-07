using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SolidWorks.Interop.swdocumentmgr;
using SolidWorks.Interop.sldworks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SolidWorks.Interop.swconst;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using System.Data.SqlClient;


namespace TEST2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static SldWorks SwApp;
        private static AssemblyDoc swAssembly;
        private static ModelDoc2 swModel;


        public static ISldWorks ConnectToSolidWorks()
        {
            if (SwApp != null)
            {
                return SwApp;
            }
            else
            {
                Debug.Print("connect to solidworks on " + DateTime.Now);
                try
                {
                    SwApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application");
                }
                catch (COMException)
                {
                    try
                    {
                        SwApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application.23"); // 2015
                    }
                    catch (COMException)
                    {
                        try
                        {
                            SwApp = (SldWorks)Marshal.GetActiveObject("SldWorks.Application.26"); // 2018
                        }
                        catch (COMException)
                        {
                            MessageBox.Show("Could not connect to SolidWorks.", "SolidWorks", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            SwApp = null;
                        }
                    }
                }

                return SwApp;
            }
        }

       



        //运动单位为m 因此1mm挪动应该为0.01
        // thetax 是角度 绕某个轴旋转  Distance是位移 绕某个轴平移 scale是放缩
        private void MovePart(string partName,double thetax,double thetay,double thetaz, double xDistance, double yDistance, double zDistance,double scale)
        {
            if (swAssembly == null)
            {
                MessageBox.Show("swAssembly is null");
                return;
            }

            // 获取部件
            Component2 swComponent = swAssembly.GetComponentByName(partName);
            if (swComponent != null)
            {
                // 获取IMathUtility对象
                IMathUtility mathUtil = (IMathUtility)SwApp.GetMathUtility();

                // 获取当前变换矩阵
                IMathTransform componentTransform = swComponent.Transform2;

                if (componentTransform == null)
                {
                    MessageBox.Show("Component Transform is null");
                    return;
                }

                // 获取当前变换矩阵的数据
                double[] transformData = (double[])componentTransform.ArrayData;
                // 修改平移部分的值
                // X轴旋转
                double cosThetax = Math.Cos(thetax/ 57.295779513);
                double sinThetax = Math.Sin(thetax/ 57.295779513);
                double cosThetay = Math.Cos(thetay / 57.295779513);
                double sinThetay = Math.Sin(thetay / 57.295779513);
                double cosThetaz = Math.Cos(thetaz / 57.295779513);
                double sinThetaz = Math.Sin(thetaz / 57.295779513);

                double[,] matrixx = new double[,]
                {
                    {1, 0, 0},
                    {0, cosThetax, -sinThetax},
                    {0, sinThetax, cosThetax}
                };

                double[,] matrixy = new double[,]
                {
                    {cosThetay, 0, sinThetay},
                    {0, 1, 0},
                    {-sinThetay, 0, cosThetay}
                };

                double[,] matrixz = new double[,]
                {
                    {cosThetaz, -sinThetaz, 0},
                    {sinThetaz, cosThetaz, 0},
                    {0, 0, 1}
                };


                double[,] intermediateResult = MultiplyMatrices(matrixx, matrixy);
                double[,] finalResult = MultiplyMatrices(intermediateResult, matrixz);
                int index = 0;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        transformData[index] = finalResult[i, j];
                        index++;
                    }
                }
                transformData[9] += xDistance; // X轴平移
                transformData[10] += yDistance; // Y轴平移
                transformData[11] += zDistance; // Z轴平移
                transformData[12] = scale;
                // 创建新的变换矩阵并设置修改后的数据
                IMathTransform newTransform = (IMathTransform)mathUtil.CreateTransform(transformData);

                // 将新的变换矩阵应用到部件上
                swComponent.Transform2 = (MathTransform)newTransform;

                swModel.GraphicsRedraw2();
            }
            else
            {
                MessageBox.Show("未找到部件: " + partName);
            }
        }





        private void ListComponents()
        {
            if (swAssembly == null)
            {
                MessageBox.Show("swAssembly is null");
                return;
            }

            int componentCount = swAssembly.GetComponentCount(false);
            object[] components = (object[])swAssembly.GetComponents(false);

            StringBuilder componentNames = new StringBuilder();
            foreach (Component2 component in components)
            {
                componentNames.AppendLine(component.Name2);
            }

            MessageBox.Show(componentNames.ToString(), "Component Names");
        }


        private async void RotatePartContinuously(string partName, double thetax, double thetay, double thetaz, double xDistance, double yDistance, double zDistance, double scale, int intervalMs)
        {

            for(int j =0; j < 360;j = j+ 5)
            {
                MovePart(partName, j, 0, 0, xDistance, yDistance, zDistance, scale);
                await Task.Delay(100);
                if (CheckForInterference())
                {
                    Console.WriteLine("yes");

                }
                else
                    Console.WriteLine("no");
            }
        }
        static double[,] MultiplyMatrices(double[,] matrix1, double[,] matrix2)
        {
            double[,] result = new double[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        result[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                }
            }

            return result;
        }
        //test
        private void MoveBed_button_Click(object sender, EventArgs e)
        {
            // 尝试移动部件
            string partName = "bed2-1"; // 确保这是装配体中实际存在的部件名称
            string partName1 = "gantry-1";
            double theta = 5; // 每次旋转的角度
            int axis = 1; // 旋转轴
            double xDistance = 0; // X 轴平移
            double yDistance = 0; // Y 轴平移
            double zDistance = 0; // Z 轴平移
            double scale = 1; // 缩放因子
            int intervalMs = 1000; // 每次旋转的时间间隔（毫秒）
            MovePart(partName1, 0, 0, 30, xDistance, yDistance, zDistance, scale);

            // RotatePartContinuously(partName, 0,0,0, xDistance, yDistance, zDistance, scale, intervalMs);
        }
        //碰撞检测
        public bool CheckForInterference()
        {
            ModelDoc2 swModelDoc = (ModelDoc2)SwApp.ActiveDoc;
            if (swModelDoc == null)
            {
                Console.WriteLine("请打开一个装配体文档.");
                return false;
            }

            AssemblyDoc swAssemblyDoc = (AssemblyDoc)swModelDoc;
            InterferenceDetectionMgr pIntMgr = swAssemblyDoc.InterferenceDetectionManager;

            // 设置干扰检测选项
            pIntMgr.TreatCoincidenceAsInterference = false;
            pIntMgr.TreatSubAssembliesAsComponents = true;
            pIntMgr.IncludeMultibodyPartInterferences = true;
            pIntMgr.MakeInterferingPartsTransparent = false;
            pIntMgr.CreateFastenersFolder = true;
            pIntMgr.IgnoreHiddenBodies = true;
            pIntMgr.ShowIgnoredInterferences = false;
            pIntMgr.UseTransform = true;

            // 设置非干扰部件的显示方式
            pIntMgr.NonInterferingComponentDisplay = (int)swNonInterferingComponentDisplay_e.swNonInterferingComponentDisplay_Wireframe;

            // 运行干扰检测
            object[] vInts = (object[])pIntMgr.GetInterferences();
            int interferenceCount = pIntMgr.GetInterferenceCount();

            // 停止干扰检测
            pIntMgr.Done();

            // 如果干扰数量大于0，则返回true，否则返回false
            return interferenceCount > 0;
        }


        private void Connect_button_Click(object sender, EventArgs e)
        {
            ISldWorks swApp = Form1.ConnectToSolidWorks();

            if (swApp != null)
            {
                string msg = "This message from C#. SolidWorks version is " + swApp.RevisionNumber();

                swApp.SendMsgToUser(msg);
                // 获取当前活动的模型文档
                swModel = (ModelDoc2)swApp.ActiveDoc;

                if (swModel == null)
                {
                    MessageBox.Show("No active document found.");
                    return;
                }

                // 检查当前文档是否为装配体
                if (swModel.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
                {
                    swAssembly = (AssemblyDoc)swModel;
                }
                else
                {
                    MessageBox.Show("The active document is not an assembly.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Failed to connect to SolidWorks.");
            }
        }
    }

    class DatabaseHelper : IDisposable
    {
        private string connectionString;
        private SqlConnection connection;

        // 构造函数，初始化连接字符串并打开连接
        public DatabaseHelper(string serverName, string databaseName, string username, string password)
        {
            connectionString = $"Server={serverName};Database={databaseName};User Id={username};Password={password};";
            connection = new SqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("数据库连接成功。");
        }

        // 插入数据的函数，传入八个参数
        public void InsertData(float gantry, float rx, float ry, float rz, float x, float y, float z, bool crash)
        {
            string insertQuery = "INSERT INTO TSS_1 (Gantry, Rx, Ry, Rz, X, Y, Z, Crash) VALUES (@Gantry, @Rx, @Ry, @Rz, @X, @Y, @Z, @Crash)";

            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Gantry", gantry);
                command.Parameters.AddWithValue("@Rx", rx);
                command.Parameters.AddWithValue("@Ry", ry);
                command.Parameters.AddWithValue("@Rz", rz);
                command.Parameters.AddWithValue("@X", x);
                command.Parameters.AddWithValue("@Y", y);
                command.Parameters.AddWithValue("@Z", z);
                command.Parameters.AddWithValue("@Crash", crash);

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} 行插入成功。");
            }
        }

        // 释放资源
        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                Console.WriteLine("数据库连接已关闭。");
            }
        }
    }

}
