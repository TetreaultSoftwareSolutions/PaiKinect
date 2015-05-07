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
        /*The default thickness of the line*/
        public static int DEFAULTTHICKNESS = 10;
        /*The kinect sensor that is used*/
        public KinectSensor _Kinect;
        /*The ColorImageBitmap used by the Kinect Sensor*/
        private WriteableBitmap _ColorImageBitmap;
        /*The Rect used by the ColorImageBitmap*/
        private Int32Rect _ColorImageBitmapRect;
        /*The stride size*/
        private int _ColorImageStride;
        /*The array that holds all the skeletons the Kinect is picking up*/
        private Skeleton[] FrameSkeletons;
        /*If a button is clickable or not*/
        public bool isClickable;
        /*The paintHandler used to draw lines on the canvas*/
        private PaintHandler paintBrush; 
        /*The current point of the cursor*/
        public Point currentPoint;
        /*The current position of the cursor*/
        public Point? cursorPosition = null;
        /*The past position of the cursor*/
        public Point? _pastCursorPosition = null;
        /*The brush used to draw with*/
        private SolidColorBrush _previousFill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        /*The color of the brush used to draw with (used to change the color of the line)*/
        public SolidColorBrush brushColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        /*The list of buttons in the xaml*/
        public List<System.Windows.Controls.Button> buttons;
        /*the button that is currently being selected*/
        static System.Windows.Controls.Button selected;
        /*The drawing handler*/
        DrawingHandler draw;
        #endregion
       
        #region KinectHandling
        
        /**
         * This constructor resets the hand position, and makes sure that the buttons click methods are set in place.
         * 
         */
        public MainWindow()
        {
            InitializeComponent();
            InitializeButtons();
            Generics.ResetHandPosition(kinectButton);
            kinectButton.Click += new RoutedEventHandler(kinectButton_Click);
            
            this.Loaded += Main_Loaded;
            KinectRegion.AddHandPointerMoveHandler(this, OnHandPointerMove);
            paintBrush = new PaintHandler(cursorPosition, _previousFill, DEFAULTTHICKNESS);
            draw = new DrawingHandler(_previousFill, brushColor, paintBrush, this);
           
            isClickable = true;

            setButtonActionListeners();

 

        }
        
        /**
         * sets the buttons' click methods
         */
        private void setButtonActionListeners()
        {
            BLACK.Click += draw.BLACK_Click;
            QUIT.Click += draw.QUIT_Click; 
            ERASER.Click += draw.ERASER_Click; 
            RED.Click += draw.RED_Click; 
            ORANGE.Click += draw.ORANGE_Click; 
            BLUE.Click += draw.BLUE_Click;
            GREEN.Click += draw.GREEN_Click; 
            YELLOW.Click += draw.YELLOW_Click; 
            PURPLE.Click += draw.PURPLE_Click; 
            CLEAR.Click += draw.CLEAR_Click; 
            SMALLLINESIZE.Click += draw.SML_Click;
            MEDIUMLINESIZE.Click += draw.MED_Click; 
            LARGELINESIZE.Click += draw.LRG_Click; 
            SQUARE.Click += draw.SQUARE_Click;
            CIRCLE.Click += draw.CIRCLE_Click;
            RECTANGLE.Click += draw.RECTANGLE_Click;
            SHAPEINCREASE.Click += draw.SHAPEINCREASE_Click; 
            SHAPEDECREASE.Click += draw.SHAPEDECREASE_Click;
        }      
        /**
         * Gets the current hand position
         * @param sender the cursor that the method is tracking
         * @param e The arguments associated with that cursor
         */
        private void OnHandPointerMove(object sender, HandPointerEventArgs e)
        {
            currentPoint = e.HandPointer.GetPosition(myCanvas1);
        }

        /**
         * Make the buttons that will appear in the application
         */
        private void InitializeButtons()
        {
            buttons = new List<System.Windows.Controls.Button> { QUIT, ERASER, RED, ORANGE, BLUE, GREEN, YELLOW, BLACK, PURPLE, CLEAR, 
                                                                SMALLLINESIZE, MEDIUMLINESIZE, LARGELINESIZE, SQUARE, CIRCLE, RECTANGLE,
                                                                SHAPEINCREASE, SHAPEDECREASE};
        }
        /**
         * Make sure that the Kinect is in a nice generic state so nothing can go wrong
         */
        public void UnregisterEvents()
        {
            KinectSensor.KinectSensors.StatusChanged -= KinectSensors_StatusChanged;
            this.Kinect.SkeletonFrameReady -= Kinect_SkeletonFrameReady;
            this.Kinect.ColorFrameReady -= Kinect_ColorFrameReady;

        }
        /**
         * When the main is loaded, get the kinect sensor
         */
        void Main_Loaded(object sender, RoutedEventArgs e)
        {
            DiscoverKinectSensor();
        }

        /**
         * Gets the connected Kinect sensor
         */
        private void DiscoverKinectSensor()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;

            this.Kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }

        /**
         * When the Kinect is disconnected, raise an error
         * @param sender The kinect that is being tracked
         * @param e The arguments that says if the kinect has been unplugged or not
         */
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

        /**
         *Instantiates the Kinect Sensor
         */
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
        
        /**
         * Initialize the sensor
         * @param kinectSensor the sensor that is initialized
         */
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
        /**
         * Get the color frame ready 
         * @param sender The fram that is getting ready
         * @param e The event listener that states if it's ready or not
         */
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
        /**
         * Get the skeleton frame ready
         * @param sender The skeleton frame getting ready
         * @param e The event listener that states if it's ready or not
         */
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
                        
                        
                        draw.TrackHand(primaryHand, leftHand, leftShoulder);
                    }
                }
            }
        }

        /**
         * detect if hand is overlapping over any button
         * @param hand The hand that it's tracking
         * @param the buttons list that holds the buttons
         * @return If the hand is hovering over the button or not
         */
        public bool isHandOver(FrameworkElement hand, List<System.Windows.Controls.Button> buttonslist)
        {
            bool value = false;
            if (draw.isClickable)
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

       /**
         * get the hand closest to the Kinect sensor
         * @param skeleton The skeleton that holds the hand
         * @return the right hand
         */
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

        /**
         * get the elbow closest to the Kinect sensor
         * @param skeleton The skeleton that holds the elbow
         * @return the elbow
         */
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

        /**
          * get the left shoulder closest to the Kinect sensor
          * @param skeleton The skeleton that holds the shoulder
          * @return the left shoulder
          */
        private static Joint GetLeftShoulder(Skeleton skeleton)
        {
            Joint leftShoulder = new Joint();
            if (skeleton != null)
            {
                leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
  
            }
            return leftShoulder;
        }

        /**
          * get the left hand closest to the Kinect sensor
          * @param skeleton The skeleton that holds the left hand
          * @return the left hand
          */
        private static Joint GetLeftHand(Skeleton skeleton)
        {
            Joint leftHand = new Joint();
            if (skeleton != null)
            {
                leftHand = skeleton.Joints[JointType.HandLeft];
                
            }
            return leftHand;
        }

        /**
          * get the skeleton closest to the Kinect sensor
          * @param skeletons the list of skeletons that the kinect is picking up
          * @return the closest skeleton
          */
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
        /**
          * The click method for the generic button click
          * @param sender the button that is being clicked
          * @param e The arguments that the button has
          */
        void kinectButton_Click(object sender, RoutedEventArgs e)
        {
            selected.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, selected));
        }

         #endregion

       
        
    }
}
