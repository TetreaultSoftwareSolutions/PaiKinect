using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    /*
     * This class handles all of the painting being done in this application.
     */
    class PaintHandler
    {

        
        public Point? point;  //The current point that the user's hand is on
        public SolidColorBrush brush;  //The brush being used to draw the line
        
        /*
         * This constructor instantiates the brush and point variables.
         */
        public PaintHandler(Point? inputPoint, SolidColorBrush brush){
            
            this.point = inputPoint;
            this.brush = brush;
        }

        #region Getters_and_Setters
        public void setPoint(Point? point)
        {
            this.point = point;
        }

        public void setBrush(SolidColorBrush brush)
        {
            this.brush = brush;
        }
        #endregion

        /*
         * This method takes two floats, and creates a line based on those floats passed in (by the kinect sensor). It
         * then returns the line created.
         */
        public Line DrawLine(float x, float y)
        {

            
            Line ln = new Line
            {
                X1 = point.Value.X,
                Y1 = point.Value.Y,
                X2 = x,
                Y2 = y,
                StrokeThickness = 10,
                StrokeLineJoin = PenLineJoin.Round
            };

            ln.Stroke = new SolidColorBrush(brush.Color);
            ln.StrokeDashCap = PenLineCap.Round;
            ln.StrokeStartLineCap = PenLineCap.Round;
            ln.StrokeEndLineCap = PenLineCap.Round;
            //canvas.Children.Add(ln);
            return ln;


        }

    }
}
