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


        /*The current point that the user's hand is on*/
        public Point? point;
        /*The brush being used to draw the line*/
        public SolidColorBrush brush;
        /*The Stroke thickness of the brush*/
        int thickness; 
        /*The increment that the shapes change size with*/
        private static int SIZE_INCREMENT = 50;

        
        
        /*
         * This constructor instantiates the brush and point variables.
         * @param inputPoint the point that the cursor is at
         * @param brush the brush used to add the lines
         * @param strokeThickness the thickness of the line
         */
        public PaintHandler(Point? inputPoint, SolidColorBrush brush, int strokeThickness){
            
            this.point = inputPoint;
            this.brush = brush;
            thickness = strokeThickness;
        }

        #region Getters_and_Setters
        /**
         * Sets the point location
         * @param point the point that is being set
         */
        public void setPoint(Point? point)
        {
            this.point = point;
        }
        /**
         * Sets the brush being used to draw the lines
         * @param brush the brush that will be changed
         */
        public void setBrush(SolidColorBrush brush)
        {
            this.brush = brush;
        }
        /**
         * Sets the stroke thickness
         * @param thickness the number that will be used to determine the thickness of the stroke
         */
        public void setStrokeThickness(int thickness)
        {
            this.thickness = thickness;
        }
        #endregion

        /*
         * This method takes two floats, and creates a line based on those floats passed in (by the kinect sensor). It
         * then returns the line created.
         * @param x The x Coordinate
         * @param y The y Coordinate
         * @return the line that is being added to the canvas
         */
        public Line DrawLine(float x, float y)
        {

            
            Line ln = new Line
            {
                X1 = point.Value.X,
                Y1 = point.Value.Y,
                X2 = x,
                Y2 = y,
                StrokeThickness = thickness,
                StrokeLineJoin = PenLineJoin.Round
            };

            ln.Stroke = new SolidColorBrush(brush.Color);
            ln.StrokeDashCap = PenLineCap.Round;
            ln.StrokeStartLineCap = PenLineCap.Round;
            ln.StrokeEndLineCap = PenLineCap.Round;
            //canvas.Children.Add(ln);
            return ln;


        }
        /**
         * Draws an ellipse and returns it
         * @return the ellipse being added to the canvas
         */
        public Ellipse DrawEllipse()
        {
            Ellipse e = new Ellipse
            {
                Fill = new SolidColorBrush(brush.Color),
                Width = 100,
                Height = 100,
                Opacity = 1,
                Margin = new Thickness(10, 10, 0, 0)

            };
            return e;
        }
        /**
         * Draws a rectangle or square and return sit
         * @param isSquare if the object being drawn is a square or a rectangle
         * @return the object being added to the canvas
         */
        public Rectangle DrawRectangle(bool isSquare)
        {
            if (isSquare)
            {
                Rectangle r = new Rectangle
                {
                    Fill = new SolidColorBrush(brush.Color),
                    Width = 100,
                    Height = 100,
                    Opacity = 1,
                    Margin = new Thickness(10, 10, 0, 0)

                };
                return r;
            }
            else
            {
                Rectangle r = new Rectangle
                {
                    Fill = new SolidColorBrush(brush.Color),
                    Width = 150,
                    Height = 100,
                    Opacity = 1,
                    Margin = new Thickness(10, 10, 0, 0)
                    
                };
                return r;
            }
        }
        /**
         * Changes the size of the Ellipse
         * @param e The ellipse that is being changed
         * @bool isIncrease if the size will be increased or decreased
         * @return the ellipse that was changed
         */
        public Ellipse ChangeEllipseSize(Ellipse e, bool isIncrease)
        {
            if(isIncrease)
            {
                e.Width = e.Width + SIZE_INCREMENT;
                e.Height = e.Height + SIZE_INCREMENT;    
            }
            else
            {
                e.Width = e.Width - SIZE_INCREMENT;
                e.Height = e.Height - SIZE_INCREMENT;
            }           
            return e;
        }
        /**
        * Changes the size of the rectangle object
        * @param e The rectangle object that is being changed
        * @bool isIncrease if the size will be increased or decreased
        * @return the rectangle object that was changed
        */
        public Rectangle ChangeRectangleSize(Rectangle r, bool isIncrease)
        {
            if(isIncrease)
            {
                r.Width = r.Width + SIZE_INCREMENT;
                r.Height = r.Height + SIZE_INCREMENT;
            }
            else
            {
                r.Width = r.Width - SIZE_INCREMENT;
                r.Height = r.Height - SIZE_INCREMENT;
            }
            return r;
        }


    }
}
