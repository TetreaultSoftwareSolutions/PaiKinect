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
    class DrawingHandler
    {
        #region variables
        /*The mainWindow instance that will be accessed for helper methods*/
        MainWindow main;
        /*if a button is clickable*/
        public bool isClickable;
        /*if the user picked the square shape option*/
        bool drawingSquare;
        /*if the user picked the circle shape option*/
        bool drawingCircle;
        /*if the user picked the rectangle shape option*/
        bool drawingRectangle;
        /*if a shape is being increased*/
        bool shapeIncreased;
        /*if a shape is being decreased*/
        bool shapeDecreased;
        /*the x coordinate of the hand being tracked*/
        float handX;
        /*the y coordinate of the hand being tracked*/
        float handY;
        /*the amount that the shape will change size*/
        int amtChanged;
        /*the current cursor position*/
        public Point? cursorPosition = null;
        /*the past cursor position*/
        public Point? _pastCursorPosition = null;
        /*the paintbrush used to add lines to the canvas*/
        SolidColorBrush _previousFill;
        /*The color of the brush (used to change the color of _previousFill)*/
        SolidColorBrush brushColor;
        /*The paintBrush used to draw on the canvas*/
        PaintHandler paintBrush;
        /*The increment that the shapes will change size with*/
        public static int SIZE_INCREMENT = 50;
        #endregion
        #region Methods

        /**
         * Instantiates the drawing object
         * @param brush the brush being used to create the lines
         * @param brushColor the color of the brush
         * @param the paintBrush used to create the lines
         * @param main the MainWindow class accessed for helper methods
         */
        public DrawingHandler(SolidColorBrush brush, SolidColorBrush brushColor, PaintHandler paintbrush, MainWindow main)
        {
            this.main = main;
            _previousFill = brush;
            this.brushColor = brushColor;
            this.paintBrush = paintbrush;
            drawingSquare = false; 
            drawingCircle = false;
            drawingRectangle = false;
            shapeIncreased = false;
            shapeDecreased = false;
            amtChanged = 0;
            isClickable = false;
        }
        /**
         * Hides the buttons from being viewed on the canvas
         */
        public void hideButtons()
        {
            isClickable = false;
            main.RED.Visibility = System.Windows.Visibility.Hidden;
            main.QUIT.Visibility = System.Windows.Visibility.Hidden;
            main.ERASER.Visibility = System.Windows.Visibility.Hidden;
            main.GREEN.Visibility = System.Windows.Visibility.Hidden;
            main.BLUE.Visibility = System.Windows.Visibility.Hidden;
            main.PURPLE.Visibility = System.Windows.Visibility.Hidden;
            main.YELLOW.Visibility = System.Windows.Visibility.Hidden;
            main.ORANGE.Visibility = System.Windows.Visibility.Hidden;
            main.BLACK.Visibility = System.Windows.Visibility.Hidden;
            main.DummyCanvas.Visibility = System.Windows.Visibility.Hidden;
            main.myCanvas1.Visibility = System.Windows.Visibility.Visible;
        }
        /**
         * Shows the buttons on the canvas
         */
        public void showButtons()
        {
            isClickable = true;
            main.RED.Visibility = System.Windows.Visibility.Visible;
            main.QUIT.Visibility = System.Windows.Visibility.Visible;
            main.ERASER.Visibility = System.Windows.Visibility.Visible;
            main.GREEN.Visibility = System.Windows.Visibility.Visible;
            main.BLUE.Visibility = System.Windows.Visibility.Visible;
            main.PURPLE.Visibility = System.Windows.Visibility.Visible;
            main.YELLOW.Visibility = System.Windows.Visibility.Visible;
            main.ORANGE.Visibility = System.Windows.Visibility.Visible;
            main.BLACK.Visibility = System.Windows.Visibility.Visible;
            main.DummyCanvas.Visibility = System.Windows.Visibility.Visible;
            main.myCanvas1.Visibility = System.Windows.Visibility.Visible;
        }
        /**
         * The click method for the Eraser
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void ERASER_Click(object sender, RoutedEventArgs e)
        {

            _previousFill = new SolidColorBrush(Colors.White);
            brushColor = _previousFill;
        }
        /**
         * The click method for the Red Color Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void RED_Click(object sender, RoutedEventArgs e)
        {


           _previousFill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            brushColor = _previousFill;
            
        }
        /**
         * The click method for the Yellow Color Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void YELLOW_Click(object sender, RoutedEventArgs e)
        {

            _previousFill = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            brushColor = _previousFill;
           
        }
        /**
         * The click method for the Green Color Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void GREEN_Click(object sender, RoutedEventArgs e)
        {

            _previousFill = new SolidColorBrush(Color.FromRgb(0, 128, 0));
            brushColor = _previousFill;
            
        }
        /**
         * The click method for the Blue Color Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void BLUE_Click(object sender, RoutedEventArgs e)
        {


            _previousFill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            brushColor = _previousFill;
        }
        /**
         * The click method for the purple Color Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void PURPLE_Click(object sender, RoutedEventArgs e)
        {


            _previousFill = new SolidColorBrush(Color.FromRgb(128, 0, 128));
            brushColor = _previousFill;

            
        }
        /**
         * The click method for the orange Color Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void ORANGE_Click(object sender, RoutedEventArgs e)
        {


            _previousFill = new SolidColorBrush(Color.FromRgb(255, 102, 0));
            brushColor = _previousFill;
        }
        /**
         * The click method for the black Color Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void BLACK_Click(object sender, RoutedEventArgs e)
        {


            _previousFill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            brushColor = _previousFill;
        }
        /**
          * The click method for the quit Option
          * @param sender the button that was clicked
          * @param e The args that says it was clicked
          */
        public void QUIT_Click(object sender, RoutedEventArgs e)
        {
            main.UnregisterEvents();
            System.Windows.Application.Current.Shutdown();
        }
        /**
         * The click method for the clear Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void CLEAR_Click(object sender, RoutedEventArgs e)
        {
            main.myCanvas1.Children.Clear();
        }
        /**
         * The click method for the small line size Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void SML_Click(object sender, RoutedEventArgs e)
        {
            paintBrush.setStrokeThickness(5);
        }
        /**
         * The click method for the medium line size Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void MED_Click(object sender, RoutedEventArgs e)
        {
            paintBrush.setStrokeThickness(10);
        }
        /**
         * The click method for the large line size Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void LRG_Click(object sender, RoutedEventArgs e)
        {
            paintBrush.setStrokeThickness(20);
        }
        /**
         * The click method for the square shape Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void SQUARE_Click(object sender, RoutedEventArgs e)
        {
            drawingSquare = true;
        }
        /**
         * The click method for the circle shape Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void CIRCLE_Click(object sender, RoutedEventArgs e)
        {
            drawingCircle = true;
        }
        /**
         * The click method for the rectangle shape Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void RECTANGLE_Click(object sender, RoutedEventArgs e)
        {
            drawingRectangle = true;
        }
        /**
         * The click method for the shape decrease Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void SHAPEDECREASE_Click(object sender, RoutedEventArgs e)
        {
            shapeDecreased = true;
            if (!(100 - amtChanged < SIZE_INCREMENT))
            {
                amtChanged = amtChanged + SIZE_INCREMENT;
            }
        }
        /**
         * The click method for the shape increase Option
         * @param sender the button that was clicked
         * @param e The args that says it was clicked
         */
        public void SHAPEINCREASE_Click(object sender, RoutedEventArgs e)
        {
            shapeIncreased = true;
            amtChanged = amtChanged + SIZE_INCREMENT;
         }
        /**
         * Track and display hand, and add the lines to the canvas
         * @param hand The hand being tracked
         * @param leftHand the left hand being tracked
         * @param leftShoulder the left shoulder being tracked
         */
        public void TrackHand(Joint hand, Joint leftHand, Joint leftShoulder)
        {
            if (hand.TrackingState == JointTrackingState.NotTracked)
            {
                main.kinectButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                main.kinectButton.Visibility = System.Windows.Visibility.Visible;
                DepthImagePoint point = main.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(hand.Position, DepthImageFormat.Resolution640x480Fps30);
                handX = (int)((point.X * main.LayoutRoot.ActualWidth / main.Kinect.DepthStream.FrameWidth) -
                    (main.kinectButton.ActualWidth / 2.0));
                handY = (int)((point.Y * main.LayoutRoot.ActualHeight / main.Kinect.DepthStream.FrameHeight) -
                    (main.kinectButton.ActualHeight / 2.0));
                Canvas.SetLeft(main.kinectButton, handX);
                Canvas.SetTop(main.kinectButton, handY);
                if (main.isHandOver(main.kinectButton, main.buttons)) main.kinectButton.Hovering();
                else main.kinectButton.Release();
                Joint lh = leftHand;
                Joint ls = leftShoulder;
                bool isup = lh.Position.Y > ls.Position.Y; //This value is 'true' if the user's left hand is above their left shoulder, and false if it's not.
           
                if (hand.JointType == JointType.HandRight && isup)
                {
                    main.kinectButton.ImageSource = "/PaiKinect;component/Images/right.png";
                    main.kinectButton.ActiveImageSource = "/PaiKinect;component/Images/right.png";
                }
                else
                {
                    main.kinectButton.ImageSource = "/PaiKinect;component/Images/right.png";
                    main.kinectButton.ActiveImageSource = "/PaiKinect;component/Images/right.png";
                }

                cursorPosition = new Point(handX, handY);
                         
                //If the hand has been lifted, reset the past cursor position so that it doesnt pick up where you left off.
                if(isup == true)
                {
                    _pastCursorPosition = new Point(handX, handY);
                    showButtons();
                }
                else
                {
                    if(drawingSquare)
                    {
                        Rectangle square = paintBrush.DrawRectangle(true);
                        if(shapeIncreased)
                        {
                            square.Width += amtChanged;
                            square.Height += amtChanged;
                            shapeIncreased = false;
                            amtChanged = 0;
                        }
                        if(shapeDecreased)
                        {
                            square.Width -= amtChanged;
                            square.Height -= amtChanged;
                            shapeDecreased = false;
                            amtChanged = 0;
                        }
                        main.myCanvas1.Children.Add(square);
                        Canvas.SetLeft(square, handX - (square.Width / 2));
                        Canvas.SetTop(square, handY - (square.Height / 2));
                        drawingSquare = false;
                    }
                    if(drawingRectangle)
                    {
                        Rectangle rectangle = paintBrush.DrawRectangle(false);
                        if (shapeIncreased)
                        {
                            rectangle.Width += amtChanged;
                            rectangle.Height += amtChanged;
                            shapeIncreased = false;
                            amtChanged = 0;
                        }
                        if (shapeDecreased)
                        {
                            rectangle.Width -= amtChanged;
                            rectangle.Height -= amtChanged;
                            shapeDecreased = false;
                        }
                        main.myCanvas1.Children.Add(rectangle);
                        Canvas.SetLeft(rectangle, handX - (rectangle.Width / 2));
                        Canvas.SetTop(rectangle, handY - (rectangle.Height / 2));
                        drawingRectangle = false;
                    }
                    if(drawingCircle)
                    {
                        Ellipse circle = paintBrush.DrawEllipse();
                        if (shapeIncreased)
                        {
                            circle.Width += amtChanged;
                            circle.Height += amtChanged;
                            shapeIncreased = false;
                            amtChanged = 0;
                        }
                        if (shapeDecreased)
                        {
                            circle.Width = circle.Width - amtChanged;
                            circle.Height = circle.Height - amtChanged;
                            shapeDecreased = false;
                            amtChanged = 0;
                        }
                        main.myCanvas1.Children.Add(circle);
                        Canvas.SetLeft(circle, handX - (circle.Width / 2));
                        Canvas.SetTop(circle, handY - (circle.Height / 2));

                        drawingCircle = false;
                    }
                    hideButtons();
                    if (!main.isHandOver(main.kinectButton, main.buttons))
                    {
                        if (_pastCursorPosition == null)
                        {

                            main.kinectButton2.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            main.kinectButton2.Visibility = System.Windows.Visibility.Visible;
                            if (_pastCursorPosition.Value.X > handX)
                            {
                                if ((handX - _pastCursorPosition.Value.X) < -50)
                                {
                                    paintBrush.setPoint(_pastCursorPosition);
                                    paintBrush.setBrush(_previousFill);
                                    Line l = paintBrush.DrawLine(handX, handY);
                                    main.myCanvas1.Children.Add(l);
                                }
                                else
                                {
                                    paintBrush.setPoint(_pastCursorPosition);
                                    paintBrush.setBrush(_previousFill);
                                    Line l = paintBrush.DrawLine(handX, handY);
                                    main.myCanvas1.Children.Add(l);
                                }
                            }
                            else
                            {
                                if ((_pastCursorPosition.Value.X - handX) < -50)
                                {
                                    paintBrush.setPoint(_pastCursorPosition);
                                    paintBrush.setBrush(_previousFill);
                                    Line l = paintBrush.DrawLine(handX, handY);
                                    main.myCanvas1.Children.Add(l);
                                }
                                else
                                {
                                    paintBrush.setPoint(_pastCursorPosition);
                                    paintBrush.setBrush(_previousFill);
                                    Line l = paintBrush.DrawLine(handX, handY);
                                    main.myCanvas1.Children.Add(l);
                                }
                            }

                        }
                        _pastCursorPosition = cursorPosition;
                    }
                }
            }
        }

        #endregion
    }
}
