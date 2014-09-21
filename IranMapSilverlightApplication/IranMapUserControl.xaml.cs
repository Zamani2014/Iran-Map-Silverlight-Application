//-----------------------------------------------------------------------------------
// Created & Edited By Alireza Zamani                                         -----------------
//-----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xaml;
using System.Globalization;
using System.Threading;

namespace IranMapSilverlightApplication
{
    public partial class IranMapUserControl : UserControl
    {
        #region Variables
        // Variables that used
        char[] trimChars = new char[2] { '_', ' ' };
        private Point pt;
        private string lastSelected = string.Empty;
        private List<Links> _links = new List<Links>();
        private Dictionary<object, bool> provinceState = new Dictionary<object,bool>();
        private Dictionary<object, bool> cityState = new Dictionary<object, bool>();
        //private System.Threading.Thread mapThread = new System.Threading.Thread(new ThreadStart());
        /// <summary>
        /// Slider Existence Status.
        /// </summary>
        public bool SliderEnabaled = true;
        /// <summary>
        /// Province Color : 255-X-X-X.
        /// </summary>
        public string Color = string.Empty;
        /// <summary>
        /// Province Hover Color : 255-X-X-X.
        /// </summary>
        public string HoverColor = "255-100-255-0";
        /// <summary>
        /// Speed Ratio for Province Blinking.
        /// </summary>
        public int ProvinceSpeedRatio = 5;
        /// <summary>
        /// Speed Ratio for City Blinking.
        /// </summary>
        public int CitySpeedRatio = 5;
        /// <summary>
        /// Width of Map Control.
        /// </summary>
        public double width = 800;
        /// <summary>
        /// Height of Map Control
        /// </summary>
        public double height = 600;
        /// <summary>
        /// Opacity of Province Colors.
        /// </summary>
        public double mapOpacity = 0.4;
        /// <summary>
        /// Background Color
        /// </summary>
        public string BackColor = "255-255-255-255";
        /// <summary>
        /// Map Left Offset;
        /// </summary>
        public double LeftOffset;
        /// <summary>
        /// Enabling Zoom;
        /// </summary>
        public bool ZoomEnabled = true;
        #endregion

        #region Methods
        // Color Extractor for X-X-X-X
        private byte[] ColorExtractor(string color)
        {
            byte[] argb = new byte[4];
            string[] splitted = new string[4];
            char[] separator = new char[1];
            separator[0] = '-';

            splitted = color.Split(separator[0]);
            argb[0] = byte.Parse(splitted[0]);
            argb[1] = byte.Parse(splitted[1]);
            argb[2] = byte.Parse(splitted[2]);
            argb[3] = byte.Parse(splitted[3]);

            return argb;
        }

        // Constructor
        public IranMapUserControl()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.Loaded += UserControl_Loaded;
        }

        // Menu Method
        void HideMenu()
        {
            HidePopup.Begin();
            contextMenu.HorizontalOffset = -50.0;
        }

        // Menu Contents
        void SetupLinks()
        {
            //_links.Add(new Links("درباره مردم این شهر", "http://www.#.org/{0}.aspx"));
            //_links.Add(new Links("صنایع دستی", "http://www.#.com/a/{0}.htm"));
            //_links.Add(new Links("مناطق دیدنی و گردشگری", "http://www.#.com/charts/population1.aspx"));
            //_links.Add(new Links("آمارها", "http://www.#.com/cntry/{0}.aspx"));
            //_links.Add(new Links("جمعیت", "http://www.#.com/charts/population1.aspx"));
            //_links.Add(new Links("اطلاعات بیشتر در ویکی", "http://en.wikipedia.org/wiki/{0}"));
        }

        // Menu position on map
        private void PositionContextMenu(Point p, bool useTransition)
        {
            if (useTransition)
                contextMenu.IsOpen = false;
            contextMenu.HorizontalOffset = p.X;
            contextMenu.VerticalOffset = p.Y;
            contextMenu.IsOpen = true;
        }

        // Reset last selected canvas -Not Used
        void ResetLastSelected()
        {
            if (!string.IsNullOrEmpty(lastSelected))
            {
                Canvas selectedCanvas = this.FindName(lastSelected) as Canvas;
                SetCanvasColor(selectedCanvas, System.Windows.Media.Color.FromArgb(255, 250, 254, 255), 1, Colors.Black);
            }
        }

