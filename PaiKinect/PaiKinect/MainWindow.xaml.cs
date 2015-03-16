using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;

namespace PaiKinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        public static int DEFAULTTHICKNESS = 10;
        public KinectSensor _Kinect;
        private WriteableBitmap _ColorImageBitmap;
        private Int32Rect _ColorImageBitmapRect;
        private int _ColorImageStride;
        private Skeleton[] FrameSkeletons;

        private bool notDrawing;
        private bool isClickable;

        private PaintHandler paintBrush;                   // I AM NOT IN CONTROL OF ALL THE CODEE MUHAHAHA

        

        public Point currentPoint;

        public Point? cursorPosition = null;
        public Point? _pastCursorPosition = null;
        //private Point _pastCursorPosition = new Point(300,300);
        private SolidColorBrush _previousFill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        public SolidColorBrush brushColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        //InteractionStream interactionStream;

        List<System.Windows.Controls.Button> buttons;
        static System.Windows.Controls.Button selected;

      

        float handX;
        float handY;
        #endregion
       
        #region KinectHandling
        
        /*
         * This constructor resets the hand position, and makes sure that the buttons click methods are set in place.
         * 
         */
        public MainWindow()
        {
            InitializeComponent();
            InitializeButtons();
            Generics.ResetHandPosition(kinectButton);
            kinectButton.Click += new RoutedEventHandler(kinectButton_Click);
            //kinectButton2.Click += new RoutedEventHandler(kinectButton_Click);
            this.Loaded += Main_Loaded;
            KinectRegion.AddHandPointerMoveHandler(this, OnHandPointerMove);
            paintBrush = new PaintHandler(cursorPosition, _previousFill, DEFAULTTHICKNESS);
            notDrawing = false;
            isClickable = true;

        }
                                      

        private void OnHandPointerMove(object sender, HandPointerEventArgs e)
        {
            currentPoint = e.HandPointer.GetPosition(myCanvas1);
        }

        //Make the buttons that will appear in the application
        private void InitializeButtons()
        {
            buttons = new List<System.Windows.Controls.Button> { QUIT, ERASER, RED, ORANGE, BLUE, GREEN, YELLOW, BLACK, PURPLE, CLEAR, 
                                                                SMALLLINESIZE, MEDIUMLINESIZE, LARGELINESIZE};
        }
        //Make sure that the Kinect is in a nice generic state so nothing can go wrong
        private void UnregisterEvents()
        {
            KinectSensor.KinectSensors.StatusChanged -= KinectSensors_StatusChanged;
            this.Kinect.SkeletonFrameReady -= Kinect_SkeletonFrameReady;
            this.Kinect.ColorFrameReady -= Kinect_ColorFrameReady;

        }
        //When the main is loaded, get the kinect sensor
        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            DiscoverKinectSensor();
        }

        //Gets the connected Kinect sensor
        private void DiscoverKinectSensor()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;

            this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }

        //When the Kinect is disconnected, raise an error
        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (this.Kinect == null)
                    {
                        this.Kinect = e.Sensor;
                    }
                    break;
                case KinectStatus.Disconnected:
                    if (this.Kinect == e.Sensor)
                    {
                        this.Kinect = null;
                        this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (this.Kinect == null)
                        {
                            System.Windows.MessageBox.Show("Sensor Disconnected. Please reconnect to continue.");
                        }
                    }
                    break;
            }
        }
        //The kinect sensor
        public KinectSensor Kinect
        {
            get { return this._Kinect; }
            set
            {
                if (this._Kinect != value)
                {
                    if (this._Kinect != null)
                    {
                        this._Kinect = null;
                    }
                    if (value != null && value.Status == KinectStatus.Connected)
                    {
                        this._Kinect = value;
                        InitializeKinectSensor(this._Kinect);
                    }
                }
            }
        }
        //Initialize the sensor
        private void InitializeKinectSensor(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                ColorImageStream colorStream = kinectSensor.ColorStream;
                colorStream.Enable();
                this._ColorImageBitmap = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight,
                    96, 96, PixelFormats.Bgr32, null);
                this._ColorImageBitmapRect = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
                this._ColorImageStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
                

                kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
                {
                    Correction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f,
                    Smoothing = 0.5f
                });

                kinectSensor.SkeletonFrameReady += Kinect_SkeletonFrameReady;
                kinectSensor.ColorFrameReady += Kinect_ColorFrameReady;

                if (!kinectSensor.IsRunning)
                {
                    kinectSensor.Start();
                }

                this.FrameSkeletons = new Skeleton[this.Kinect.SkeletonStream.FrameSkeletonArrayLength];

            }
        }
        //Get the color frame ready (not currently used I don't think)
        private void Kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);
                    this._ColorImageBitmap.WritePixels(this._ColorImageBitmapRect, pixelData,
                        this._ColorImageStride, 0);
                }
            }
        }
        //Get the skeleton frame ready (very important)
        private void Kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this.FrameSkeletons);
                    Skeleton skeleton = GetPrimarySkeleton(this.FrameSkeletons);

                    if (skeleton == null)
                    {
                        kinectButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Joint primaryHand = GetPrimaryHand(skeleton);
                        Joint primaryElbow = GetPrimaryElbow(skeleton);
                        Joint leftShoulder = GetLeftShoulder(skeleton);
                        Joint leftHand = GetLeftHand(skeleton);
                        
                        //TrackHand(primaryHand, primaryElbow);
                        TrackHand(primaryHand, leftHand, leftShoulder);
                    }
                }
            }
        }

        //track and display hand
        private void TrackHand(Joint hand, /*Joint elbow*/ Joint leftHand, Joint leftShoulder)
        {



            if (hand.TrackingState == JointTrackingState.NotTracked)
            {
                kinectButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                kinectButton.Visibility = System.Windows.Visibility.Visible;

                DepthImagePoint point = this.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(hand.Position, DepthImageFormat.Resolution640x480Fps30);
                handX = (int)((point.X * LayoutRoot.ActualWidth / this.Kinect.DepthStream.FrameWidth) -
                    (kinectButton.ActualWidth / 2.0));
                handY = (int)((point.Y * LayoutRoot.ActualHeight / this.Kinect.DepthStream.FrameHeight) -
                    (kinectButton.ActualHeight / 2.0));
                Canvas.SetLeft(kinectButton, handX);
                Canvas.SetTop(kinectButton, handY);

                if (isHandOver(kinectButton, buttons)) kinectButton.Hovering();
                else kinectButton.Release();
                if (hand.JointType == JointType.HandRight)
                {
                    kinectButton.ImageSource = "/PaiKinect;component/Images/right.png";
                    kinectButton.ActiveImageSource = "/PaiKinect;component/Images/right.png";
                }
                else
                {
                    kinectButton.ImageSource = "/PaiKinect;component/Images/right.png";
                    kinectButton.ActiveImageSource = "/PaiKinect;component/Images/right.png";
                }

                cursorPosition = new Point(handX, handY);




                //bool _handLifted = false;
                /*_HandPositionZStyle = hand.Position.Z;
                if(hand.Position.Z > _HandPositionZStyle)
                {
                    _handLifted = true;
                }
                else
                {
                    _handLifted = false;
                }*/
                /*double _elbowToHandValue = elbow.Position.Y + hand.Position.Y;
                if((elbow.Position.Y + hand.Position.Y) > _elbowToHandValue)
                {
                    _handLifted = true;
                }
                else
                {
                    _handLifted = false;
                }*/

                Joint lh = leftHand;
                Joint ls = leftShoulder;

                bool isup = lh.Position.Y > ls.Position.Y; //This value is 'true' if the user's left hand is above their left shoulder, and false if it's not.
                
                //If the hand has been lifted, reset the past cursor position so that it doesnt pick up where you left off.
                if(isup == true)
                {
                    _pastCursorPosition = new Point(handX, handY);
                    kinectButton.Visibility = System.Windows.Visibility.Visible;
                    showButtons();
                    
                    
                    //notDrawing = true;
                }
                else
                {
                    //notDrawing = false;
                    hideButtons();
                    if (!isHandOver(kinectButton, buttons))
                    {
                        if (_pastCursorPosition == null)
                        {

                            kinectButton.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            //Random r = new Random();
                            //byte _r1 = (byte)r.Next(0, 255);
                            //byte _r2 = (byte)r.Next(0, 255);
                            //byte _r3 = (byte)r.Next(0, 255);
                            //_previousFill = new SolidColorBrush(Color.FromRgb(_r1,_r2,_r3));
                            
                            

                            if (_pastCursorPosition.Value.X > handX)
                            {
                                if ((handX - _pastCursorPosition.Value.X) < -50)
                                {
                                    //DrawLine(handX, handY);
                                    paintBrush.setPoint(_pastCursorPosition);
                                    paintBrush.setBrush(_previousFill);
                                    Line l = paintBrush.DrawLine(handX, handY);
                                    myCanvas1.Children.Add(l);


                                }
                                else
                                {
                                    //DrawLine(handX, handY);
                                    paintBrush.setPoint(_pastCursorPosition);
                                    paintBrush.setBrush(_previousFill);
                                    Line l = paintBrush.DrawLine(handX, handY);
                                    myCanvas1.Children.Add(l);
                                }
                            }
                            else
                            {
                                if ((_pastCursorPosition.Value.X - handX) < -50)
                                {
                                    //DrawLine(handX, handY);
                                    paintBrush.setPoint(_pastCursorPosition);
                                    paintBrush.setBrush(_previousFill);
                                    Line l = paintBrush.DrawLine(handX, handY);
                                    myCanvas1.Children.Add(l);
                                }
                                else
                                {
                                    //DrawLine(handX, handY);
                                    paintBrush.setPoint(_pastCursorPosition);
                                    paintBrush.setBrush(_previousFill);
                                    Line l = paintBrush.DrawLine(handX, handY);
                                    myCanvas1.Children.Add(l);
                                }
                            }

                        }

                        _pastCursorPosition = cursorPosition;
                        

                    }
                }
            }
        }

        private void hideButtons()
        {
            isClickable = false;
            RED.Visibility = System.Windows.Visibility.Hidden;
            QUIT.Visibility = System.Windows.Visibility.Hidden;
            ERASER.Visibility = System.Windows.Visibility.Hidden;
            GREEN.Visibility = System.Windows.Visibility.Hidden;
            BLUE.Visibility = System.Windows.Visibility.Hidden;
            PURPLE.Visibility = System.Windows.Visibility.Hidden;
            YELLOW.Visibility = System.Windows.Visibility.Hidden;
            ORANGE.Visibility = System.Windows.Visibility.Hidden;
            BLACK.Visibility = System.Windows.Visibility.Hidden;
            DummyCanvas.Visibility = System.Windows.Visibility.Hidden;
            myCanvas1.Visibility = System.Windows.Visibility.Visible;
        }

        private void showButtons()
        {
            isClickable = true;
            RED.Visibility = System.Windows.Visibility.Visible;
            QUIT.Visibility = System.Windows.Visibility.Visible;
            ERASER.Visibility = System.Windows.Visibility.Visible;
            GREEN.Visibility = System.Windows.Visibility.Visible;
            BLUE.Visibility = System.Windows.Visibility.Visible;
            PURPLE.Visibility = System.Windows.Visibility.Visible;
            YELLOW.Visibility = System.Windows.Visibility.Visible;
            ORANGE.Visibility = System.Windows.Visibility.Visible;
            BLACK.Visibility = System.Windows.Visibility.Visible;
            DummyCanvas.Visibility = System.Windows.Visibility.Visible;
            myCanvas1.Visibility = System.Windows.Visibility.Visible;
        }

        //detect if hand is overlapping over any button
        private bool isHandOver(FrameworkElement hand, List<System.Windows.Controls.Button> buttonslist)
        {
            bool value = false;
            if (isClickable)
            {
                var handTopLeft = new Point(Canvas.GetLeft(hand), Canvas.GetTop(hand));
                var handX = handTopLeft.X + hand.ActualWidth / 2;
                var handY = handTopLeft.Y + hand.ActualHeight / 2;

                foreach (System.Windows.Controls.Button target in buttonslist)
                {

                    if (target != null)
                    {
                        Point targetTopLeft = new Point(Canvas.GetLeft(target), Canvas.GetTop(target));
                        if (handX > targetTopLeft.X &&
                            handX < targetTopLeft.X + target.Width &&
                            handY > targetTopLeft.Y &&
                            handY < targetTopLeft.Y + target.Height)
                        {
                            selected = target;
                            value = true;
                            return value;
                        }
                    }
                }
            }
            return value;
            
        }

        //get the hand closest to the Kinect sensor
        private static Joint GetPrimaryHand(Skeleton skeleton)
        {
            Joint primaryHand = new Joint();
            if (skeleton != null)
            {
                primaryHand = skeleton.Joints[JointType.HandLeft];
                Joint rightHand = skeleton.Joints[JointType.HandRight];
                if (rightHand.TrackingState != JointTrackingState.NotTracked)
                {
                    if (primaryHand.TrackingState == JointTrackingState.NotTracked)
                    {
                        primaryHand = rightHand;
                    }
                    else
                    {
                        if (primaryHand.Position.Z > rightHand.Position.Z)
                        {
                            primaryHand = rightHand;
                        }
                    }
                }
            }
            return primaryHand;
        }

        //get the hand closest to the Kinect sensor
        private static Joint GetPrimaryElbow(Skeleton skeleton)
        {
            Joint primaryElbow = new Joint();
            if (skeleton != null)
            {
                primaryElbow = skeleton.Joints[JointType.ElbowLeft];
                Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
                if (rightElbow.TrackingState != JointTrackingState.NotTracked)
                {
                    if (primaryElbow.TrackingState == JointTrackingState.NotTracked)
                    {
                        primaryElbow = rightElbow;
                    }
                    else
                    {
                        if (primaryElbow.Position.Z > rightElbow.Position.Z)
                        {
                            primaryElbow = rightElbow;
                        }
                    }
                }
            }
            return primaryElbow;
        }

        //get the hand closest to the Kinect sensor
        private static Joint GetLeftShoulder(Skeleton skeleton)
        {
            Joint leftShoulder = new Joint();
            if (skeleton != null)
            {
                leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
  
            }
            return leftShoulder;
        }

        //get the hand closest to the Kinect sensor
        private static Joint GetLeftHand(Skeleton skeleton)
        {
            Joint leftHand = new Joint();
            if (skeleton != null)
            {
                leftHand = skeleton.Joints[JointType.HandLeft];
                
            }
            return leftHand;
        }

        //get the skeleton closest to the Kinect sensor
        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }
            return skeleton;
        }
        //placeholder, ignore
        void kinectButton_Click(object sender, RoutedEventArgs e)
        {
            selected.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, selected));
        }
        //when the quit button is clicked, exit the application

        private void ERASER_Click(object sender, RoutedEventArgs e)
        {
         

            
            if (ERASER.Background == Brushes.DeepSkyBlue)
            {
                ERASER.Background = Brushes.DimGray;
                brushColor = _previousFill;
                _previousFill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                
            }
            else
            {
                ERASER.Background = Brushes.DeepSkyBlue;
                _previousFill = brushColor;

            }
        }

        private void RED_Click(object sender, RoutedEventArgs e)
        {


           _previousFill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushColor = _previousFill;
            
        }
        private void YELLOW_Click(object sender, RoutedEventArgs e)
        {

            _previousFill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            brushColor = _previousFill;
           
        }
        private void GREEN_Click(object sender, RoutedEventArgs e)
        {

            _previousFill = new SolidColorBrush(Color.FromRgb(0, 128, 0));
            brushColor = _previousFill;
            
        }
        private void BLUE_Click(object sender, RoutedEventArgs e)
        {


            _previousFill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            brushColor = _previousFill;
        }
        private void PURPLE_Click(object sender, RoutedEventArgs e)
        {


            _previousFill = new SolidColorBrush(Color.FromRgb(128, 0, 128));
            brushColor = _previousFill;

            
        }
        private void ORANGE_Click(object sender, RoutedEventArgs e)
        {


            _previousFill = new SolidColorBrush(Color.FromRgb(255, 102, 0));
            brushColor = _previousFill;
        }
        
        private void BLACK_Click(object sender, RoutedEventArgs e)
        {


            _previousFill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            brushColor = _previousFill;
        }


        
        private void QUIT_Click(object sender, RoutedEventArgs e)
        {
            UnregisterEvents();
            System.Windows.Application.Current.Shutdown();
        }

        private void CLEAR_Click(object sender, RoutedEventArgs e)
        {
            myCanvas1.Children.Clear();
        }

        private void SML_Click(object sender, RoutedEventArgs e)
        {
            paintBrush.setStrokeThickness(5);
        }

        private void MED_Click(object sender, RoutedEventArgs e)
        {
            paintBrush.setStrokeThickness(10);
        }
        
        private void LRG_Click(object sender, RoutedEventArgs e)
        {
            paintBrush.setStrokeThickness(20);
        }

        #endregion

        #region DrawingCode
        /*
         * This reigon of code is obsolete. It is being kept just in case.
         *
         */
        
        //draws a line if it senses a change in the cursor position.
        /*
        private void DrawLine(float x, float y)
        {

            Line ln = new Line
            {
                X1 = _pastCursorPosition.Value.X,
                Y1 = _pastCursorPosition.Value.Y,
                X2 = x,
                Y2 = y,
                StrokeThickness = 10,
                StrokeLineJoin = PenLineJoin.Round
            };

            ln.Stroke = new SolidColorBrush(_previousFill.Color);
            ln.StrokeDashCap = PenLineCap.Round;
            ln.StrokeStartLineCap = PenLineCap.Round;
            ln.StrokeEndLineCap = PenLineCap.Round;
            myCanvas1.Children.Add(ln);
            
            
        }
        */

        #endregion


        
    }
}
