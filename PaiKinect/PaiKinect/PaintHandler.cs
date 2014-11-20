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
    class PaintHandler
    {

        
        public Point? point;
        public SolidColorBrush brush;
        
        public PaintHandler(Point? inputPoint, SolidColorBrush brush){
            
            this.point = inputPoint;
            this.brush = brush;
        }
        
        public void setPoint(Point? point)
        {
            this.point = point;
        }

        public void setBrush(SolidColorBrush brush)
        {
            this.brush = brush;
        }
        
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
