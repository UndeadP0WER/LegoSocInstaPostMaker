using LegoSocInstaPostMaker.Actions;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Linq;

namespace LegoSocInstaPostMaker {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        /*NEXT UP: 
         * border generation
         * * specify which side, generate based on a few params
         
         * brick settings pannel 
         * * change settings of each brick (colour from a selector, studs from a number selector thingu)
         
         * more layout settings
         * * change resolution, number of bricks per side, number of panels, etc
        */

        #region Variables
        //  visualisation settings 
        (int, int) border = (10, 2);
        (int, int) visSquare = (36,30); //must be a 6:5 to make it a square, also gotta pick the right numbers so the edges line up
        
        //list of bricks, to speed up exporting to png
        List<Brick> brickList = new List<Brick>(); 

        //last cell over, for drag & drop
        (int, int) cell = (0, 0);

        //list of shaded squares that are sent to back to allow event handling
        List<UIElement> sentToBack = new List<UIElement>();

        //notes how many studs the last click was
        int clickToPlace = 4;

        //undo/redo
        ActionManager actions = new ActionManager();

        //deleting multiple bricks at once
        List<Brick> del = new List<Brick>();

        //save file location
        string saveLocation = "";

        //furthest left when loading in
        int leftmost = -1;
        int rightmost = 0;

        //random
        static System.Random rnd = new System.Random();
        #endregion

        #region Init
        //inits components
        public MainWindow() {
            InitializeComponent();

            InitBrickSelector();
            InitBrickViewer();
            
        }

        //brick selector
        private void InitBrickSelector() {
            //makes 1,2,3 and 4 length bricks

            for (int i=1; i<=4; i++) {
                Brick b = new Brick();
                b.Studs = i;
                b.MouseDown += Selector_BrickClick;

                Grid.SetColumn(b, 1);
                Grid.SetRow(b, 2 * i - 1);
                Grid.SetColumnSpan(b, i);
                Grid.SetRowSpan(b, 2);

                Selector.Children.Add(b);
            }
        }

