using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyControls
{
    /// <summary>
    /// Dashboard.xaml 的交互逻辑
    /// </summary>
    public partial class Dashboard : UserControl
    {
        /// <summary>
        /// 依赖对象，依赖属性
        /// 背景色
        /// </summary>

        public Brush DashboardBackground
        {
            get { return (Brush)GetValue(DashboardBackgroundProperty); }
            set { SetValue(DashboardBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DashboardBackgroundProperty =
            DependencyProperty.Register("DashboardBackground", typeof(Brush), typeof(Dashboard),
                new PropertyMetadata(default(Brush)));
        /// <summary>
        /// 指针颜色
        /// </summary>
        public Brush PointerBackground
        {
            get { return (Brush)GetValue(PointerBackgroundProperty); }
            set { SetValue(PointerBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointerBackgroundProperty =
            DependencyProperty.Register("PointerBackground", typeof(Brush), typeof(Dashboard),
                new PropertyMetadata(default(Brush)));


        /// <summary>
        /// 仪表盘数值
        /// </summary>
        public double Value {
            get { return (double)GetValue(ValueProperty);} 
            set { SetValue(ValueProperty, value);}
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double),typeof(Dashboard),
                new PropertyMetadata(double.IsNaN(double.NaN)?0:double.NaN,new PropertyChangedCallback(OnPropertyChanged)));
        /// <summary>
        /// 仪表盘最小值
        /// </summary>
        public int MinNum
        {
            get { return (int)GetValue(MinNumProperty); }
            set { SetValue(MinNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinNumProperty =
            DependencyProperty.Register("MinNum", typeof(int), typeof(Dashboard), 
                new PropertyMetadata(default(int), new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// 仪表盘最大值
        /// </summary>
        public int MaxNum
        {
            get { return (int)GetValue(MaxNumProperty); }
            set { SetValue(MaxNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxNumProperty =
            DependencyProperty.Register("MaxNum", typeof(int), typeof(Dashboard),
                new PropertyMetadata(default(int), new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// 仪表盘区块数量
        /// </summary>
        public int BlockNum
        {
            get { return (int)GetValue(BlockNumProperty); }
            set { SetValue(BlockNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BlockNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlockNumProperty =
            DependencyProperty.Register("BlockNum", typeof(int), typeof(Dashboard),
                new PropertyMetadata(default(int), new PropertyChangedCallback(OnPropertyChanged)));


        /// <summary>
        /// 字体大小
        /// </summary>
        public int SizeNum
        {
            get { return (int)GetValue(SizeNumProperty); }
            set { SetValue(SizeNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SizeNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeNumProperty =
            DependencyProperty.Register("SizeNum", typeof(int), typeof(Dashboard),
                new PropertyMetadata(default(int), new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush FontColor
        {
            get { return (Brush)GetValue(FontColorProperty); }
            set { SetValue(FontColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SizeNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontColorProperty =
            DependencyProperty.Register("FontColor", typeof(Brush), typeof(Dashboard),
                new PropertyMetadata(default(Brush), new PropertyChangedCallback(OnPropertyChanged)));



        public static void OnPropertyChanged(DependencyObject sender,DependencyPropertyChangedEventArgs args)
        {
            (sender as Dashboard).Refrech();
        }
        public Dashboard()
        {
            InitializeComponent();
            this.SizeChanged += Dashboard_SizeChanged;
        }

        private void Dashboard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double minSize = Math.Min(this.RenderSize.Width, this.RenderSize.Height);
            this.backellipse.Width = minSize;
            this.backellipse.Height = minSize;
        }

        private void Refrech()
        {
            //得到园的半径
            double radius = this.backellipse.Width / 2;
            if (double.IsNaN(radius))
            {
                return;
            }
            //先清空
            this.dashboardcanvas.Children.Clear();
            //计算画刻度线 
            //起始刻度
            //int min = 0;
            //终止刻度
            //int max = 100;
            //区块数量
            //int block = 10;
            //区间值
            double siding = (this.MaxNum - this.MinNum) / this.BlockNum;
            //270度单个刻度所占角度
            double step = 270.0 / (this.MaxNum - this.MinNum);
            //循环画刻度线
            //从起始刻度记录循环j
            int j = this.MinNum;
            for (int i = 0; i < this.MaxNum - this.MinNum; i++)
            {
                //默认是画短线
                double linesize=13;
                if (i == 0 || i + 1 == this.MaxNum ||(i+1) % siding == 0)
                {
                    linesize = 25;

                    TextBlock textScale=new TextBlock();
                    textScale.Text = j == this.MinNum ? this.MinNum.ToString() : j + 1 == this.MaxNum ? this.MaxNum.ToString() : ((j + 1) / siding * siding).ToString(); 
                    textScale.Foreground=this.FontColor;
                    textScale.Width = 35;
                    textScale.FontSize= this.SizeNum;
                    textScale.TextAlignment= TextAlignment.Center;  

                    //设置横坐标（X）
                    Canvas.SetLeft(textScale, radius - (radius - 40) * Math.Cos((i * step - 45) * Math.PI / 180)-15);
                    //设置纵坐标（Y）
                    Canvas.SetTop(textScale,radius - (radius - 40) * Math.Sin((i * step - 45) * Math.PI / 180)- 10);

                    this.dashboardcanvas.Children.Add(textScale);
                }
                Line line= new Line();
                line.X1 = radius - (radius - linesize) * Math.Cos((i * step - 45) * Math.PI / 180);
                line.Y1 = radius - (radius - linesize) * Math.Sin((i * step - 45) * Math.PI / 180); 
                line.X2 = radius - (radius - 8) * Math.Cos((i * step - 45) * Math.PI / 180); 
                line.Y2 = radius - (radius - 8) * Math.Sin((i * step - 45) * Math.PI / 180); 
                 
                //设置线的颜色
                line.Stroke = this.FontColor;
                //设置线宽
                line.StrokeThickness = 1;   


                this.dashboardcanvas.Children.Add(line);
                j++;
            }
            //画仪表盘内部圆弧
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));

            string sData = "M{0} {1} A{0} {0} 0 1 1 {1} {2}";
            sData=string.Format(sData, radius / 2, radius, radius * 1.5); 
            this.circle.Data=(Geometry)converter.ConvertFrom(sData);
            //画仪表盘上指针
            sData = "M{0} {1},{1} {2},{1} {3}";
            sData = string.Format(sData, radius*0.3, radius, radius-5, radius +5);
            this.pointer.Data = (Geometry)converter.ConvertFrom(sData);
            //设置指针指向
            //this.agpointer.Angle = step * this.Value - 45;
            //设置指针指向(带动画效果)
            double amplitude = this.Value- this.MinNum <= 0 ? 0 : this.Value - this.MinNum >= (this.MaxNum - this.MinNum) ? (this.MaxNum - this.MinNum) : this.Value - this.MinNum;
            double Sngle = step * amplitude - 45;


            DoubleAnimation da= new DoubleAnimation(Sngle, new Duration(TimeSpan.FromMilliseconds(200)));
            this.agpointer.BeginAnimation(RotateTransform.AngleProperty, da);
        }
    }
}
