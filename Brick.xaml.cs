using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing;
using Newtonsoft.Json;


namespace LegoSocInstaPostMaker {
    /// <summary>
    /// Interaction logic for Brick.xaml
    /// </summary>
    public partial class Brick : UserControl {
        #region Variables

        #region Studs
        private int _studs;
        public int Studs {
            get { return _studs; }
            set { _studs = value; UpdateStuds(); } }
        #endregion

        #region Colour
        private System.Windows.Media.Brush _colour = System.Windows.Media.Brushes.Red;
        public System.Windows.Media.Brush Colour { 
            get { return _colour; }
            set { _colour = value; UpdateStuds(); } }
        #endregion

        #endregion

        public Brick() {
            InitializeComponent();
            UpdateStuds();
        }

        //updates the visualisation with the updated number of studs
        private void UpdateStuds() {
            //sets studs to a minimum of 1
            if (_studs < 1) { _studs = 1; }

            //clears the existing studs and also background
            grid.Children.Clear();
            
            //clears the existing properties
            grid.ColumnDefinitions.Clear();


            //places the studs
            for (int i=0; i<_studs; i++) {
                //adds the stud sizing
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                //adds the stud background
                Border b = new Border() {
                    Background = _colour,
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    BorderThickness = new Thickness(0, 0, 1, 1)
                };
                Grid.SetRow(b, 1);
                Grid.SetColumn(b, (3 * i) + 1);
                grid.Children.Add(b);
                

            }

            //places the rest of the brick
            Border br = new Border() { 
                Background = _colour,
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1)
            };

            Grid.SetRow(br, 2);
            Grid.SetColumnSpan(br, 3 * _studs);
            grid.Children.Add(br);

        }

        #region Events
        //handles starting the drag event
        private void Grid_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop(this, new DataObject(DataFormats.Xaml, this), DragDropEffects.Copy);
            }
            e.Handled = true;
        }

        #endregion

        #region ToBrush
        /// <summary>
        /// Gets a texture brush with the studs of the brick.
        /// </summary>
        /// <param name="brickUnitSize"> how large a brick unit is in pixels (each brick stud is 1 unit tall, 3 units wide. </param>
        /// <param name="borderWidth"> how many pixels wide the border is. </param>
        /// <returns> a texture brush, 6 brickUnits tall, 5*studCount brickUnits tall, with the studs evenly spaced at the bottom. </returns>
        public TextureBrush StudBrush(int brickUnitSize, int borderWidth) {
            Bitmap bm = new Bitmap(brickUnitSize * 5 * _studs, brickUnitSize * 6);
            Graphics gr = Graphics.FromImage(bm);

            //gets the colour
            byte A = ((System.Windows.Media.SolidColorBrush)_colour).Color.A;
            byte R = ((System.Windows.Media.SolidColorBrush)_colour).Color.R;
            byte G = ((System.Windows.Media.SolidColorBrush)_colour).Color.G;
            byte B = ((System.Windows.Media.SolidColorBrush)_colour).Color.B;

            //sets up the fill and border pens
            Brush fill = new SolidBrush(Color.FromArgb(A, R, G, B));
            Pen border = new Pen(Color.FromArgb(255, 0, 0, 0), borderWidth);

            //offset for adjusting for the pen width
            int off = (borderWidth + 1) / 2;

            /* studs */
            for (int i=0; i<_studs; i++) {
                //main fill
                gr.FillRectangle(fill, ((i * 5) + 1) * brickUnitSize, brickUnitSize * 5, brickUnitSize * 3, brickUnitSize);

                //border
                //bottom, left to right
                gr.DrawLine(border,
                    new System.Drawing.Point(((i * 5) + 1) * brickUnitSize, (brickUnitSize * 6) - off),
                    new System.Drawing.Point(((i * 5) + 4) * brickUnitSize- off, (brickUnitSize * 6) - off));
                //right, top down
                gr.DrawLine(border,
                    new System.Drawing.Point((((i * 5) + 4) * brickUnitSize) - off, brickUnitSize * 5),
                    new System.Drawing.Point((((i * 5) + 4) * brickUnitSize) - off, (brickUnitSize * 6)));

            }

            return new TextureBrush(bm);
        }

        /// <summary>
        /// Gets a texture brush with the body of the brick.
        /// </summary>
        /// <param name="brickUnitSize"> how large a brick unit is in pixels (each 1x1 brick is 6 units tall, 5 units wide (excluding studs). </param>
        /// <param name="borderWidth"> how amny pixels wide the border is. </param>
        /// <returns> a texture brush, 6 brickUnits tall, 5*studCount brickUnits tall, filled with the brick colour. </returns>
        public TextureBrush BrickBrush(int brickUnitSize, int borderWidth) {

            Bitmap bm = new Bitmap(brickUnitSize * 5 * _studs, brickUnitSize * 6);
            Graphics gr = Graphics.FromImage(bm);

            //gets the colour
            byte A = ((System.Windows.Media.SolidColorBrush)_colour).Color.A;
            byte R = ((System.Windows.Media.SolidColorBrush)_colour).Color.R;
            byte G = ((System.Windows.Media.SolidColorBrush)_colour).Color.G;
            byte B = ((System.Windows.Media.SolidColorBrush)_colour).Color.B;

            //sets up the fill and border pens
            Brush fill = new SolidBrush(Color.FromArgb(A, R, G, B));
            Pen border = new Pen(Color.FromArgb(255, 0, 0, 0), borderWidth);

            //offset for adjusting for the pen width
            int off = (borderWidth + 1) / 2;

            /* main area */
            //main fill
            gr.FillRectangle(fill, 0, 0, brickUnitSize * 5 * _studs, brickUnitSize * 6);

            //border
            //bottom
            gr.DrawLine(border,
                new System.Drawing.Point(0, (brickUnitSize * 6) - off),
                new System.Drawing.Point(brickUnitSize * 5 * _studs, (brickUnitSize * 6) - off));
            //right
            gr.DrawLine(border,
                new System.Drawing.Point((brickUnitSize * 5 * _studs) - off, 0),
                new System.Drawing.Point((brickUnitSize * 5 * _studs) - off, brickUnitSize * 6));

            return new TextureBrush(bm);
        }

        #endregion

        #region Json
        //Writes the brick to json as a json object
        public void WriteToJson(JsonWriter writer) {
            writer.WriteStartObject();

            //studs
            writer.WritePropertyName("studs");
            writer.WriteValue(_studs);

            //colour
            writer.WritePropertyName("colour");
            writer.WriteValue(BrickColour.GetName(_colour));

            //grid location
            writer.WritePropertyName("x");
            writer.WriteValue(Grid.GetColumn(this));
            writer.WritePropertyName("y");
            writer.WriteValue(Grid.GetRow(this));


            writer.WriteEndObject();
        }

        #endregion


    }
}