        //brick viewer
        private void InitBrickViewer() {
            //gets total height and width of the viewer
            int x = visSquare.Item1 + (2 * border.Item1);
            int y = visSquare.Item2 + (2 * border.Item2);


            //splits into x colums
            for (int i = 0; i < x; i++) {
                Bricks.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            }
            //splits into y rows
            for (int j = 0; j < y; j++) {
                Bricks.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }


            //for every cell, adds a transparent border to handle dropping into a cell
            for (int i=0; i<x; i++) {
                for (int j=0; j<y; j++) {
                    //grid to handle dropping
                    Grid g = new Grid();
                    //don't allow dropping if its at the top row (the studs will crash it)
                    if (j != 0) {
                        g.AllowDrop = true;
                        g.Drop += Cell_Drop;
                    }
                    

                    //transparent
                    g.Background = System.Windows.Media.Brushes.Transparent;
                    g.Children.Add(new Border());

                    //sets to corresponding grid
                    Grid.SetColumn(g, i);
                    Grid.SetRow(g, j);

                    //if its out of the visSquare, grey out
                    if (i < border.Item1 || i >= (x - border.Item1) ||
                        j < border.Item2 || j >= (y - border.Item2)) {

                        //event for sending to back for deleting bricks behind the shading
                        g.PreviewMouseDown += Shading_PreviewMouseDown;
                        g.PreviewMouseMove += Shading_PreviewMouseMove;

                        g.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 30, 30, 30));
                        Panel.SetZIndex(g, y);
                    }

                    //g.Children.Add(new Label() { Content = (i + ", " + j), FontSize = 10});
                    Bricks.Children.Add(g);
                }
            }

        }

        #endregion

        #region Brick Management
        //clears the viewer ready for re-initialisation
        private void ClearViewer() {
            //clears brick list and viewer
            brickList.Clear();
            Bricks.Children.Clear();
            Bricks.RowDefinitions.Clear();
            Bricks.ColumnDefinitions.Clear();

            //resets leftmost
            leftmost = -1; 
        }

        //adds a brick
        private Brick AddBrick(int studs, System.Windows.Media.Brush colour, int x, int y) {
            Brick b = new Brick();

            //brick colour and width
            b.Studs = studs;
            b.Colour = colour;

            //width and height, _studs colums wide, and 2 rows tall (top is for the studs)
            Grid.SetColumnSpan(b, b.Studs);
            Grid.SetRowSpan(b, 2);

            //sets position and z index 
            // higher up ones are rendered in front, so studs behind don't show through
            Grid.SetColumn(b, x);
            Grid.SetRow(b, y);
            Panel.SetZIndex(b, visSquare.Item2 + border.Item2 - y);

            //event handlers
            b.MouseEnter += Brick_MouseEnter;
            b.MouseDown += Brick_MouseDown;
            

            //adds to list of bricks and visualiser
            brickList.Add(b);
            Bricks.Children.Add(b);

            return b;
        }
        private Brick AddBrick(Brick b) {
            brickList.Add(b);
            Bricks.Children.Add(b);
            return b;
        }

        //removes a brick
        private void DeleteBrick(Brick b) {
            //removes from active use
            brickList.Remove(b);
            Bricks.Children.Remove(b);

        }

        #endregion


        #region Border Generation
        /// <summary>
        /// Generates a set of bricks that fill a horizontal line
        /// </summary>
        /// <param name="start">The X coordinate of the start position of the generation</param>
        /// <param name="end">The X coordinate of the end position of the generation</param>
        /// <param name="y">The Y coordinate the generation is at</param>
        /// <param name="weights">The weights of each brick length. weights[i] is the weighting of bricks with (i+1) studs.</param>
        /// <param name="truncateEnd">If the final brick is truncated to meet the end exactly</param>
        /// <returns>An Array of Bricks that fill the given line, that have also been placed.</returns>
        private Brick[] GenerateHorizontal(int start, int end, int y, int[] weights, bool truncateEnd) {

            //list of generated bricks
            List<Brick> generated = new List<Brick>();

            //location of the current end of the generation
            int curr = start;

            //flag for if filled. if start is the same as end then its filled, else false
            bool filled = (start==end);

            //flag for direction, if start is bigger than end it is filling left to right
            bool leftToRight = (start < end);


            while (!filled) {
                //generates another brick based on weightings
                int studs = GenerateFromWeights(weights) + 1;



                //places the brick and moves the current according to direction
                if (leftToRight) {
                    //checks if it needs to truncate
                    if (curr+studs > end) {
                        studs = end - curr;
                    }

                    generated.Add(AddBrick(studs, BrickColour.GetRandom(), curr, y));
                    curr += studs;

                    if (curr >= end) { 
                        filled = true; 
                    }
                }
                else {
                    if (curr - studs < end) {
                        studs = curr - end;
                    }

                    curr -= studs;
                    generated.Add(AddBrick(studs, BrickColour.GetRandom(), curr, y));

                    if (curr <= end) {
                        filled = true; 
                    }
                }

            }

            //adds this action to the undo stack
            return generated.ToArray();

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="centerX"></param>
        /// <param name="widthWeights"></param>
        /// <returns></returns>
        private Brick[] GenerateVertical(int top, int bottom, double centerX, int[] widthWeights) {
            //list of generated bricks
            List<Brick> generated = new List<Brick>();

            //starts a random weight
            int weight = GenerateFromWeights(widthWeights);

            int curr = bottom;
            int l = 0, r = 0;
            
            while (curr > top) {
                //generates another layer

                //randomly weights to one side (only affects odd)

                if (rnd.Next(1) == 0) {
                    l = (int)System.Math.Floor(centerX) - (int)System.Math.Ceiling((double)(weight-1)/2);
                    r = (int)System.Math.Ceiling(centerX) + (int)System.Math.Floor((double)(weight-1)/2);
                }
                else {
                    //right lean
                    l = (int)System.Math.Floor(centerX) - (int)System.Math.Floor((double)(weight - 1) / 2);
                    r = (int)System.Math.Ceiling(centerX) + (int)System.Math.Ceiling((double)(weight-1)/2);
                }

                //weightings of horizontal bricks
                int[] hWeights = { 0, 5, 4, 5 };

                //generates the horizontal bricks in the layer, from a random direction
                Brick[] layer = rnd.Next(2) == 0 
                    ? GenerateHorizontal(l, r, curr, hWeights, true) 
                    : GenerateHorizontal(r, l, curr, hWeights, true);


                //adds each to the generated list
                foreach (Brick b in layer) {
                    generated.Add(b);
                }

                //picks a new (different) width
                int[] newWeight = (int[])widthWeights.Clone();
                newWeight[weight] = 0;
                weight = GenerateFromWeights(newWeight);

                //updates the y level
                curr--;
            }

            return generated.ToArray();
        }

        #endregion


        #region Undo/Redo

        //handles undoing actions
        private void Undo() {
            Action action = actions.UndoAction();
            if (action == null) { return;  }

            switch (action.ActionType) {
                case ActionType.Place:
                    //undo place
                    foreach (Brick b in (Brick[])action.Data) {
                        DeleteBrick(b);
                    }
                    break;
                case ActionType.Delete:
                    //undo delete
                    foreach (Brick b in (Brick[])action.Data) { 
                        AddBrick(b);
                    }

                    break;
            }

        }

        //handles redoing undone actions
        private void Redo() {
            Action action = actions.RedoAction();
            if (action == null) { return; }

            switch (action.ActionType) {
                case ActionType.Place:
                    //redo place
                    foreach (Brick b in (Brick[])action.Data) { 
                        AddBrick(b); 
                    }
                    break;

                case ActionType.Delete:
                    //redo delete
                    foreach (Brick b in (Brick[])action.Data) {
                        DeleteBrick(b);
                    }
                    break;
            }
        }


        #endregion

        #region Saving/Loading/Exporting

        //saves to a .lgop file
        private void WriteToFile(string file) {
            //sets up writer
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.Indented; //indented format

            //saves

            writer.WriteStartObject();

            //vissquare
            writer.WritePropertyName("xVisSquare");
            writer.WriteValue(visSquare.Item1);
            writer.WritePropertyName("yVisSquare");
            writer.WriteValue(visSquare.Item2);

            //border
            writer.WritePropertyName("xBorder");
            writer.WriteValue(border.Item1);
            writer.WritePropertyName("yBorder");
            writer.WriteValue(border.Item2);

            //bricks
            writer.WritePropertyName("bricks");
            writer.WriteStartArray();
            foreach(Brick b in brickList) {
                b.WriteToJson(writer);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();


            //writes to file
            using (StreamWriter fileWriter = new StreamWriter(file, false)) {
                fileWriter.Write(sb.ToString());
            }

        }

        //loads a .lgop file
        private void ReadFromFile(string file, int off) {

            using (StreamReader sr = File.OpenText(file))
            using (JsonTextReader reader = new JsonTextReader(sr)) {
                //gets json
                JToken json = JToken.ReadFrom(reader);

                //clears viewer
                ClearViewer();

                //sets up viewer variables
                visSquare = (json.Value<int>("xVisSquare"),
                             json.Value<int>("yVisSquare"));

                border = (json.Value<int>("xBorder"),
                          json.Value<int>("yBorder"));

                //reinits brick viewer
                InitBrickViewer();

                //sets up placement offsets
                leftmost = visSquare.Item1 + border.Item1; //sets it to quite far right
                rightmost = 0;

                //places each saved brick
                JArray jar = JArray.Parse(json.Value<JArray>("bricks").ToString());
                foreach (JObject j in jar) {
                    AddBrick(
                        j.Value<int>("studs"),
                        BrickColour.FromName(j.Value<string>("colour")),
                        System.Math.Max(j.Value<int>("x") + off, 0),
                        j.Value<int>("y")
                    );

                    //gets the furthest left brick for further use
                    leftmost = System.Math.Min(j.Value<int>("x") + off, leftmost);
                    rightmost = System.Math.Max((j.Value<int>("x") + j.Value<int>("studs") + off), rightmost);
                }


            }


        }

        #endregion

        #region MenuBarThings

        //opens a new 
        private void New() {
            //checks if there are unsaved changes

            //clears current viewer
            ClearViewer();

            //reinits the viewer
            InitBrickViewer();

            //clears the active file
            saveLocation = "";
        }

        private void Save() {
            //if a file exists, save to it, else save as
            if (saveLocation == "") {
                SaveAs();
                return;
            }

            WriteToFile(saveLocation);
        }

        private void SaveAs() {
            //saves to a new file
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "Lego Insta Post file (*.lgop)|*.lgop";
            if (s.ShowDialog() == true) {
                WriteToFile(s.FileName);
                saveLocation = s.FileName;
            }
        }

        #endregion

        #region Events

        //resizes the viewer
        private void Bricks_SizeChanged(object sender, SizeChangedEventArgs e) {
            //gets grid size
            int x = visSquare.Item1 + (2 * border.Item1);
            int y = visSquare.Item2 + (2 * border.Item2);

            //*gets brick unit
            double w = (MainGrid.ColumnDefinitions[2].ActualWidth - 40) / (5 * x);
            double h = (MainGrid.RowDefinitions[0].ActualHeight - 40) / (6 * y);

            //selects the smaller one so that it all fits on the grid
            double d = (w > h) ? h : w;

            //sizes up with the correct brick sizes
            Bricks.Width = d * 5 * x;
            Bricks.Height = d * 6 * y;
        }

        #region Keyboard Shortcuts

        //keyboard events
        private void MainGrid_KeyDown(object sender, KeyEventArgs e) {
            //ctrl shift s
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && 
                e.KeyboardDevice.IsKeyDown(Key.LeftShift) &&
                e.Key == Key.S) {
                //save as
                SaveAs();
            }

            //ctrl s
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) &&
                e.Key == Key.S) {
                //save
                Save();
            }

            //ctrl z
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) &&
                e.Key == Key.Z) {
                //undo
                Undo();
            }

            //ctrl y
            else if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) &&
                e.Key == Key.Y) {
                //redo
                Redo();
            }

        }

        #endregion

        #region Placing
        //handles dropping event, if data is a brick xaml, places a brick
        private void Cell_Drop(object sender, DragEventArgs e) {
            cell = (Grid.GetColumn((UIElement)sender), Grid.GetRow((UIElement)sender));
           
            if (e.Data.GetDataPresent(DataFormats.Xaml)) {
                //if the drop data is a brick
                if (e.Data.GetData(DataFormats.Xaml).GetType() == typeof(Brick)) {
                    Brick b = AddBrick(
                            ((Brick)e.Data.GetData(DataFormats.Xaml)).Studs,
                            BrickColour.GetRandom(),
                            cell.Item1,
                            cell.Item2 - 1
                        );

                    actions.DoAction(new Action(ActionType.Place, new Brick[1] { b }));
                }
            
                e.Handled = true;
            }
        }

        //handles click down event for viewer
        private void Grid_Click(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Brick b = new Brick() { Studs = clickToPlace };
                
                //starts a drag drop for placement
                // allows the placement to be moved before releasing
                DragDrop.DoDragDrop(b,
                                    new DataObject(DataFormats.Xaml, b),
                                    DragDropEffects.Copy);
            }
            e.Handled = true;
        }

        //handles clicking in the selector
        private void Selector_BrickClick(object sender, MouseEventArgs e) {
            //updates the click to place studs
            if (sender.GetType() == typeof(Brick) && 
                e.LeftButton == MouseButtonState.Pressed) {

                clickToPlace = ((Brick)sender).Studs;
            }
        }

        //Brick_MouseDown handles clicking of bricks on the viewer, handled in Deleting region

        #endregion

        #region Deleting
        //brick right click to delete
        private void Brick_MouseDown(object sender, MouseEventArgs e) {
            if (sender.GetType() == typeof(Brick)) { 
                //right click, deleting
                if (e.MouseDevice.RightButton == MouseButtonState.Pressed) {
                    DeleteBrick((Brick)sender);
                    del.Add((Brick)sender);
                }
                //left click, selecting new brick to place
                else if (e.MouseDevice.LeftButton == MouseButtonState.Pressed) {
                    clickToPlace = ((Brick)sender).Studs;
                }
            }
        }
       
        //brick drag to delete
        private void Brick_MouseEnter(object sender, MouseEventArgs e) {
            if (sender.GetType() == typeof(Brick) &&
                e.MouseDevice.RightButton == MouseButtonState.Pressed) {
                DeleteBrick((Brick)sender);
                del.Add((Brick)sender);
            }
        }

        //moving shading cells out the way to allow the brick events to be raised
        //mouse down to delete
        private void Shading_PreviewMouseDown(object sender, MouseEventArgs e) {
            //sends sender to back
            Panel.SetZIndex((UIElement)sender, 0);
            sentToBack.Add((UIElement)sender);
        }

        //drag to delete
        private void Shading_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.RightButton == MouseButtonState.Pressed) {
                //sends sender to back
                Panel.SetZIndex((UIElement)sender, 0);
                sentToBack.Add((UIElement)sender);
            }
        }
        
        //returns shading cells to their correct z index for correct visualisation
        private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e) {
            //if right button up, send the deletion action to the manager
            if (e.ChangedButton == MouseButton.Right && del.Count > 0) {
                //sends the list of deleted bricks (if there are any)
                actions.DoAction(new Action(ActionType.Delete, del.ToArray()));
                //new list for the next deletion action
                del = new List<Brick>();
            }

            //moves elements moved to back back to the front
            foreach (UIElement g in sentToBack) {
                Panel.SetZIndex(g, visSquare.Item2 + (2 * border.Item2));
            }
            sentToBack.Clear();
        }

        #endregion

        #region Menu bar

        #region File
        //opens a new viewer
        private void New_Click(object sender, RoutedEventArgs e) {
            New();
        }

        //saves project, if no active save location, save as is used instead
        private void Save_Click(object sender, RoutedEventArgs e) {
            Save();
        }

        //saves project to new file location
        private void SaveAs_Click(object sender, RoutedEventArgs e) {
            SaveAs();
        }

        //opens a project in the center
        private void OpenCentre_Click(object sender, RoutedEventArgs e) {
            //opens a file, and opens it at the centre
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Lego Insta Post file (*.lgop)|*.lgop";
            if (d.ShowDialog() == true) {
                ReadFromFile(d.FileName, 0);
            }

        }

        //opens a project to the right
        // for building a border that lines up when placed to the right
        private void OpenRight_Click(object sender, RoutedEventArgs e) {
            //opens a file, and opens it to the right
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Lego Insta Post file (*.lgop)|*.lgop";
            if (d.ShowDialog() == true) {
                ReadFromFile(d.FileName, visSquare.Item1);
            }

        }

        //opens a project file to the left
        // for building a border that lines up when placed to the left       
        private void OpenLeft_Click(object sender, RoutedEventArgs e) {
            //opens a file, and opens it to the left
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Lego Insta Post file (*.lgop)|*.lgop";
            if (d.ShowDialog() == true) {
                ReadFromFile(d.FileName, -visSquare.Item1);
            }

        }

        #endregion

        #region Export
        //export event
        private void Export_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "PNG (*.png)|*.png";
            if (s.ShowDialog() == true) { 
                Export(1080, 1080, s.FileName);
            }
        }

        //exports to a file file with dimensions x by y
        private void Export(int x, int y, string file) {
            
            //checks if the current settings gives a square
            int brickUnit = (x) / (visSquare.Item1*5);
            if (brickUnit != (y) / (visSquare.Item2*6)) {
                MessageBox.Show("Current settings cannot be a square!");
                return;
            }
            //settings is a square

            //post bitmap
            Bitmap post = new Bitmap(x, y);
            Graphics gr = Graphics.FromImage(post);

            //draws the studs first so they are drawn over if there's a brick above it
            foreach (Brick b in brickList) {
                PointF pos = new PointF((Grid.GetColumn(b) - border.Item1) * brickUnit * 5, 
                                        (Grid.GetRow(b) - border.Item2) * brickUnit * 6);
                TextureBrush t = b.StudBrush(brickUnit, 1);


                gr.DrawImage(t.Image, pos);
            }

            //draws the rest of the brick over the studs
            foreach (Brick b in brickList) {
                PointF pos = new PointF((Grid.GetColumn(b) - border.Item1) * brickUnit * 5,
                                        (Grid.GetRow(b) - border.Item2 + 1) * brickUnit * 6);
                TextureBrush t = b.BrickBrush(brickUnit, 1);


                gr.DrawImage(t.Image, pos);

            }

            post.Save(file);

        }


        #endregion

        #region Generate

        //border at the top
        private void GenerateTop_Click(object sender, RoutedEventArgs e) {
            //where to start from
            int offset = GenerateFromWeights(new int[] { 1,1,1,1 });
            //weights of each length of brick
            int[] weights = { 3, 7, 10, 80 };

            //settings for if there's a border loaded already
            int r;
            bool hasLeftmost;
            if (leftmost < 0 ) {
                r = (border.Item1 + visSquare.Item1) + GenerateFromWeights(new int[] { 1, 1, 1, 1 });
                hasLeftmost = false;
            }
            else {
                r = leftmost;
                hasLeftmost = true;
            }

            //generates the bricks
            Brick[] generated = GenerateHorizontal(
                border.Item1 - offset, 
                r, 
                (border.Item2 - 1), 
                weights,
                hasLeftmost);
        
            //adds an action of placing them all down
            actions.DoAction(new Action(ActionType.Place, generated));
        }

        //border at the bottom
        private void GenerateBottom_Click(object sender, RoutedEventArgs e) {
            //where to start from
            int offset = GenerateFromWeights(new int[] { 1, 1, 1, 1 });
            //weights of each length of brick
            int[] weights = { 3, 7, 10, 80 };

            //settings for if there's a border loaded already
            int r;
            bool hasLeftmost;
            if (leftmost < 0) {
                r = (border.Item1 + visSquare.Item1) + GenerateFromWeights(new int[] { 1, 1, 1, 1 });
                hasLeftmost = false;
            }
            else {
                r = leftmost;
                hasLeftmost = true;
            }

            //generates the bricks
            Brick[] generated = GenerateHorizontal(
                border.Item1 - offset, 
                r,
                (border.Item2 + visSquare.Item2 - 2), 
                weights,
                hasLeftmost);

            //adds an action of placing them all down
            actions.DoAction(new Action(ActionType.Place, generated));
        }

        //border at the left side
        private void GenerateLeft_Click(object sender, RoutedEventArgs e) {
            //weights of each possible width
            int[] weights = { 0, 0, 0, 2, 30, 10, 30, 7, 1};

            //generates the bricks
            Brick[] generated = GenerateVertical(
                border.Item2/2 - 1, 
                (border.Item2/2) + visSquare.Item2 - 1, 
                border.Item1+0.5, 
                weights);

            //adds an action of placing them all down
            actions.DoAction(new Action(ActionType.Place, generated));
        }

        //border at the right side
        private void GenerateRight_Click(object sender, RoutedEventArgs e) {
            //weights of each possible width
            int[] weights = { 0, 0, 0, 2, 30, 10, 30, 7, 1 };

            //generates the bricks
            Brick[] generated = GenerateVertical(
                border.Item2 / 2 - 1,
                (border.Item2 / 2) + visSquare.Item2 - 1,
                border.Item1 + visSquare.Item1 + 0.5,
                weights);

            //adds an action of placing them all down
            actions.DoAction(new Action(ActionType.Place, generated));
        }


        //top, bottom and left border
        private void GenerateTBL_Click(object sender, RoutedEventArgs e) {
            //where to start from
            int offset = GenerateFromWeights(new int[] { 1, 1, 1, 1 });
            //weights of each side
            int[] hWeights = { 3, 7, 10, 80 };
            int[] vWeights = { 0, 0, 0, 2, 30, 10, 30, 7, 1 };

            //settings for if there's a border loaded already
            int right;
            bool hasLeftmost;
            if (leftmost < 0) {
                right = (border.Item1 + visSquare.Item1) + GenerateFromWeights(new int[] { 1, 1, 1, 1 });
                hasLeftmost = false;
            }
            else {
                right = leftmost;
                hasLeftmost = true;
            }

            //generates top
            Brick[] t = GenerateHorizontal(
                border.Item1 - offset,
                right,
                (border.Item2 - 1),
                hWeights,
                hasLeftmost);

            //regenerates offset
            offset = GenerateFromWeights(new int[] { 1, 1, 1, 1 });

            //generates bottom
            Brick[] b = GenerateHorizontal(
                border.Item1 - offset,
                right,
                (border.Item2 + visSquare.Item2 - 2),
                hWeights,
                hasLeftmost);


            //generates left
            Brick[] l = GenerateVertical(
                border.Item2 / 2,
                (border.Item2 / 2) + visSquare.Item2 - 2,
                border.Item1 + 0.5,
                vWeights);

            //collects all generated bricks into one array
            Brick[] generated = new Brick[t.Length + b.Length + l.Length];
            t.CopyTo(generated, 0);
            b.CopyTo(generated, t.Length);
            l.CopyTo(generated, t.Length + b.Length);


            //adds an action of placing them all down
            actions.DoAction(new Action(ActionType.Place, generated));
        }

        //top, bottom and left border
        private void GenerateTBR_Click(object sender, RoutedEventArgs e) {
            //where to start from
            int offset = GenerateFromWeights(new int[] { 1, 1, 1, 1 });
            //weights of each side
            int[] hWeights = { 3, 7, 10, 80 };
            int[] vWeights = { 0, 0, 0, 2, 30, 10, 30, 7, 1 };

            //settings for if there's a border loaded already
            int left;
            bool hasRightmost;
            if (rightmost == 0) {
                left = (border.Item1 + visSquare.Item1) + GenerateFromWeights(new int[] { 1, 1, 1, 1 });
                hasRightmost = false;
            }
            else {
                left = rightmost;
                hasRightmost = true;
            }


            //generates top
            Brick[] t = GenerateHorizontal(
                border.Item1 + visSquare.Item1 + offset,
                left,
                (border.Item2 - 1),
                hWeights,
                hasRightmost);

            //regenerates offset
            offset = GenerateFromWeights(new int[] { 1, 1, 1, 1 });

            //generates bottom
            Brick[] b = GenerateHorizontal(
                border.Item1 + visSquare.Item1 + offset,
                left,
                (border.Item2 + visSquare.Item2 - 2),
                hWeights,
                hasRightmost);


            //generates right
            Brick[] r = GenerateVertical(
                border.Item2 / 2,
                (border.Item2 / 2) + visSquare.Item2 - 2,
                border.Item1 + visSquare.Item1 + 0.5,
                vWeights);

            //collects all generated bricks into one array
            Brick[] generated = new Brick[t.Length + b.Length + r.Length];
            t.CopyTo(generated, 0);
            b.CopyTo(generated, t.Length);
            r.CopyTo(generated, t.Length + b.Length);


            //adds an action of placing them all down
            actions.DoAction(new Action(ActionType.Place, generated));
        }

        //all four borders
        private void GenerateAll_Click(object sender, RoutedEventArgs e) {
            //where to start from
            int offset = GenerateFromWeights(new int[] { 1, 1, 1, 1 });
            //weights of each side
            int[] hWeights = { 3, 7, 10, 80 };
            int[] vWeights = { 0, 0, 0, 2, 30, 10, 30, 7, 1 };

            //settings for if there's a border loaded already
            int right;
            bool hasLeftmost;
            if (leftmost < 0) {
                right = (border.Item1 + visSquare.Item1) + GenerateFromWeights(new int[] { 1, 1, 1, 1 });
                hasLeftmost = false;
            }
            else {
                right = leftmost;
                hasLeftmost = true;
            }

            //generates top
            Brick[] t = GenerateHorizontal(
                border.Item1 - offset,
                right,
                (border.Item2 - 1),
                hWeights,
                hasLeftmost);
            
            //regenerates offset
            offset = GenerateFromWeights(new int[] { 1, 1, 1, 1 });
            
            //generates bottom
            Brick[] b = GenerateHorizontal(
                border.Item1 - offset,
                right,
                (border.Item2 + visSquare.Item2 - 2),
                hWeights,
                hasLeftmost);


            //generates left
            Brick[] l = GenerateVertical(
                border.Item2 / 2,
                (border.Item2 / 2) + visSquare.Item2 - 2,
                border.Item1 + 0.5,
                vWeights);

            //generates right
            Brick[] r = GenerateVertical(
                border.Item2 / 2 ,
                (border.Item2 / 2) + visSquare.Item2 - 2,
                border.Item1 + visSquare.Item1 + 0.5,
                vWeights);

            //collects all generated bricks into one array
            Brick[] generated = new Brick[t.Length + b.Length + l.Length + r.Length];
            t.CopyTo(generated, 0);
            b.CopyTo(generated, t.Length);
            l.CopyTo(generated, t.Length + b.Length);
            r.CopyTo(generated, t.Length + b.Length + l.Length);


            //adds an action of placing them all down
            actions.DoAction(new Action(ActionType.Place, generated));
        }

        #endregion

        #endregion

        #endregion


        #region Utils
        //picks a random index from a list of weights
        static int GenerateFromWeights(int[] weights) {
            int sum = weights.Sum();

            int r = rnd.Next(sum);

            for (int i=0; i<weights.Length; i++) {
                if (weights[i]>r) {
                    return i;
                }
                else {
                    r -= weights[i];
                }
            }

            return weights.Last();
        }


        #endregion

    }

}
         
         