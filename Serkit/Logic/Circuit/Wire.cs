using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Logic.Circuit.Controls
{
    public class Wire
    {
        List<Point> points;

        public SolidColorBrush High = new SolidColorBrush(new Color() { A = 255, R = 0, G = 0, B = 0 });
        public SolidColorBrush Low = new SolidColorBrush(new Color() { A = 255, R = 128, G = 128, B = 128 });
        public SolidColorBrush Indeterminate = new SolidColorBrush(new Color() { A = 255, R = 255, G = 90, B = 90 });

        public Wire()
        {
            points = new List<Point>();
        }

        public Line Add(Point p, Canvas canvas)
        {
            points.Add(p);
            if(Count >= 2){
                Line line = new Line
                {
                    X1 = points[Count - 2].X,
                    Y1 = points[Count - 2].Y,
                    X2 = points[Count - 1].X,
                    Y2 = points[Count - 1].Y,
                    Stroke = Low,
                    StrokeThickness = 1
                };
                canvas.Children.Add(line);
                return line;
            }
            return null;
        }

        public int Count { get { return points.Count; } }

        public Point getPoint(int idx)
        {
            return points[idx];
        }
    }
}