        // Change Canvas Color
        void SetCanvasColor(Canvas country, Color setColor, double strokeThickness, Color stroke)
        {
            Path mPath = new Path();
            mPath = country.Children[0] as Path;

            mPath.Opacity = mapOpacity;

            mPath.Stroke = new SolidColorBrush(stroke);
            mPath.Fill = new SolidColorBrush(setColor);
            mPath.StrokeThickness = strokeThickness;
        }

        // Open and Populate Menu Contents
        private void PopulateContextMenu(string country)
        {
            contextListBox.Items.Clear();

            foreach (Links l in _links)
            {
                HyperlinkButton hlb = new HyperlinkButton();
                hlb.Content = l.Title;
                hlb.NavigateUri = new Uri(string.Format(l.URL, country));
                hlb.TargetName = "_blank";

                contextListBox.Items.Add(hlb);
            }
        }

        // Province Colors
        private string getColor(string provinceName)
        {
            if (Color == string.Empty)
            {
                switch (provinceName)
                {
                    #region Province Colors
                    case "PersianGulfAndUmmanSea":
                        return "255-106-194-255";
                        break;
                    case "Alborz":
                        return "255-255-255-0";
                        break;
                    case "CaspianSea":
                        return "255-106-194-255";
                        break;
                    case "Kermanshah":
                        return "255-255-0-0";
                        break;

                    case "Kordestan":
                        return "255-0-147-47";
                        break;

                    case "Mazandaran":
                        return "255-200-15-160";
                        break;

                    case "Ghazvin":
                        return "255-0-200-0";
                        break;

                    case "Tehran":
                        return "255-0-150-255";
                        break;

                    case "Golestan":
                        return "255-255-80-0";
                        break;

                    case "KhorasanRazavi":
                        return "255-0-200-0";
                        break;

                    case "KhorasanJonoubi":
                        return "255-255-255-0";
                        break;

                    case "KhorasanShomali":
                        return "255-5-232-255";
                        break;

                    case "Semnan":
                        return "255-255-0-100";
                        break;

                    case "Yazd":
                        return "255-255-200-0";
                        break;

                    case "Kerman":
                        return "255-73-253-211";
                        break;

                    case "Hormozgan":
                        return "255-9-196-255";
                        break;

                    case "Systan":
                        return "255-148-215-255";
                        break;

                    case "AzarGharbi":
                        return "255-245-195-140";
                        break;

                    case "AzarSharghi":
                        return "255-255-121-232";
                        break;

                    case "Lorestan":
                        return "255-241-147-0";
                        break;

                    case "Boushehr":
                        return "255-241-147-0";
                        break;

                    case "Esfahan":
                        return "255-0-255-128";
                        break;

                    case "Fars":
                        return "255-103-0-255";
                        break;

                    case "Boyer":
                        return "255-0-43-255";
                        break;

                    case "Bakhtyari":
                        return "255-233-255-0";
                        break;

                    case "Khouzestan":
                        return "255-0-150-255";
                        break;

                    case "Ghom":
                        return "255-15-255-222  ";
                        break;

                    case "Markazi":
                        return "255-41-196-255";
                        break;

                    case "Hamedan":
                        return "255-90-15-200";
                        break;

                    case "Zanjan":
                        return "255-255-96-10";
                        break;

                    case "Gilan":
                        return "255-255-255-0";
                        break;

                    case "Ardebil":
                        return "255-35-15-210";
                        break;

                    case "Ilam":
                        return "255-128-53-134";
                        break;

                    case "Iran":
                        return "255-255-255-255";
                        break;
                    #endregion
                }
            }
            else
            {
                switch (provinceName)
                {
                    #region Province Colors
                    case "PersianGulfAndUmmanSea":
                        return "255-106-194-255";
                        break;
                    case "CaspianSea":
                        return "255-106-194-255";
                        break;
                    #endregion
                }
                return Color;
            }
            return "";
        }
        #endregion

        #region Events
        // On Loaded Event
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //this.BackColor = "255-226-20-219";

