using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Serialization;

namespace Lab5
{
    public class Shape
    {
        private const double RECTANGLE_WIDTH_FACTOR = 2.0;
        private const double RECTANGLE_HEIGHT_FACTOR = 0.6;
        private const double ARROW_HEAD_SIZE_FACTOR = 0.8;
        private const double GRADIENT_ANIMATION_TARGET = 0.3;
        private const double GRADIENT_ANIMATION_DURATION = 2.0;

        public Shape()
        {
        }

        public Shape(int thickness, Color? background, Color? foreground, int width, int height)
        {
            Thickness = thickness;
            Background = background;
            Foreground = foreground;
            Width = width;
            Height = height;
        }

        public int Thickness { get; set; } = 0;
        public Color? Background { get; set; }
        public Color? Foreground { get; set; }

        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        public void draw(Canvas canvas, Point position)
        {
            var gradient = CreateLinearGradient();
            AnimateGradient(gradient);

            var arrowBody = CreateArrowBody(gradient, position);

            canvas.Children.Add(arrowBody);
        }

        private System.Windows.Shapes.Path CreateArrowBody(LinearGradientBrush gradient, Point position)
        {
            double rectWidth = Width * RECTANGLE_WIDTH_FACTOR;
            double rectHeight = Height * RECTANGLE_HEIGHT_FACTOR;
            double headSize = Height * ARROW_HEAD_SIZE_FACTOR;

            double top = position.Y - rectHeight / 2;
            double left = position.X - rectWidth;

            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = new Point(left, top) };

            figure.Segments.Add(new LineSegment(new Point(position.X, top), true)); // верхняя сторона прямоугольника
            figure.Segments.Add(new LineSegment(new Point(position.X, top - (rectHeight / 2.0)), true));
            figure.Segments.Add(new LineSegment(new Point(position.X + headSize, position.Y), true)); // треугольная голова
            figure.Segments.Add(new LineSegment(new Point(position.X, top + (rectHeight * 1.5)), true));
            figure.Segments.Add(new LineSegment(new Point(position.X, top + rectHeight), true)); // нижняя сторона прямоугольника
            figure.Segments.Add(new LineSegment(new Point(left, top + rectHeight), true)); // левая сторона прямоугольника
            figure.IsClosed = true;

            geometry.Figures.Add(figure);

            return new System.Windows.Shapes.Path
            {
                Data = geometry,
                Fill = gradient,
                Stroke = new SolidColorBrush((Color)Foreground),
                StrokeThickness = Thickness
            };

        }

        private void AnimateGradient(LinearGradientBrush gradient)
        {
            var gradientAnimation = new DoubleAnimation
            {
                To = GRADIENT_ANIMATION_TARGET,
                Duration = TimeSpan.FromSeconds(GRADIENT_ANIMATION_DURATION),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            gradient.GradientStops[1].BeginAnimation(GradientStop.OffsetProperty, gradientAnimation);
        }

        private LinearGradientBrush CreateLinearGradient()
        {
            return new LinearGradientBrush
            {
                StartPoint = new Point(0, 0.5),
                EndPoint = new Point(1, 0.5),
                GradientStops =
                {
                    new GradientStop((Color)Background, 0),
                    new GradientStop((Color)Foreground, 1)
                }
            };
        }

        public void save()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Файлы xml|*.xml|Все файлы|*.*";
            if (fileDialog.ShowDialog() == false) return;
            XmlSerializer serializer = new XmlSerializer(typeof(Shape));
            using (FileStream file = new FileStream(fileDialog.FileName, FileMode.Create))
            {
                serializer.Serialize(file, this);
            }
        }

        public static Shape load()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файлы xml|*.xml|Все файлы|*.*";
            if (fileDialog.ShowDialog() == false) return null;
            XmlSerializer serializer = new XmlSerializer(typeof(Shape));
            Shape shape;
            using (FileStream file = new FileStream(fileDialog.FileName, FileMode.Open))
            {
                shape = (Shape)serializer.Deserialize(file);
            }
            return shape;
        }

        public override string? ToString()
        {
            return $"Thickness = {Thickness}  Background = {Background}  Foreground = {Foreground}\n" +
                $"Width = {Width}  Height = {Height}";
        }
    }
}