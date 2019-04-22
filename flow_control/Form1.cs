using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.IO;


namespace flow_control
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        public double T = 7200;
        public double F;
        public int entrance_cap = 600;
        public int transfer_cap = 500;

        public int period_length = 180;
        public int K ;
        public int[] walk_time = new int[] { 900, 600 };//种类与时间



        public int[] X;
        
        public int[,,,,] y;//时间，原线路，原方向，换乘后线路，换乘后方向
        public int[,,] Y;//时间，线路，线路


        //Passenger
        public int[] k;//
        public int[] passengers_sum;//各个线路方向总人数
        public int[][,] pas_line_dir;//分线路方向的人数，线路、方向
        public int[][,] get_off_num;//分线路方向的下车数，线路、方向

        //Station
        public int Count_lines = 2;
        public int ini_platfrom_num = 0;
        public int[] line_id;
        public int[] line_dir = { 0, 1 };
        public bool is_transfer = true;
        public bool station_position;

        //Entrance
        public int[] arrive_ent;//累计值，总值
        public int[][,] arrive_ent_line;//累计值，区分线路与方向
        public int[] depart_ent;//累计值，总值
        public int[][,] depart_ent_line;//累计值，区分线路与方向
        public int[] queue_ent;//累计值，总值
        public int[][,] queue_ent_line;//累计值，区分线路与方向

        //Platform
        public int[] arrive_plat;//累计值，总值
        public int[][,] arrive_plat_line;//累计值，区分线路与方向
        public int[] depart_plat;//累计值，总值
        public int[][,] depart_plat_line;//累计值，区分线路与方向
        public int[] queue_plat;//累计值，总值
        public int[][,] queue_plat_line;//累计值，区分线路与方向 
        public int[][,] queue_plat_train;//累计等车人数，与等待值不同，有区别

        //Train
        //？？？？？？车次与间隔还有问题
        public int[] train_id;//车次与时间间隔的关系
        public int[][,] train_schedule;//线路，方向，存车次信息
        //public int line_id;
        //public int line_dir; //line_dir=0,下行；line_dir=1,上行
        public int[][,] arrive_time;//[k][线路，方向]，存时间
        public int[][,] depart_time;//[k][线路，方向]，存时间
        public int[][,] dwell_time;//[k][线路，方向]，存时间
        public int ini_passengers = 500;//到站时列车内的人数
        public int train_capacity = 1400;//额定
        public int[][,] get_on_num;//上车[k][线路，方向]，存人数
        public int train_num = 40;//总数

        //Transfer
        public int[][,] transfer_line;//需求，[k][线路，线路]，线路间关系,线路，线路
        public int[][,,,] transfer_line_dir;//需求，线路方向与线路方向的关系
        public int[] Arrive_Transfer;
        public int[][,] arrive_transfer;
        public int[][,,,] arrive_transfer_dir;
        public int[] Depart_Transfer;
        public int[][,] depart_transfer;//累计
        public int[][,,,] depart_transfer_dir;//线路方向与线路方向的关系
        public int[][,] queue_transfer;
        public int[][,,,] queue_transfer_dir;//线路方向与线路方向的关系

        //Time
        //瞎改不确定
        public int[][] t_wait;//节点,0进站
        public int[][] T_wait;
        public int[] T_Wait;
        public int[][] t_walk;//节点
        public int[][] T_walk;
        public int[] T_Walk;



        public void Initialize()
        {
            K = (int)(T / period_length);
            int[] X = new int[K+1];
            int[,,,,] y = new int[K + 1, Count_lines, 2, Count_lines, 2];//时间，原线路，原方向，换乘后线路，换乘后方向
            int[,,] Y = new int[K + 1, Count_lines, Count_lines];//时间，线路，线路

            //Passenger
            int[] k = new int[K + 1];//
            int[] passengers_sum = new int[K + 1];//各个线路方向总人数
            int[][,] pas_line_dir = new int[K + 1][,];//分线路方向的人数，线路、方向
            for (int i = 0; i <= K; i++)
            {
                pas_line_dir[i] = new int[Count_lines, 2];
            }
            int[][,] get_off_num = new int[K + 1][,];//分线路方向的下车数，线路、方向
            for (int i = 0; i <= K; i++)
            {
                get_off_num[i] = new int[Count_lines, 2];
            }
           
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        if (m == 0 & n == 0)
                        {
                            get_off_num[i][m, n] = 25;
                        }
                        if (m == 0 & n == 1)
                        {
                            get_off_num[i][m, n] = 12;
                        }
                        if (m == 1 & n == 0)
                        {
                            get_off_num[i][m, n] = 12;
                        }
                        if (m == 1 & n == 1)
                        {
                            get_off_num [i][m, n] = 55;
                        }
                    }
                }
            }

            //Station

            int[] line_id = new int[Count_lines];

            //Entrance
            int[] arrive_ent = new int[K + 1];//累计值，总值
            int[][,] arrive_ent_line = new int[K + 1][,];//累计值，区分线路与方向
            for (int i=0;i<=K; i++)
            {
                arrive_ent_line[i] = new int[Count_lines, 2];
            }
            int[] depart_ent = new int[K + 1];//累计值，总值
            int[][,] depart_ent_line = new int[K + 1][,];//累计值，区分线路与方向
            for (int i = 0; i <= K; i++)
            {
                depart_ent_line[i] = new int[Count_lines, 2];
            }
            int[] queue_ent = new int[K + 1];//累计值，总值
            int[][,] queue_ent_line = new int[K + 1][,];//累计值，区分线路与方向
            for (int i = 0; i <= K; i++)
            {
                queue_ent_line[i] = new int[Count_lines, 2];
            }

            //Platform
            int[] arrive_plat = new int[K + 1];//累计值，总值
            int[][,] arrive_plat_line = new int[K + 1][,];//累计值，区分线路与方向
            for (int i = 0; i <= K; i++)
            {
                arrive_plat_line[i] = new int[Count_lines, 2];
            }
            int[] depart_plat = new int[K + 1];//累计值，总值
            int[][,] depart_plat_line = new int[K + 1][,];//累计值，区分线路与方向
            for (int i = 0; i <= K; i++)
            {
                depart_plat_line[i] = new int[Count_lines, 2];
            }
            int[] queue_plat = new int[K + 1];//累计值，总值
            int[][,] queue_plat_line = new int[K + 1][,];//累计值，区分线路与方向 
            for (int i = 0; i <= K; i++)
            {
                queue_plat_line[i] = new int[Count_lines, 2];
            }
            int[][,] queue_plat_train = new int[K + 1][,];
            for (int i = 0; i <= K; i++)
            {
                queue_plat_train[i] = new int[Count_lines, 2];
            }

            //Train
            //？？？？？？车次与间隔还有问题
            int[] train_id = new int[K + 1];//车次与时间间隔的关系
            int[][,] train_schedule = new int[K + 1][,];//线路，方向，存车次信息
            for (int i = 0; i <= K; i++)
            {
                train_schedule[i] = new int[Count_lines, 2];
            }
            int[][,] arrive_time = new int[K + 1][,];//[k][线路，方向]，存时间
            for (int i = 0; i <= K; i++)
            {
                arrive_time[i] = new int[Count_lines, 2];
            }
            int[][,] depart_time = new int[K + 1][,];//[k][线路，方向]，存时间
            for (int i = 0; i <= K; i++)
            {
                depart_time[i] = new int[Count_lines, 2];
            }
            int[][,] dwell_time = new int[K + 1][,];//[k][线路，方向]，存时间
            for (int i = 0; i <= K; i++)
            {
                dwell_time[i] = new int[Count_lines, 2];
            }


            int[][,] get_on_num = new int[K + 1][,];//上车[k][线路，方向]，存人数
            for (int i = 0; i <= K; i++)
            {
                get_on_num[i] = new int[Count_lines, 2];
            }


            //Transfer
            int[][,] transfer_line = new int[K + 1][,];//需求，[k][线路，线路]，线路间关系,线路，线路
            for (int i = 0; i <= K; i++)
            {
                transfer_line[i] = new int[Count_lines, Count_lines];
            }
            int[][,,,] transfer_line_dir = new int[K + 1][,,,];//需求，线路方向与线路方向的关系
            for (int i = 0; i <= K; i++)
            {
                transfer_line_dir[i] = new int[Count_lines, 2, Count_lines, 2];
            }
            int[] Arrive_Transfer = new int[K + 1];
            int[][,] arrive_transfer = new int[K + 1][,];
            for (int i = 0; i <= K; i++)
            {
                arrive_transfer[i] = new int[Count_lines, 2];
            }

            int[][,,,] arrive_transfer_dir = new int[K + 1][,,,];
            for (int i = 0; i <= K; i++)
            {
                arrive_transfer_dir[i] = new int[Count_lines, 2,Count_lines ,2];
            }
            int[] Depart_Transfer = new int[K + 1];
            int[][,] depart_transfer = new int[K + 1][,];//累计
            for (int i = 0; i <= K; i++)
            {
                depart_transfer[i] = new int[Count_lines, 2];
            }
            int[][,,,] depart_transfer_dir = new int[K + 1][,,,];//线路方向与线路方向的关系
            for (int i = 0; i <= K; i++)
            {
                depart_transfer_dir[i] = new int[Count_lines, 2,Count_lines ,2];
            }
            int[][,] queue_transfer = new int[K + 1][,];
            for (int i = 0; i <= K; i++)
            {
                queue_transfer[i] = new int[Count_lines, 2];
            }
            int[][,,,] queue_transfer_dir = new int[K + 1][,,,];//线路方向与线路方向的关系
            for (int i = 0; i <= K; i++)
            {
                queue_transfer_dir[i] = new int[Count_lines, 2,Count_lines,2];
            }

            //Time
            //瞎改不确定
            int[][] t_wait = new int[K + 1][];//节点,0进站
            for (int i = 0; i <= K; i++)
            {
                t_wait[i] = new int[ 3];////?????2
            }
            int[][] T_wait = new int[K + 1][];
            for (int i = 0; i <= K; i++)
            {
               T_wait[i] = new int[ 3];
            }
            int[] T_Wait = new int[K + 1];
            int[][] t_walk = new int[K + 1][];//节点
            for (int i = 0; i <= K; i++)
            {
                t_walk[i] = new int[ 3];
            }
            int[][] T_walk = new int[K + 1][];
            for (int i = 0; i <= K; i++)
            {
                T_walk[i] = new int[ 3];
            }
            int[] T_Walk = new int[K + 1];






            //Random rand = new Random(Guid.NewGuid().GetHashCode());
            //int X = rand.Next(250, 300);

            //FileStream fs = new FileStream("C:\\Users\\DELL\\Desktop\flow_control\\passenger -1337.csv", FileMode.Open, FileAccess.Read, FileShare.None);
            //StreamReader sr = new StreamReader(fs, Encoding.GetEncoding(936));

            //string str = "";
            //string s = Console.ReadLine();
            //while (str != null)
            //{
            //    str = sr.ReadLine();
            //    string[] train_time = new String[40];
            //    train_time = str.Split(',');
            //    string ser = train_time[0];
            //    string dse = train_time[1];
            //    if (ser == s)
            //    {
            //        Console.WriteLine(dse); break;
            //    }
            //}
            //sr.Close();//借鉴


            //for (int i = 0;i <= K;i++)
            //{
            //    Entrance entrance = new Entrance();
            //    Entrances.Add(entrance);
            //}

            //Station station = new Station();

            //for (int i = 0; i <= K; i++)
            //{
            //    Passenger passenger = new Passenger();
            //    Passengers.Add(passenger);

            //}

            //以下均为测试用
           
            for (int i = 0; i <= K; i++)
            {
                if (i < 2)
                {
                    X[i] = 230;
                }
                else
                {
                    X[i] = 250;
                }
            }

            //时间，原线路，原方向，换乘后线路，换乘后方向，public int[,,,,] y
            for (int i = 0; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)
                {
                    for (int a = 0; a < 2; a++)
                    {
                        for (int n = 0; n < Count_lines; n++)
                        {
                            for(int b = 0; b < 2; b++)
                            {
                                if (m == n )
                                {
                                    y[i, m, a, n, b] = 0;
                                }
                                else
                                {
                                    y[i, m, a, n, b] = 0;
                                }
                            }
                        }
                    }
                }

            }//测试用

            for (int i = 1; i <= K; i++)
            {
                for(int m = 0; m < Count_lines; m++)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        if (m == 0 & n == 0)
                        {
                            pas_line_dir[i][m, n] = 82;
                        }
                        if (m == 0 & n == 1)
                        {
                            pas_line_dir[i][m, n] = 27;
                        }
                        if (m == 1 & n == 0)
                        {
                            pas_line_dir[i][m, n] = 38;
                        }
                        if (m == 1 & n == 1)
                        {
                            pas_line_dir[i][m, n] = 155;
                        }
                    }
                }                
            }

            for (int i = 1; i <= K; i++)
            {
                passengers_sum[i] = 302;
            }

            //进站口
            //公式1


            //到达进站排队节点的人数,公式2
            for (int i = 1;i <= K;i++)
            {
                arrive_ent[0] = 0;//初始值
                arrive_ent[i] = arrive_ent[i - 1] + passengers_sum[i]; 
            }

            //按线路方向区分，公式3
            for (int i = 1; i <= K; i++)//同时间
            {
                for (int m = 0; m < Count_lines; m++)//同线路号
                {
                    for (int n = 0; n < 2; n++)//同方向
                    {
                        arrive_ent_line[0][m, n] = 0;//初始
                        arrive_ent_line[i][ m, n] = arrive_ent_line[i - 1][m,n] + pas_line_dir[i][m,n];
                        
                    }
                }
            }

            //公式4，离开节点人数
            for (int i = 1; i <= K;i++)
            {
                depart_ent[0] = 0;//初始
                depart_ent[i] = depart_ent[i - 1] + X[i]; 
            }
           
            //公式5，排队人数
            for(int i = 1; i <= K; i++)
            {
                queue_ent[i] = arrive_ent[i] - depart_ent[i];
            }

            //公式6，区分线路方向
            for(int i = 1;i <= K;i++)//同时
            {
                for (int m = 0; m < Count_lines; m++)//同线
                {
                    for (int n = 0; n < 2; n++)//同方向
                    {
                        queue_ent_line[0][m, n] = 0;
                        depart_ent_line[i][ m, n] = depart_ent[i] * (queue_ent_line[i - 1][m,n] + pas_line_dir[i][m,n]) / (queue_ent[i - 1] + passengers_sum[i ]); 
                        
                    }
                }
            }

            //公式7，区分线路方向
            for (int i = 1; i <= K; i++)//同时
            {
                for (int m = 1; m < Count_lines; m++)//同线
                {
                    for (int n = 0; n < 2; n++)//同方向
                    {
                        depart_ent_line[i][m,n] = arrive_ent_line[i][m,n] - depart_ent_line[i][m,n];
                        
                    }
                }
            }

            //公式8，逻辑约束
            for (int i = 1; i <= K;i++)
            {
                queue_ent[0] = 0;
                if (X[i] < (queue_ent[i - 1] + arrive_ent[i] - arrive_ent[i - 1]))
                {
                    X[i] = X[i];
                }
                else
                {
                    X[i] = queue_ent[i - 1] + arrive_ent[i] - arrive_ent[i - 1];
                }
            }

            //站台
            

            //公式9,到站台,换乘站
            //测试
            for (int i = 1; i <= K; i++)//同时
            {
                for (int m = 0; m < Count_lines; m++)//换乘后线
                {
                    for (int n = 0; n < 2; n++)//换后方向
                    {
                        Y[i, m, n] = 0;                     
                    }
                }
            }

            for (int i = 1; i <= K; i++)//同时
            {
                for (int m = 0; m < Count_lines; m++)//换乘后线
                {
                    for (int n = 0; n < 2; n++)//换后方向
                    {
                        for (int a = 0; a < Count_lines; a++)
                        {
                            for (int b = 0; b < 2; b++)
                            {
                                Y[i, m, n] = Y[i, m, n] + y[i, a, b, m, n];
         
                            }

                        }

                    }
                }
            }
            //测试待删

            if (is_transfer)
            {
                for (int i = 1; i <= K; i++)//同时
                {
                    for (int m = 0; m < Count_lines; m++)//同线
                    {
                        for (int n = 0; n < 2; n++)//同方向
                        {
                            for(int c = 0;c < Count_lines; c++)
                            {for (int d = 0; d < 2; d++)
                                {
                                    arrive_plat_line[0][m, n] = 0;//初始
                                    int a = i - walk_time[0] / (int)period_length;
                                    int b = i - walk_time[1] / (int)period_length;
                                    arrive_plat_line[i][m, n] = arrive_plat_line[i - 1][m, n] + depart_ent_line[i - a][m, n] +Y[i,m,n];                                 
                                }
                            }                           
                        }
                    }
                }                    
            }
            else//普通站
            {
                    for (int i = 1; i <= K; i++)//同时
                    {
                        for (int m = 1; m <= Count_lines; m++)//同线
                        {
                            for (int n = 0; n < 2; n++)//同方向
                            {
                                arrive_plat[0] = 0;
                                int a = i - walk_time[0] / (int)period_length;
                                arrive_plat_line[i][m,n] = arrive_plat_line[i - 1][m, n] + depart_ent_line[i - a][ m, n];
                            }
                        }
                    }               
            }

            //公式11，上车
           

            //测试

            for (int i = 1; i <= K; i++)
            {
                //train_id[0] = 70000;
                train_schedule[0][0, 0] = 70000;
                arrive_time[0][0, 0] = 0;
                dwell_time[0][0, 0] = 0;
                depart_time[0][0, 0] = 0;
                //train_id[K] = 70100;
                train_schedule[0][0, 1] = 70100;
                arrive_time[0][0, 1] = 0;
                dwell_time[0][0, 1] = 0;
                depart_time[0][0, 1] = 0;
                //train_id[2 * K] = 71100;
                train_schedule[0][1, 1] = 71100;
                arrive_time[0][1, 1] = 0;
                dwell_time[0][1, 1] = 0;
                depart_time[0][1, 1] = 0;
                //train_id[3 * K] = 71100;
                train_schedule[0][1, 0] = 71100;
                arrive_time[0][1, 0] = 0;
                dwell_time[0][1, 0] = 0;
                depart_time[0][1, 0] = 0;

                //train_id[i] = 70000 + i;
                train_schedule[i][0, 0] = train_schedule[i - 1][0, 0] + i;
                arrive_time[i][0, 0] = arrive_time[i - 1][0, 0] + 180;
                dwell_time[i][0, 0] = 60;
                depart_time[i][0, 0] = depart_time[i - 1][0, 0] + 180;
                //train_id[i] = 70100 + i - K;
                train_schedule[i][0, 1] = train_schedule[i - 1][0, 1] + i;
                arrive_time[i][0, 1] = arrive_time[i - 1][0, 1] + 180;
                dwell_time[i][0, 1] = 60;
                depart_time[i][0, 1] = depart_time[i - 1][0, 1] + 180;
                //train_id[i] = 71100 + i - 2 * K;
                train_schedule[i][1, 1] = train_schedule[i - 1][1, 1] + i;
                arrive_time[i][1, 1] = arrive_time[i - 1][1, 1] + 180;
                dwell_time[i][1, 1] = 60;
                depart_time[i][1, 1] = depart_time[i - 1][1, 1] + 180;
                //train_id[i] = 71100 + i - 3 * K;
                train_schedule[i][1, 0] = train_schedule[i - 1][1, 0] + i;
                arrive_time[i][1, 0] = arrive_time[i - 1][1, 0] + 180;
                dwell_time[i][1, 0] = 60;
                depart_time[i][1, 0] = depart_time[i - 1][1, 0] + 180;

            }

            
            //前面建立车次时已经和时间间隔建立了联系
            
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)//同线
                {
                    for (int n = 0; n < 2; n++)//同方向
                    {
                        depart_plat_line[0][m, n] = 0;
                        queue_plat_train[1][m, n] = arrive_plat_line[1][m, n] - depart_plat_line[1 - 1][m, n];
                        get_on_num[i][m, n] = Math.Min(train_capacity - ini_passengers + get_off_num[i][m, n], queue_plat_train[(depart_time[i][m, n] / period_length)][m, n]);                                               
                        //get_on_num[1][m, n] = Math.Min(train_capacity - ini_passengers + get_off_num[1][m, n], queue_plat_train[1][m, n]);                        
                        depart_plat_line[1][m, n] = depart_plat_line[1 - 1][m, n] + get_on_num[1][m, n];
                    }
                }
            }
            
                                               
            //公式13，离开即上车,累计
            
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        depart_plat_line[i][m, n] = depart_plat_line[i - 1][m, n] + get_on_num[i][m, n];
                    }
                }                
            }

            //公式10，排队等待k时乘车离开
            for (int i = 1; i <= K; i++)
            {
                for(int m = 0; m < Count_lines; m++)
                {
                    for (int n = 0; n < 2; n++)//同方向
                    {
                        queue_plat_train[i][m, n] = arrive_plat_line[i][m, n] - depart_plat_line[i - 1][m, n];                
                    }
                }     
            }

            //公式15，排队
            for(int i = 1;i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)
                {
                    for (int n = 0; n < 2; n++)//同方向
                    {
                        queue_plat_line[i][m, n] = arrive_plat_line[i][m, n] - depart_plat_line[i][m, n];
                    }
                }
            }

            //公式14，累计总排队
            for (int i = 0; i <= K; i++)
            {
                queue_plat[i] = 0;
            }

            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)
                {
                    for (int n = 0; n < 2; n++)//同方向
                    {

                        queue_plat[i] = queue_plat[i] + queue_plat_line[i][m, n];
                       
                    }
                }
            }

            //换乘排队节点
            //公式16,确定站点时，此处为已知

            //公式17，换乘，线路到另一线路          

            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)//线路
                {
                    for (int n = 0; n < Count_lines; n++)//线路
                    {
                        transfer_line[i][m, n] = transfer_line_dir[i][m, 0, n, 0] + transfer_line_dir[i][m, 0, n, 1] + transfer_line_dir[i][m, 1, n, 0] + transfer_line_dir[i][m, 1, n, 1];
                    }
                }
            }

            //公式18,到达
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)//线路
                {
                    for (int n = 0; n < Count_lines; n++)//线路
                    {
                        arrive_transfer[i][m, n] = arrive_transfer[i - 1][m, n] + transfer_line[i][m, n];
                    }
                }
            }
            //公式19
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)//线路
                {
                    for (int n = 0; n < Count_lines; n++)//线路
                    {
                        for (int a = 0; a < 2; a++)//方向
                        {
                            for (int b = 0; b < 2; b++)//方向
                            {
                                
                                arrive_transfer_dir[i][m,a,n,b] = arrive_transfer_dir[i - 1][m, a, n, b]+ transfer_line_dir[i][m,a,n,b];
                            }
                        }
                    }
                }
            }

            //公式20，离开
            for (int m = 0; m < Count_lines; m++)
            {
                for (int n = 0; n < Count_lines; n++)//线路
                {
                    for (int i = 1; i <= K; i++)//线路
                    {
                        depart_transfer[i][m, n] = depart_transfer[i - 1][m, n] + Y[i, m, n];
                    }
                }
            }

            //公式21，排队
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)//线路
                {
                    for (int n = 0; n < Count_lines; n++)//线路
                    {
                        queue_transfer[i][m,n] = arrive_transfer[i][m,n] + depart_transfer[i][m,n];
                    }
                }
            }

            //公式22
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)//线路
                {
                    for (int n = 0; n < Count_lines; n++)//线路
                    {
                        for (int a = 0; a < 2; a++)//方向
                        {
                            for (int b = 0; b < 2; b++)//方向
                            {
                                if (transfer_line[i - 1][m, n] == 0| queue_transfer[i][m, n]==0)
                                {
                                    y[i, m, a, n, b] = 0;

                                }
                                else
                                {
                                    y[i, m, a, n, b] = Y[i, m, n] * (queue_transfer_dir[i - 1][m, a, n, b] + transfer_line_dir[i][m, a, n, b]) / (queue_transfer[i][m, n] + transfer_line[i - 1][m, n]);

                                }//测试用
                            }
                        }
                    }
                }
            }

            //公式23
            for (int m = 0; m < Count_lines; m++)//线路
            {
                for (int n = 0; n < Count_lines; n++)//线路
                {
                    for (int a = 0; a < 2; a++)//方向
                    {
                        for (int b = 0; b < 2; b++)//方向
                        {
                            for (int i = 1; i <= K; i++)//时间
                            {

                                depart_transfer_dir[i][m, a, n, b] = depart_transfer_dir[i][m, a, n, b] + transfer_line_dir[i][m, a, n, b];

                            }
                        }
                    }
                }
            }
            
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)//线路
                {
                    for (int n = 0; n < Count_lines; n++)//线路
                    {
                        for (int a = 0; a < 2; a++)//方向
                        {
                            for (int b = 0; b < 2; b++)//方向
                            {
                                queue_transfer_dir[i][m, a, n, b] = arrive_transfer_dir[i][m, a, n, b] - arrive_transfer_dir[i][m, a, n, b];

                            }
                        }
                    }
                }
            }

            //公式24
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)//线路
                {
                    for (int n = 0; n < Count_lines; n++)//线路
                    {
                        Y[i,m,n]=Math.Min(queue_transfer[i - 1][m,n]+transfer_line[i][m,n],Y[i,m,n]);
                    }
                }
            }

            //公式25

            //公式26，进站、换乘、乘车排队
           
            for (int i = 1; i <= K; i++)
            {
                Arrive_Transfer[i] = 0;
            }

            for (int i = 1; i <= K; i++)
            {
                for(int m = 0; m < Count_lines; m++)
                {
                    for (int n = 0; n < Count_lines; n++)
                    {
                        Arrive_Transfer[i] = Arrive_Transfer[i] + arrive_transfer[i][m, n];
                    }                   
                }
           }

            for (int i = 1; i <= K; i++)
            {
                t_wait[i][0] = (arrive_ent[i] - depart_ent[i]) * period_length;
                t_wait[i][1] = (Arrive_Transfer[i]-Depart_Transfer[i]) * period_length;
                t_wait[i][2] = (arrive_plat[i]-depart_plat[i]) * period_length;
                
            }

            for (int j = 0; j < 3; j++)
            {
                for(int i = 1; i <= K; i++)
                {
                    T_wait[i][j] = T_wait[i - 1][ j] + t_wait[i][j];
                }
            }

            //公式27
            T_Wait[K] = T_wait[K][0] + T_wait[K][1] + T_wait[K][2];

            //公式28

            //公式29       

            //公式31，见公式27

            //公式32
            for (int i = 1; i <= K; i++)
            {
                t_walk[i][0] = walk_time[0] * (depart_ent[i] - depart_ent[i - 1]);
                t_walk[i][1] = walk_time[1] * (Depart_Transfer[i] - Depart_Transfer[i - 1]);
            }

            for (int i = 1; i <= K; i++)
            {
                T_walk[i][0] = T_walk[i - 1][0] + t_walk[i][0];
                T_walk[i][1] = T_walk[i - 1][1] + t_walk[i][1];
            }

            for(int i = 1; i <= K; i++)
            {
                T_Walk[i] = T_walk[i][0] + T_walk[i][1];
            }

            //公式30
            for (int i = 1; i <= K; i++)
            {
                F = (T_Walk[i] + T_Wait[i]) / (arrive_ent[i] + Arrive_Transfer[i]);
            }


            //公式33
            //F最小

            //公式34
            for (int i = 1; i <= K; i++)
            {
               if ( X[i] < entrance_cap)
                {
                    X[i] = X[i];
                }
               else
                {
                    X[i] = entrance_cap;//不确定
                }
            }

            //公式35
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)
                {
                    for(int n = 0; n < Count_lines; n++)
                    {
                        if (Y[i,m,n] < transfer_cap)
                        {
                            Y[i,m,n] = Y[i,m,n];
                        }
                        else
                        {
                            Y[i,m,n] = transfer_cap;//不确定
                        }
                    }
                }               
            }

            //公式36
            for (int i = 1; i <= K; i++)
            {
                if (X[i] > 0)
                {
                    X[i] = X[i];
                }
                else
                {
                    X[i] = -X[i];//不确定
                }
            }

            //公式37
            for (int i = 1; i <= K; i++)
            {
                for (int m = 0; m < Count_lines; m++)
                {
                    for (int n = 0; n < Count_lines; n++)
                    {
                        if (Y[i, m, n] > 0)
                        {
                            Y[i, m, n] = Y[i, m, n];
                        }
                        else
                        {
                            Y[i, m, n] = -Y[i,m,n];//不确定
                        }
                    }
                }
            }




       





        }

        public void Write()
        {
            FileStream fs = new FileStream("D:\\ak.txt", FileMode.Create);
            //获得字节数组
            byte[] data = Encoding.Default.GetBytes("Hello World!");
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