            byte[] argb = new byte[4];
            argb = ColorExtractor(BackColor);
            System.Windows.Media.Brush br = new SolidColorBrush(System.Windows.Media.Color.FromArgb(argb[0], argb[1], argb[2], argb[3]));
            this.zoomArea.Background = br;
            
            ColorPainter();
            SetupLinks();
            AddMapsEvent();
            LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);

            HtmlPage.Window.AttachEvent("DOMMouseScroll", OnMouseWheel);
            HtmlPage.Window.AttachEvent("onmousewheel", OnMouseWheel);
            HtmlPage.Document.AttachEvent("onmousewheel", OnMouseWheel);

            Loaded += new RoutedEventHandler(Page_Loaded);
            Loaded += new RoutedEventHandler(UserControl_Loaded);
            
            //this.LayoutRoot.Height = height; 
            //this.LayoutRoot.Width = width;
            //this.ZoomEnabled = false;
            // Tests :
            //this.SizeChanger(1.7);
            //Color = "0-0-0-235";
            //Color = "255-0-150-255";
            //HoverColor = "255-255-0-0";
            //this.Blinker("کردستان");
            //this.Blinker("تهران");
            //this.CityBlinker("بيرجند");
            //this.Kermanshah.Visibility = Visibility.Collapsed;

            //this.BlinkProvinceState.SetValue(Storyboard.TargetNameProperty, this.Kermanshah.Name);
            //this.BlinkProvinceState.Begin();
            //this.BlinkKermanshah.Begin();
        }

        // UserControl On Loaded Event
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SliderEnabaled)
            {
                this.zoomIn.Visibility = Visibility.Visible;
                this.zoomScale.Visibility = Visibility.Visible;
                this.zoomOut.Visibility = Visibility.Visible;
                this.zoomReset.Visibility = Visibility.Visible;
            }
            else
            {
                this.zoomIn.Visibility = Visibility.Collapsed;
                this.zoomScale.Visibility = Visibility.Collapsed;
                this.zoomOut.Visibility = Visibility.Collapsed;
                this.zoomReset.Visibility = Visibility.Collapsed;
            }
        }

        // Mouse Event
        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //ResetLastSelected();
            HideMenu();
        }

        // Mouse Events
        void AddMapsEvent()
        {
            foreach (Canvas c in (this.FindName("zoomArea") as Canvas).Children)
            {
                if (!string.IsNullOrEmpty(c.Name))
                {
                    c.Cursor = Cursors.Hand;
                    ToolTip toolTip = new ToolTip { Content = c.Name };
                    c.SetValue(ToolTipService.ToolTipProperty, toolTip);

                    c.MouseLeftButtonUp += new MouseButtonEventHandler(c_MouseLeftButtonUp);
                    c.MouseMove += new MouseEventHandler(c_MouseMove);
                    c.MouseLeave += new MouseEventHandler(c_MouseLeave);
                }
            }
        }

        // Mouse Event
        void c_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(lastSelected))
            {
                Canvas selectedCanvas = this.FindName(lastSelected) as Canvas;
                Path mPath = new Path();
                mPath = selectedCanvas.Children[0] as Path;
                string[] splitted = new string[4];
                char[] separator = new char[1];
                separator[0] = '-';
                string color = getColor(mPath.Name);

                splitted = color.Split(separator[0]);
                byte a = byte.Parse(splitted[0]);
                byte r = byte.Parse(splitted[1]);
                byte g = byte.Parse(splitted[2]);
                byte b = byte.Parse(splitted[3]);

                SetCanvasColor(selectedCanvas, System.Windows.Media.Color.FromArgb(a, r, g, b), 1, Colors.Black);

                if (provinceState.ContainsKey(mPath.Name.Trim(trimChars)))
                {
                    if (provinceState[mPath.Name])
                    {
                        Blinker(selectedCanvas.Name);
                    }
                }
            }
        }

        // Color Painter
        void ColorPainter()
        {
            Canvas childCanvas = new Canvas();
            Path mPath = new Path();

            for (int i = 1; i < zoomArea.Children.Count; i++)
            {
                childCanvas = zoomArea.Children[i] as Canvas;
                mPath = childCanvas.Children[0] as Path;

                string[] splitted = new string[4];
                char[] separator = new char[1];
                separator[0] = '-';
                string color = getColor(mPath.Name);

                splitted = color.Split(separator[0]);
                byte a = byte.Parse(splitted[0]);
                byte r = byte.Parse(splitted[1]);
                byte g = byte.Parse(splitted[2]);
                byte b = byte.Parse(splitted[3]);

                SetCanvasColor(childCanvas, System.Windows.Media.Color.FromArgb(a, r, g, b), 1, Colors.Black);

                for (int j = 1; j < childCanvas.Children.Count; j++)
                {
                    childCanvas.Children[j].Visibility = Visibility.Collapsed;
                }

                if (provinceState.ContainsKey(mPath.Name.Trim(trimChars)))
                {
                    if (provinceState[mPath.Name])
                    {
                        Blinker(childCanvas.Name);
                    }
                }

            }
        }

        // Mouse Event
        void c_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas c = sender as Canvas;
            //ResetLastSelected();

            if (!string.IsNullOrEmpty(c.Name))
            {
                if (c.Name != lastSelected)
                {
                    HideMenu();
                }

                lastSelected = c.Name;
                SetCanvasColor(c, System.Windows.Media.Color.FromArgb(ColorExtractor(HoverColor)[0], ColorExtractor(HoverColor)[1], ColorExtractor(HoverColor)[2], ColorExtractor(HoverColor)[3])
                    , 1, System.Windows.Media.Color.FromArgb(ColorExtractor(HoverColor)[0], ColorExtractor(HoverColor)[1], ColorExtractor(HoverColor)[2], ColorExtractor(HoverColor)[3]));
                //SetCanvasColor(c,Colors.Green, 1, Colors.Green);
            }
        }

        // Mouse Event
        void c_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Canvas c = sender as Canvas;
            //if (!string.IsNullOrEmpty(c.Name))
            //{
            //    PopulateContextMenu(c.Name);
            //    PositionContextMenu(e.GetPosition(contextMenu.Parent as UIElement), true);
            //    e.Handled = true;
            //}
        }

        // Menu Event
        private void contextMenu_Opened(object sender, EventArgs e)
        {
            ShowPopup.Begin();
        }

        // Menu Event
        private void contextMenu_Closed(object sender, EventArgs e)
        {
            HidePopup.Begin();
        }

        // Page Event
        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot.MouseMove += new MouseEventHandler(zoomArea_MouseMove);
            zoomScale.Maximum = 10;
            zoomScale.Minimum = 1;
            zoomScale.ValueChanged += new RoutedPropertyChangedEventHandler<double>(zoomScale_ValueChanged);
            zoomIn.Click += new RoutedEventHandler(zoomIn_Click);
            zoomOut.Click += new RoutedEventHandler(zoomOut_Click);
            zoomReset.Click += new RoutedEventHandler(zoomReset_Click);
        }

        // Mouse Event
        private void OnMouseWheel(object sender, HtmlEventArgs args)
        {
            if (ZoomEnabled)
            {
                double mouseDelta = 0;
                ScriptObject e = args.EventObject;

                // Mozilla and Safari
                if (e.GetProperty("detail") != null)
                {
                    mouseDelta = ((double)e.GetProperty("detail"));
                }
                // IE and Opera
                else if (e.GetProperty("wheelDelta") != null)
                    mouseDelta = ((double)e.GetProperty("wheelDelta"));

                mouseDelta = Math.Sign(mouseDelta);
                // Action zoom effect
                if (mouseDelta != 0 && ScaleFactor.ScaleX >= 1 && ScaleFactor.ScaleX <= 10)
                {
                    zoomArea.RenderTransformOrigin = new Point(pt.X / zoomArea.Width, pt.Y / zoomArea.Height);
                    ScaleFactor.ScaleX += (mouseDelta / 5) * ScaleFactor.ScaleX;
                    ScaleFactor.ScaleY += (mouseDelta / 5) * ScaleFactor.ScaleY;
                    zoomScale.Value = ScaleFactor.ScaleX;
                }
                if (ScaleFactor.ScaleX < 1)
                {
                    ScaleFactor.ScaleX = 1;
                    ScaleFactor.ScaleY = 1;
                }
                if (ScaleFactor.ScaleX > 10)
                {
                    ScaleFactor.ScaleX = 10;
                    ScaleFactor.ScaleY = 10;
                }
            }
        }
        #endregion

        #region Zoom Area Canvas Events
        void zoomArea_MouseMove(object sender, MouseEventArgs e)
        {
             pt = e.GetPosition(zoomArea);
        }

        void zoomReset_Click(object sender, RoutedEventArgs e)
        {
                ScaleFactor.ScaleX = 1;
                ScaleFactor.ScaleY = 1;
                zoomScale.Value = ScaleFactor.ScaleX;
                DragFactor.X = 0;
                DragFactor.Y = 0;
        }

        void zoomOut_Click(object sender, RoutedEventArgs e)
        {
                ScaleFactor.ScaleX -= 0.5;
                ScaleFactor.ScaleY -= 0.5;
                if (ScaleFactor.ScaleX < 1)
                {
                    ScaleFactor.ScaleX = 1;
                    ScaleFactor.ScaleY = 1;
                }
                zoomScale.Value = ScaleFactor.ScaleX;
        }

        void zoomIn_Click(object sender, RoutedEventArgs e)
        {
                ScaleFactor.ScaleX += 0.5;
                ScaleFactor.ScaleY += 0.5;
                if (ScaleFactor.ScaleX > 10)
                {
                    ScaleFactor.ScaleX = 10;
                    ScaleFactor.ScaleY = 10;
                }
                zoomScale.Value = ScaleFactor.ScaleX;
        }

        void zoomScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
                ScaleFactor.ScaleX = zoomScale.Value;
                ScaleFactor.ScaleY = zoomScale.Value;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Iran Province Path Blinker.
        /// </summary>
        /// <param name="city">Name of province for blink.</param>
        /// <returns>For detected province true, otherwise false.</returns>
        public bool Blinker(String city)
        {
            if ((city.Contains("كرمان") && city.Contains("شاه")) 
                | city.Contains("کرمانشاه") 
                | (city.Contains("كرمان") && city.Contains("شاه")) 
                | city.Contains("كرمانشاه") 
                | city.Contains("شاه") | city.Contains("شاه"))
            {
                this.BlinkKermanshah.Begin();
                this.BlinkKermanshah.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Kermanshah"))
                    this.provinceState.Add("Kermanshah", true);
                return true;
            }
            else if (city.Contains("کردستان") | city.Contains("كردستان"))
            {
                this.BlinkKordestan.Begin();
                this.BlinkKordestan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Kordestan"))
                    this.provinceState.Add("Kordestan", true);
                return true;
            }
            else if (city.Contains("مازندران") | city.Contains("مازندران"))
            {
                this.BlinkMazandaran.Begin();
                this.BlinkMazandaran.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Mazandaran"))
                    this.provinceState.Add("Mazandaran", true);
                return true;
            }
            else if (city.Contains("قزوین") | city.Contains("قزوين"))
            {
                this.BlinkGhazvin.Begin();
                this.BlinkGhazvin.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Ghazvin"))
                    this.provinceState.Add("Ghazvin", true);
                return true;
            }
            else if (city.Contains("تهران") | city.Contains("تهران"))
            {
                this.BlinkTehran.Begin();
                this.BlinkTehran.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Tehran"))
                    this.provinceState.Add("Tehran", true);
                return true;
            }
            else if (city.Contains("گلستان") | city.Contains("كلستان"))
            {
                this.BlinkGolestan.Begin();
                this.BlinkGolestan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Golestan"))
                    this.provinceState.Add("Golestan", true);
                return true;
            }
            else if ((city.Contains("خراسان") && city.Contains("رضوی"))
                | (city.Contains("خراسان") && city.Contains("رضوي")))
            {
                this.BlinkKhorasanRazavi.Begin();
                this.BlinkKhorasanRazavi.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("KhorasanRazavi"))
                    this.provinceState.Add("KhorasanRazavi", true);
                return true;
            }
            else if ((city.Contains("خراسان") && city.Contains("جنوبی"))
                | (city.Contains("خراسان") && city.Contains("جنوبي")))
            {
                this.BlinkKhorasanJonoubi.Begin();
                this.BlinkKhorasanJonoubi.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("KhorasanJonoubi"))
                    this.provinceState.Add("KhorasanJonoubi", true);
                return true;
            }
            else if ((city.Contains("خراسان") && city.Contains("شمالی")) 
                | (city.Contains("خراسان") && city.Contains("شمالي")))
            {
                this.BlinkKhorasanShomali.Begin();
                this.BlinkKhorasanShomali.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("KhorasanShomali"))
                    this.provinceState.Add("KhorasanShomali", true);
                return true;
            }
            else if (city.Contains("سمنان") | city.Contains("سمنان"))
            {
                this.BlinkSemnan.Begin();
                this.BlinkSemnan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Semnan"))
                    this.provinceState.Add("Semnan", true);
                return true;
            }
            else if (city.Contains("یزد") | city.Contains("يزد"))
            {
                this.BlinkYazd.Begin();
                this.BlinkYazd.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Yazd"))
                    this.provinceState.Add("Yazd", true);
                return true;
            }
            else if (city.Contains("کرمان") | city.Contains("كرمان"))
            {
                this.BlinkKerman.Begin();
                this.BlinkKerman.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Kerman"))
                    this.provinceState.Add("Kerman", true);
                return true;
            }
            else if (city.Contains("هرمزگان") | city.Contains("هرمزكان"))
            {
                this.BlinkHormozgan.Begin();
                this.BlinkHormozgan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Hormozgan"))
                    this.provinceState.Add("Hormozgan", true);
                return true;
            }
            else if ((city.Contains("سیستان") && city.Contains("و") && city.Contains("بلوچستان"))
                | (city.Contains("سيستان") && city.Contains("و") && city.Contains("بلوجستان")))
            {
                this.BlinkSystan.Begin();
                this.BlinkSystan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Systan"))
                    this.provinceState.Add("Systan", true);
                return true;
            }
            else if ((city.Contains("آذربایجان") && city.Contains("غربی"))
                | (city.Contains("اذربايجان") && city.Contains("غربي")))
            {
                this.BlinkAzarGharbi.Begin();
                this.BlinkAzarGharbi.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("AzarGharbi"))
                    this.provinceState.Add("AzarGharbi", true);
                return true;
            }
            else if ((city.Contains("آذربایجان") && city.Contains("شرقی"))
                | (city.Contains("اذربايجان") && city.Contains("شرقي")))
            {
                this.BlinkAzarSharghi.Begin();
                this.BlinkAzarSharghi.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("AzarSharghi"))
                    this.provinceState.Add("AzarSharghi", true);
                return true;
            }
            else if (city.Contains("لرستان") | city.Contains("لرستان"))
            {
                this.BlinkLorestan.Begin();
                this.BlinkLorestan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Lorestan"))
                    this.provinceState.Add("Lorestan", true);
                return true;
            }
            else if (city.Contains("بوشهر") | city.Contains("بوشهر"))
            {
                this.BlinkBoushehr.Begin();
                this.BlinkBoushehr.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Boushehr"))
                    this.provinceState.Add("Boushehr", true);
                return true;
            }
            else if (city.Contains("اصفهان") | city.Contains("اصفهان"))
            {
                this.BlinkEsfahan.Begin();
                this.BlinkEsfahan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Esfahan"))
                    this.provinceState.Add("Esfahan", true);
                return true;
            }
            else if (city.Contains("فارس") | city.Contains("فارس"))
            {
                this.BlinkFars.Begin();
                this.BlinkFars.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Fars"))
                    this.provinceState.Add("Fars", true);
                return true;
            }
            else if ((city.Contains("کهگیلویه") && city.Contains("و") && city.Contains("بویراحمد")) 
                | city.Contains("كهكيلويه") && city.Contains("و") && city.Contains("بويراحمد"))
            {
                this.BlinkBoyer.Begin();
                this.BlinkBoyer.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Boyer"))
                    this.provinceState.Add("Boyer", true);
                return true;
            }
            else if ((city.Contains("چهارمحال") && city.Contains("و") && city.Contains("بختیاری")) 
                | (city.Contains("جهارمحال") && city.Contains("و") && city.Contains("بختياري")))
            {
                this.BlinkBakhtyari.Begin();
                this.BlinkBakhtyari.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Bakhtyari"))
                    this.provinceState.Add("Bakhtyari", true);
                return true;
            }
            else if (city.Contains("خوزستان") | city.Contains("خوزستان"))
            {
                this.BlinkKhouzestan.Begin();
                this.BlinkKhouzestan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Khouzestan"))
                    this.provinceState.Add("Khouzestan", true);
                return true;
            }
            else if (city.Contains("قم") | city.Contains("قم"))
            {
                this.BlinkGhom.Begin();
                this.BlinkGhom.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Ghom"))
                    this.provinceState.Add("Ghom", true);
                return true;
            }
            else if (city.Contains("مرکزی") | city.Contains("مركزي"))
            {
                this.BlinkMarkazi.Begin();
                this.BlinkMarkazi.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Markazi"))
                    this.provinceState.Add("Markazi", true);
                return true;
            }
            else if (city.Contains("همدان") | city.Contains("همدان"))
            {
                this.BlinkHamedan.Begin();
                this.BlinkHamedan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Hamedan"))
                    this.provinceState.Add("Hamedan", true);
                return true;
            }
            else if (city.Contains("زنجان") | city.Contains("زنجان"))
            {
                this.BlinkZanjan.Begin();
                this.BlinkZanjan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Zanjan"))
                    this.provinceState.Add("Zanjan", true);
                return true;
            }
            else if (city.Contains("گیلان") | city.Contains("كيلان"))
            {
                this.BlinkGilan.Begin();
                this.BlinkGilan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Gilan"))
                    this.provinceState.Add("Gilan", true);
                return true;
            }
            else if (city.Contains("اردبیل") | city.Contains("اردبيل"))
            {
                this.BlinkArdebil.Begin();
                this.BlinkArdebil.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Ardebil"))
                    this.provinceState.Add("Ardebil", true);
                return true;
            }
            else if (city.Contains("ایلام") | city.Contains("ايلام"))
            {
                this.BlinkIlam.Begin();
                this.BlinkIlam.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Ilam"))
                    this.provinceState.Add("Ilam", true);
                return true;
            }
            else if (city.Contains("ایران"))
            {
                //this.BlinkProvinceState.Begin();
                //this.BlinkProvinceState.Children[0].SpeedRatio = ProvinceSpeedRatio;
                //if (!provinceState.ContainsKey("Iran"))
                //    this.provinceState.Add("Iran", true);
                //return true;
            }
            else if (city.Contains("خلیج فارس") | (city.Contains("خليج") && city.Contains("فارس")) 
                | city.Contains("دریای عمان") | (city.Contains("عمان") && city.Contains("درياي")) 
                | (city.Contains("خليج") && city.Contains("فارس")) | city.Contains("پارس"))
            {
                this.BlinkPersianGulfAndUmmanSea.Begin();
                this.BlinkPersianGulfAndUmmanSea.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("PersianGulfAndUmmanSea"))
                    this.provinceState.Add("PersianGulfAndUmmanSea", true);
                return true;
            }
            else if ((city.Contains("خزر") && city.Contains("دریای")) | (city.Contains("خزر") && city.Contains("درياي")))
            {
                this.BlinkCaspian.Begin();
                this.BlinkCaspian.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("CaspianSea"))
                    this.provinceState.Add("CaspianSea", true);
                return true;
            }
            else if (city.Contains("البرز") | city.Contains("البرز"))
            {
                this.BlinkAlborz.Begin();
                this.BlinkAlborz.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!provinceState.ContainsKey("Alborz"))
                    this.provinceState.Add("Alborz", true);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Reset Blinker.
        /// </summary>
        public void BlinkerReset()
        {
            this.BlinkKermanshah.Stop();
            this.BlinkKordestan.Stop();
            this.BlinkMazandaran.Stop();
            this.BlinkGhazvin.Stop();
            this.BlinkTehran.Stop();
            this.BlinkGolestan.Stop();
            this.BlinkKhorasanRazavi.Stop();
            this.BlinkKhorasanJonoubi.Stop();
            this.BlinkKhorasanShomali.Stop();
            this.BlinkSemnan.Stop();
            this.BlinkYazd.Stop();
            this.BlinkKerman.Stop();
            this.BlinkHormozgan.Stop();
            this.BlinkSystan.Stop();
            this.BlinkAzarGharbi.Stop();
            this.BlinkAzarSharghi.Stop();
            this.BlinkLorestan.Stop();
            this.BlinkBoushehr.Stop();
            this.BlinkEsfahan.Stop();
            this.BlinkFars.Stop();
            this.BlinkBoyer.Stop();
            this.BlinkBakhtyari.Stop();
            this.BlinkKhouzestan.Stop();
            this.BlinkGhom.Stop();
            this.BlinkMarkazi.Stop();
            this.BlinkHamedan.Stop();
            this.BlinkZanjan.Stop();
            this.BlinkGilan.Stop();
            this.BlinkArdebil.Stop();
            this.BlinkIlam.Stop();
            this.BlinkPersianGulfAndUmmanSea.Stop();
            this.BlinkCaspian.Stop();
            this.BlinkAlborz.Stop();
        }

        /// <summary>
        /// Iran State Path Blinker.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public bool CityBlinker(String city)
        {
            if (city.Contains("بيرجند"))
            {
                this.Birjand.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkBirjand.Begin();
                this.CityBlinkBirjand.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Birjand"))
                    this.cityState.Add("Birjand", true);
                return true;
            }
            if (city.Contains("سربيشه"))
            {
                this.Sarbishe.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkSarbishe.Begin();
                this.CityBlinkSarbishe.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Sarbishe"))
                    this.cityState.Add("Sarbishe", true);
                return true;
            }
            if (city.Contains("فردوس"))
            {
                this.Ferdos.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkFerdos.Begin();
                this.CityBlinkFerdos.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Ferdos"))
                    this.cityState.Add("Ferdos", true);
                return true;
            }
            if (city.Contains("قاةنات") | city.Contains("قائنات"))
            {
                this.Ghaenat.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkGhaenat.Begin();
                this.CityBlinkGhaenat.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Ghaenat"))
                    this.cityState.Add("Ghaenat", true);
                return true;
            }
            if (city.Contains("نهبندان"))
            {
                this.Nahbandan.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkNahbandan.Begin();
                this.CityBlinkNahbandan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Nahbandan"))
                    this.cityState.Add("Nahbandan", true);
                return true;
            }
            if (city.Contains("درميان"))
            {
                this.Darmian.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkDarmian.Begin();
                this.CityBlinkDarmian.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Darmian"))
                    this.cityState.Add("Darmian", true);
                return true;
            }
            if (city.Contains("سرايان"))
            {
                this.Sarayan.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkSarayan.Begin();
                this.CityBlinkSarayan.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Sarayan"))
                    this.cityState.Add("Sarayan", true);
                return true;
            }
            if (city.Contains("بشرويه"))
            {
                this.Boshrooye.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkBoshrooye.Begin();
                this.CityBlinkBoshrooye.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Boshrooye"))
                    this.cityState.Add("Boshrooye", true);
                return true;
            }
            if (city.Contains("ؤيركوه") | city.Contains("زیرکوه"))
            {
                this.Zirkouh.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkZirkouh.Begin();
                this.CityBlinkZirkouh.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Zirkouh"))
                    this.cityState.Add("Zirkouh", true);
                return true;
            }
            if (city.Contains("خوسف"))
            {
                this.Khousef.Visibility = System.Windows.Visibility.Visible;
                this.CityBlinkKhousef.Begin();
                this.CityBlinkKhousef.Children[0].SpeedRatio = ProvinceSpeedRatio;
                if (!cityState.ContainsKey("Khousef"))
                    this.cityState.Add("Khousef", true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Change Map Size.
        /// </summary>
        /// <param name="Size">Size Must Between 1 to 10.</param>
        public void SizeChanger(double Size)
        {
            if (Size >= 1 && Size <= 10)
            {
                this.zoomArea.Width = this.zoomArea.Width * Size;
                this.zoomArea.Height = this.zoomArea.Height * Size;
            }
            else
            {
                MessageBox.Show("اندازه نقشه میتواند مقادیر بین یک تا ده باشد." + "\n" + "Size Must Between 1 to 10.");
            }
        }
        #endregion
    }
}
