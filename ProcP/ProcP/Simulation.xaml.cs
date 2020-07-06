using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using MySqlX.XDevAPI.Relational;
using ProcP.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Interop;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
//using Microsoft.WindowsAPICodePack.Dialogs;


namespace ProcP
{
    /// <summary>
    /// Interaction logic for Simulation.xaml
    /// </summary>
    public partial class Simulation : System.Windows.Controls.Page
    {
        private int[][] matrix;
        private int beltCount;
        private int gridSize;
        private Airport airport;
        private List<Thread> threads;
        private List<List<Belt>> shortestPaths;
        private Belt[,] grid;
        private List<Belt> vertices;
        private int luggageCount;
        private SolidColorBrush col;
        // int totalCarts;
        private int totalflights;
        int cartsPerGate;
        List<ProgressBar>[] cartsPb;
        int[] luggages;
        public int totalCarts { get; set; }
        public int estimateEmployees { get; set; }
        public int actualEmployees { get; set; }
        public int estimateCarts { get; set; }
        private List<Belt> loadbelts;
        public int extraemployees;
        public int extraCarts;
        private CurrentState state;
        private string file_name;
        int minFlightsNr;
        int nrFlightsLeft;
        int[] flights = new int[3];


        public Simulation(int carts, int nrOfEmployees, int nrOfFlights, List<Belt> loadbelts, CurrentState st, string file_name)
        {
            InitializeComponent();
            luggageCount = 0;
            beltCount = 0;
            gridSize = 15;
            airport = new Airport(nrOfFlights, carts, nrOfEmployees);
            minFlightsNr = airport.NrOfFlights / airport.GetAllCheckInGates().Count;
            nrFlightsLeft = airport.NrOfFlights % airport.GetAllCheckInGates().Count;
            vertices = new List<Belt>();
            grid = new Belt[gridSize, gridSize];
            threads = new List<Thread>();
            shortestPaths = new List<List<Belt>>();
            cartsPb = new List<ProgressBar>[3];
            SetNrOfFlights();
            PopulateListCarts();
            totalCarts = carts;
            this.totalflights = nrOfFlights;
            this.cartsPerGate = totalCarts / 3;
            luggages = new int[3];
            estimateEmployees = (nrOfFlights * 500 / 25);
            actualEmployees = nrOfEmployees;
            extraemployees = estimateEmployees - actualEmployees;

            estimateCarts = (nrOfFlights * 500 / 20);  // assuming each  cart handles 20 luggaages
            extraCarts = estimateCarts - totalCarts;

            LoadBarChartData();
            this.file_name = file_name;
            //Create Grid
            for (int r = 0; r < gridSize; r++)
            {
                for (int c = 0; c < gridSize; c++)
                {
                    Button button = new Button();
                    button.BorderThickness = new Thickness(0);
                    button.Click += BtnOnGridClick;
                    Grid.SetRow(button, r);
                    Grid.SetColumn(button, c);
                    this.gridPath.Children.Add(button);
                }
            }

            this.loadbelts = loadbelts;
            this.state = st;
            btnStopSimulation.IsEnabled = false;
            btnSaveSimulationChanges.IsEnabled = false;
            btnUndoStep.IsEnabled = false;
            btnSaveSimulation.IsEnabled = false;
            btnClearSimulation.IsEnabled = false;
            btnUndoStep.IsEnabled = false;
            this.lblPbA.Content = 0;
            this.lblPbB.Content = 0;
            this.lblPbC.Content = 0;
            //Set all gates to grid
            SetGateToGrid();

            if (state.Belts != null)
            {
                try
                {
                    loadConfig();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void LoadBarChartData()
        {
            ((BarSeries)MyChart.Series[1]).ItemsSource =
                new KeyValuePair<string, int>[] {
                     new KeyValuePair<string,int>("Shortage/Idle", actualEmployees-estimateEmployees)};
            ((BarSeries)MyChart.Series[0]).ItemsSource =
                     new KeyValuePair<string, int>[]{
                     new KeyValuePair<string,int>("Estimated ", estimateEmployees),
                     new KeyValuePair<string,int>("Engaged", actualEmployees)};
        }

        private void SetGateToGrid()
        {
            int x = 1;
            int y = 0;

            foreach (Belt g in airport.GetAllCheckInGates())
            {
                g.Name = beltCount.ToString();
                airport.startingIndexes.Add(vertices.Count);
                g.Column = y;
                g.Row = x;
                vertices.Add(g);
                beltCount++;
                grid[x, y] = g;
                canvasSimulation.Children.Add(g.getPath());
                x += 3;
            }

            x = 1;
            y = 14;
            foreach (Belt g in airport.GetAlldepartureGates())
            {
                g.Name = beltCount.ToString();
                airport.endingIndexes.Add(vertices.Count);
                g.Column = y;
                g.Row = x;
                vertices.Add(g);
                beltCount++;
                grid[x, y] = g;
                canvasSimulation.Children.Add(g.getPath());
                x += 3;
            }
        }
        public void SetNrOfFlights()
        {
            int flightsAvg = airport.NrOfFlights / airport.GetAllCheckInGates().Count;
            for (int i = 0; i < flights.Length; i++)
            {
                flights[i] = flightsAvg;
            }
            if (nrFlightsLeft == 1)
            {
                flights[0]++;
            }
            else if (nrFlightsLeft == 2)
            {
                flights[0]++;
                flights[1]++;
            }
        }
        private void PopulateListCarts()
        {
            cartsPb[0] = new List<ProgressBar>();
            cartsPb[0].Add(PbA);
            cartsPb[0].Add(PbA3);
            cartsPb[0].Add(PbA2);
            cartsPb[1] = new List<ProgressBar>();
            cartsPb[1].Add(PbB);
            cartsPb[1].Add(PbB1);
            cartsPb[1].Add(PbB2);
            cartsPb[2] = new List<ProgressBar>();
            cartsPb[2].Add(PbC);
            cartsPb[2].Add(PbC1);
            cartsPb[2].Add(PbC2);

            if (flights[0] == 1)
            {
                PbB.Visibility = Visibility.Hidden;
                PbC.Visibility = Visibility.Hidden;
            }
            else if (flights[0] == 2)
            {
                PbC.Visibility = Visibility.Hidden;
            }
            if (flights[1] == 1)
            {

                PbB1.Visibility = Visibility.Hidden;
                PbC1.Visibility = Visibility.Hidden;
            }
            else if (flights[1] == 2)
            {
                PbC1.Visibility = Visibility.Hidden;
            }
            if (flights[2] == 1)
            {

                PbB2.Visibility = Visibility.Hidden;
                PbC2.Visibility = Visibility.Hidden;
            }
            else if (flights[2] == 2)
            {
                PbC2.Visibility = Visibility.Hidden;
            }
        }
        private void BtnOnGridClick(object sender, RoutedEventArgs e)
        {
            try
            {
                btnUndoStep.IsEnabled = true;
                btnSaveSimulation.IsEnabled = true;
                btnSaveSimulationChanges.IsEnabled = true;
                btnStartSimulation.IsEnabled = true;
                btnClearSimulation.IsEnabled = true;
                btnUndoStep.IsEnabled = true;

                Button btn = sender as Button;

                //Get row and column from grid
                int row = (int)btn.GetValue(Grid.RowProperty);
                int column = (int)btn.GetValue(Grid.ColumnProperty);

                if (grid[row, column] == null)
                {

                    Label lbShowVertex = new Label();

                    //Get position/point
                    UIElement child_Element = e.Source as UIElement;
                    Point position = this.TranslatePoint(new Point(0, 0), child_Element);

                    //Create and add belt
                    Belt belt = new Belt(new Point(-position.X, -position.Y));

                    belt.Name = vertices.Count.ToString();

                    this.canvasSimulation.Children.Add(belt.getPath());

                    lbShowVertex.Content = string.Format("{0}", belt.Name);
                    Grid.SetRow(lbShowVertex, row);
                    Grid.SetColumn(lbShowVertex, column);
                    // this.gridPath.Children.Add(lbShowVertex);

                    //Add belt to grid
                    grid[row, column] = belt;
                    belt.Row = row;
                    belt.Column = column;
                    vertices.Add(belt);

                    beltCount += 1;
                }
            }

            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        public void StopSimulation()
        {
            foreach (Thread t in threads)
            {
                t.Interrupt();
            }
            setButtonsStopSimulation();
        }
        private void setButtonsStopSimulation()
        {
            btnStartSimulation.IsEnabled = true;
            btnStopSimulation.IsEnabled = false;
            btnSaveSimulation.IsEnabled = true;
            btnUndoStep.IsEnabled = false;
            btnSaveSimulationChanges.IsEnabled = true;
            slider1.IsEnabled = true;
            tbSlider.IsEnabled = true;
            btnClearSimulation.IsEnabled = true;
            btnLoadSimulation.IsEnabled = true;
        }

        private void ResetProgressBars()
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (ProgressBar pb in cartsPb[i])
                {
                    pb.Value = 0;
                }
            }
            luggages = new int[3];
            PbA3.Value = 0;
            lblPbA.Content = 0;
            lblPbB.Content = 0;
            lblPbC.Content = 0;

        }
        private void btnStartSimulationClick(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSaveSimulation.IsEnabled = false;
                btnSaveSimulationChanges.IsEnabled = false;
                lbxSimulationStatus.Items.Clear();
                lbxSimulationStatus.Items.Add("Simulation started");
                lbxSimulationStatus.Background = Brushes.Yellow;

                if (vertices.Count == 2)
                {
                    throw new MyException("Error path is to short, please make a path of at least of three belts!");
                }

                InitializeMatrix();
                FillMatrixWithBelt();

                if (!PathValidation())
                {
                    btnStartSimulation.IsEnabled = true;
                    btnStopSimulation.IsEnabled = false;
                    lbxSimulationStatus.Items.Add("Simulation stopped");
                }

                else
                {
                    slider1.IsEnabled = false;
                    tbSlider.IsEnabled = false;
                    ResetBeltColors();
                    //ResetProgressBars();
                    btnStartSimulation.IsEnabled = false;
                    btnStopSimulation.IsEnabled = true;
                    btnLoadSimulation.IsEnabled = false;
                    btnClearSimulation.IsEnabled = false;
                    btnUndoStep.IsEnabled = false;
                    for (int i = 0; i < airport.GetAllCheckInGates().Count; i++)
                    {
                        Thread pathThread = new Thread(LoopPaths);
                        Thread stopThread = new Thread(StopSimulation);
                        pathThread.SetApartmentState(ApartmentState.STA);
                        pathThread.Start(i);
                        threads.Add(pathThread);
                        Thread.Sleep(500);

                        //if there is only 25 luggages then use only one checkin gate else use more
                        //loop and call start animation based on the number of luggages
                    }

                }

            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public static void CreateSpreadsheetWorkbook(string filepath)
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

            CreateHeaderForExcelFile(sheetData);

            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }


        public static void CreateHeaderForExcelFile(SheetData sheetData)
        {
            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
            Cell cellDate = new Cell();
            CellValue vDate = new CellValue();
            cellDate.CellReference = "A1";
            vDate.Text = "Date-Time";
            cellDate.AppendChild(vDate);
            row.AppendChild(cellDate);

            Cell cellEmp = new Cell();
            CellValue vEmp = new CellValue();
            cellEmp.CellReference = "B1";
            vEmp.Text = "Employees";
            cellEmp.AppendChild(vEmp);
            row.AppendChild(cellEmp);

            Cell cellCarts = new Cell();
            CellValue vCarts = new CellValue();
            cellCarts.CellReference = "C1";
            vCarts.Text = "No of Carts";
            cellCarts.AppendChild(vCarts);
            row.AppendChild(cellCarts);

            Cell cellFlight = new Cell();
            CellValue vFlight = new CellValue();
            cellFlight.CellReference = "D1";
            vFlight.Text = "No of Flights";
            cellFlight.AppendChild(vFlight);
            row.AppendChild(cellFlight);

            Cell cellExtraEmp = new Cell();
            CellValue vExtraEmp = new CellValue();
            cellExtraEmp.CellReference = "E1";
            vExtraEmp.Text = "Extra Employees Req";
            cellExtraEmp.AppendChild(vExtraEmp);
            row.AppendChild(cellExtraEmp);

            Cell cellExtraCart = new Cell();
            CellValue vExtraCart = new CellValue();
            cellExtraCart.CellReference = "F1";
            vExtraCart.Text = " Extra Carts Required.";
            cellExtraCart.AppendChild(vExtraCart);
            row.AppendChild(cellExtraCart);

            //Append new row to sheet data.
            sheetData.AppendChild(row);
        }

        public void InsertRowIntoSheet(string filepath, Int32 Employees, Int32 Carts, Int32 Flights)

        {
            Int32 index = 0;
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.

            if (!File.Exists(filepath))
            {
                CreateSpreadsheetWorkbook(filepath);
            }

            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filepath, true, new OpenSettings());



            WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;

            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();

            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

            IEnumerable<DocumentFormat.OpenXml.Spreadsheet.Row> testrow = sheetData.Elements<DocumentFormat.OpenXml.Spreadsheet.Row>(); // Get the row IEnumerator
            index = testrow.Count();
            index++; // for the next row



            DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
            Cell cellDate = new Cell();
            CellValue vDate = new CellValue();
            cellDate.CellReference = "A" + index;
            cellDate.DataType = new EnumValue<CellValues>(CellValues.Date);
            vDate.Text = System.DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture);
            cellDate.AppendChild(vDate);
            row.AppendChild(cellDate);

            Cell cellEmp = new Cell();
            CellValue vEmp = new CellValue();
            cellEmp.CellReference = "B" + index;
            vEmp.Text = Employees.ToString();
            cellEmp.AppendChild(vEmp);
            row.AppendChild(cellEmp);

            Cell cellCarts = new Cell();
            CellValue vCarts = new CellValue();
            cellCarts.CellReference = "C" + index;
            vCarts.Text = Carts.ToString();
            cellCarts.AppendChild(vCarts);
            row.AppendChild(cellCarts);

            Cell cellFlight = new Cell();
            CellValue vFlight = new CellValue();
            cellFlight.CellReference = "D" + index;
            vFlight.Text = Flights.ToString();
            cellFlight.AppendChild(vFlight);
            row.AppendChild(cellFlight);

            Cell cellExtraEmp = new Cell();
            CellValue vExtraEmp = new CellValue();
            cellExtraEmp.CellReference = "E" + index;
            vExtraEmp.Text = (estimateEmployees - actualEmployees) > 0 ? (estimateEmployees - actualEmployees).ToString() : "Extra Emp NOT Required";
            cellExtraEmp.AppendChild(vExtraEmp);
            row.AppendChild(cellExtraEmp);

            Cell cellExtraCart = new Cell();
            CellValue vExtraCart = new CellValue();
            cellExtraCart.CellReference = "F" + index;
            vExtraCart.Text = (estimateCarts - totalCarts) > 0 ? (estimateCarts - totalCarts).ToString() : "Extra Carts NOT Required";
            cellExtraCart.AppendChild(vExtraCart);
            row.AppendChild(cellExtraCart);


            //Append new row to sheet data.

            sheetData.AppendChild(row);
            worksheetPart.Worksheet.Save();
            workbookPart.Workbook.Save();
            // Close the document.

            spreadsheetDocument.Close();
            index++;

        }
        public void LoopPaths(object index)
        {
            try
            {
                Random random = new Random();
                SolidColorBrush color;
                List<Belt> checkInGates = airport.GetAllCheckInGates();

                switch (nrFlightsLeft)
                {

                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }

                int i = (int)index;
                int endingIndex = airport.endingIndexes[i];

                switch (i)
                {
                    case 0:
                        color = Brushes.Yellow;
                        break;
                    case 1:
                        color = Brushes.DarkBlue;
                        break;
                    case 2:
                        color = Brushes.Red;
                        break;
                    default:
                        color = Brushes.Blue;
                        break;
                }

                if (shortestPaths[i] != null)
                {
                    for (int k = 0; k < flights[i] * 25; k++)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            StartAnimation(shortestPaths[i], checkInGates[i].GetCenterPoint(), i, endingIndex, color);

                        });
                       Thread.Sleep(random.Next(1000, 3000));
                    }
                }

            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ThreadInterruptedException ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lbxSimulationStatus.Items.Clear();
                    lbxSimulationStatus.Items.Add("Simulation stoped");
                    lbxSimulationStatus.Background = Brushes.White;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private bool PathValidation()
        {
            List<Belt> checkInGates = airport.GetAllCheckInGates();
            shortestPaths = new List<List<Belt>>();
            bool isValid = true;

            for (int i = 0; i < airport.GetAllCheckInGates().Count; i++)
            {
                int endingIndex = airport.endingIndexes[i];
                Dijkstra dijkstra = new Dijkstra(vertices.ToArray(), this.matrix, airport.startingIndexes[i]);
                List<Belt> shortestPath = dijkstra.GetShortestPath(vertices[Convert.ToInt32(checkInGates[i].Name)], vertices[endingIndex]);
                if (shortestPath != null)
                {
                    if (shortestPath.Count <= 2)
                    {
                        isValid = false;
                        lbxSimulationStatus.Items.Add($"Please complete path from check-in gate {i + 1} to departure gate {i + 1}");
                        lbxSimulationStatus.Background = Brushes.Red;
                    }
                    else
                    {
                        shortestPaths.Add(shortestPath);
                    }
                }
                else
                {
                    isValid = false;
                    lbxSimulationStatus.Items.Add($"Please complete path from check-in gate {i + 1} to departure gate {i + 1}");
                    lbxSimulationStatus.Background = Brushes.Red;
                }
            }
            return isValid;
        }

        private void FillMatrixWithBelt()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Belt currentBelt = grid[i, j];
                    Belt rightBelt = null;
                    Belt bottomBelt = null;
                    Belt topBelt = null;
                    Belt leftBelt = null;

                    int weight = 1;

                    if (i - 1 >= 0)
                    {
                        topBelt = grid[i - 1, j];
                    }

                    if (j - 1 >= 0)
                    {
                        leftBelt = grid[i, j - 1];
                    }

                    if (j + 1 < gridSize)
                    {
                        rightBelt = grid[i, j + 1];
                    }

                    if (i + 1 < gridSize)
                    {
                        bottomBelt = grid[i + 1, j];
                    }

                    if (currentBelt != null && topBelt != null)
                    {
                        this.matrix[Convert.ToInt32(currentBelt.Name)][Convert.ToInt32(topBelt.Name)] = weight;
                        topBelt.Parent = currentBelt;
                    }

                    if (currentBelt != null && leftBelt != null)
                    {
                        bool isCheckIn = false;
                        //foreach (Belt b in airport.GetAllCheckInGates())
                        //{
                        //    if (b == leftBelt)
                        //    {
                        //        isCheckIn = true;
                        //        break;
                        //    }
                        //}
                        if (!isCheckIn)
                        {
                            this.matrix[Convert.ToInt32(currentBelt.Name)][Convert.ToInt32(leftBelt.Name)] = weight;
                            leftBelt.Parent = currentBelt;
                        }
                    }

                    if (currentBelt != null && rightBelt != null)
                    {
                        this.matrix[Convert.ToInt32(currentBelt.Name)][Convert.ToInt32(rightBelt.Name)] = weight;
                        rightBelt.Parent = currentBelt;
                    }

                    if (currentBelt != null && bottomBelt != null)
                    {
                        this.matrix[Convert.ToInt32(currentBelt.Name)][Convert.ToInt32(bottomBelt.Name)] = weight;
                        bottomBelt.Parent = currentBelt;
                    }
                }
            }
        }

        private void StartAnimation(List<Belt> shortestPath, Point startingPoint, int startingIndex, int endingIndex, SolidColorBrush color)// startingPoint, endingPoint)
        {
            try
            {
                double startx = startingPoint.X;
                double starty = startingPoint.Y;

                // create luggage object
                Luggage luggage = new Luggage(new Point(startx, starty), luggageCount);
                luggage.GetPath().Fill = color;
                this.canvasSimulation.Children.Add(luggage.GetPath());
                NameScope.SetNameScope(this, new NameScope());
                this.RegisterName("animatedellipsegeometry", luggage.GetELlipseGeometry());

                // create the animation path.
                PathGeometry animationpath = new PathGeometry();
                PathFigure pFigure = new PathFigure();
                pFigure.StartPoint = new Point(startx, starty);

                PolyBezierSegment pBezierSegment = new PolyBezierSegment();

                foreach (Belt b in shortestPath)
                {
                    if (b != vertices[startingIndex] && b != vertices[endingIndex])
                    {
                        b.getPath().Fill = Brushes.Green;
                    }
                    else if (b == vertices[endingIndex])
                    {
                        b.getPath().Fill = color;
                    }
                    pBezierSegment.Points.Add(b.GetCenterPoint());
                    pBezierSegment.Points.Add(b.GetCenterPoint());
                    pBezierSegment.Points.Add(b.GetCenterPoint());
                    // pBezierSegment.Points.Add(b.GetEndPoint());
                }
                pBezierSegment.Points.Add(vertices[endingIndex].GetCenterPoint());
                pFigure.Segments.Add(pBezierSegment);
                animationpath.Figures.Add(pFigure);

                // freeze the pathgeometry for performance benefits.
                animationpath.Freeze();


                // create a pointanimationgusingpath to move
                // the ellipsegeometry along the animation path.
                PointAnimationUsingPath centerpointanimation = new PointAnimationUsingPath();
                centerpointanimation.PathGeometry = animationpath;
                centerpointanimation.Duration = TimeSpan.FromSeconds(slider1.Value);
                centerpointanimation.SpeedRatio = 1;
                //centerpointanimation.RepeatBehavior = RepeatBehavior.Forever;

                // set the animation to target the center property
                // of the ellipsegeometry named "animatedellipsegeometry".
                Storyboard.SetTargetName(centerpointanimation, "animatedellipsegeometry");
                Storyboard.SetTargetProperty(centerpointanimation, new PropertyPath(EllipseGeometry.CenterProperty));

                // create a storyboard to contain and apply the animation.
                Storyboard pathanimationstoryboard = new Storyboard();

                //pathanimationstoryboard.RepeatBehavior = RepeatBehavior.Forever;
                //pathanimationstoryboard.AutoReverse = true;
                pathanimationstoryboard.Children.Add(centerpointanimation);
                pathanimationstoryboard.Completed += Pathanimationstoryboard_Completed;

                void Pathanimationstoryboard_Completed(object sender, EventArgs e)
                {

                    void FillGate(ProgressBar a, ProgressBar b, ProgressBar c, Label lbl, int l)
                    {
                        void GetGate()
                        {
                            if (a.Value < 25 && a.Visibility == Visibility.Visible)
                            {
                                a.Value++;
                            }
                            else if (b.Value < 25 && b.Visibility == Visibility.Visible)
                            {
                                b.Value++;
                            }
                            else if (c.Value < 25 && c.Visibility == Visibility.Visible)
                            {
                                c.Value++;
                            }
                            else
                            {
                                a.Value = 1;
                                b.Value = 0;
                                c.Value = 0;
                            }

                        }
                        if (flights[l] * 25 - luggages[l] > 25 && (a.Value < 25 || b.Value < 25 || c.Value < 25))
                        {
                            Random rnd = new Random();
                            int gateNr = rnd.Next(0, 3);
                            switch (gateNr)
                            {
                                case 0:
                                    if (a.Value < 25 && a.Visibility == Visibility.Visible)
                                    {
                                        a.Value++;
                                    }
                                    else
                                    {
                                        GetGate();
                                    }
                                    break;
                                case 1:
                                    if (b.Value < 25 && b.Visibility == Visibility.Visible)
                                    {
                                        b.Value++;
                                    }
                                    else
                                    {
                                        GetGate();
                                    }
                                    break;
                                case 2:
                                    if (c.Value < 25 && c.Visibility == Visibility.Visible)
                                    {
                                        c.Value++;
                                    }
                                    else
                                    {
                                        GetGate();
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            /*     if (a.Value == 25)
                                 {
                                     a.Value = 0;
                                     b.Value = 0;
                                     c.Value = 0;
                                 }*/
                            // a.Value++;
                            GetGate();
                        }
                        luggages[l]++;
                        lbl.Content = luggages[l];
                        if (Convert.ToInt32(lbl.Content) % 25 == 0)
                        {
                            estimateEmployees--;
                            LoadBarChartData();
                        }

                    }
                    if (color == Brushes.Yellow)
                    {
                        FillGate(PbA, PbB, PbC, lblPbA, 0);
                    }
                    else if (color == Brushes.DarkBlue)
                    {
                        FillGate(PbA3, PbB1, PbC1, lblPbB, 1);
                    }
                    else if (color == Brushes.Red)
                    {
                        FillGate(PbA2, PbB2, PbC2, lblPbC, 2);
                    }
                    Boolean isFinished = true;

                    for (int i = 0; i < 3; i++)
                    {
                        foreach (ProgressBar b in cartsPb[i])
                        {
                            if (b.IsVisible)
                            {
                                if (b.Value != 25)
                                {
                                    isFinished = false;
                                }
                            }
                        }
                    }
                    if (isFinished)
                    {
                        StopSimulation();
                    }
                }
                Thread monitorThread = new Thread(() => CheckCriticalSection(pathanimationstoryboard, luggage, shortestPath));
                        threads.Add(monitorThread);
                        monitorThread.Start();
                        pathanimationstoryboard.Begin(luggage.GetPath(), true);
                        luggageCount++;
                    }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void CheckCriticalSection(Storyboard pathanimationstoryboard, Luggage luggage, List<Belt> shortestPath)
        {
            int beltCounter = 0;
            bool isEnteringBelt = false;
            bool isExitingBelt = false;
            bool isEnteringSucces = false;
            bool isExitingSucces = false;
            bool isFinished = false;
            Belt currentBelt = shortestPath[beltCounter];
            try
            {
                while (!isFinished)
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (beltCounter < shortestPath.Count && !isEnteringSucces)
                        {
                            isEnteringBelt = luggage.GetELlipseGeometry().Bounds.IntersectsWith(shortestPath[beltCounter].GetRectangleGeometry().Bounds);
                        }
                        if (beltCounter + 1 < shortestPath.Count && !isExitingSucces)
                        {
                            isExitingBelt = luggage.GetELlipseGeometry().Bounds.IntersectsWith(shortestPath[beltCounter + 1].GetRectangleGeometry().Bounds);
                            //isExitingBelt = luggage.GetELlipseGeometry().Bounds.Left >= (shortestPath[beltCounter + 1].GetRectangleGeometry().Bounds.Left);
                        }
                        if (isExitingSucces)
                        {
                            isFinished = luggage.GetELlipseGeometry().Bounds.IntersectsWith(shortestPath[shortestPath.Count - 1].GetRectangleGeometry().Bounds);
                        }
                    });
                    Thread.Sleep(1);
                    if (isEnteringBelt && !isFinished)
                    {
                        currentBelt.GetMonitor().EnterBelt(pathanimationstoryboard, luggage);
                        isEnteringBelt = false;
                        isEnteringSucces = true;
                        isExitingSucces = false;
                    }
                    if (isExitingBelt && !isFinished)
                    {
                        currentBelt.GetMonitor().ExitBelt();
                        isExitingBelt = false;
                        isExitingSucces = true;
                        isEnteringSucces = false;
                        beltCounter++;
                        currentBelt = shortestPath[beltCounter];
                    }
                }
            }
            catch (ThreadInterruptedException ex)
            {
                currentBelt.GetMonitor().ExitBelt();
                pathanimationstoryboard.Remove(luggage.GetPath());
                Thread.CurrentThread.Interrupt();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InitializeMatrix()
        {
            int size = beltCount;
            this.matrix = new int[size][];
            for (int i = 0; i < size; i++)
            {
                this.matrix[i] = new int[size];
                for (int j = 0; j < size; j++)
                {
                    this.matrix[i][j] = 0;
                }
            }
        }

        private void btnStopSimulationClick(object sender, RoutedEventArgs e)
        {
            StopSimulation();
        }
        private void btnSaveSimulationClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = @"C:\";
            sfd.DefaultExt = "procp";  //saving it as procp file but actually loading it as xml
            sfd.Filter = "*.procp | .procp";

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        List<Belt> nowbelts = new List<Belt>();
                        foreach (Belt b in vertices)
                        {
                            nowbelts.Add(b);
                        }
                        state.Belts = nowbelts;
                        for (int i = 0; i < this.cartsPb.Length; i++)
                        {
                            foreach (ProgressBar pb in this.cartsPb[i])
                            {
                                state.CartsValues.Add(pb.Value);
                            }
                        }
                        state.actualEmployees = actualEmployees;
                        state.estimateEmployees = estimateEmployees;
                        state.TotalCartValues = new List<double>();
                        state.TotalCartValues.Add(Convert.ToDouble(lblPbA.Content));
                        state.TotalCartValues.Add(Convert.ToDouble(lblPbB.Content));
                        state.TotalCartValues.Add(Convert.ToDouble(lblPbC.Content));
                        XmlSerializer ser = new XmlSerializer(typeof(CurrentState));
                        ser.Serialize(sw, state);
                        file_name = sfd.FileName;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong saving the file!");
                }
            }
        }

        private void btnLoadSimulationClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                ResetProgressBars();
                btnClearSimulationClick(null, null);
                var path = openFileDialog.FileName;

                XmlSerializer serializer = new XmlSerializer(typeof(CurrentState));
                StreamReader reader = new StreamReader(path);

                var input = (CurrentState)serializer.Deserialize(reader);
                reader.Close();
                btnClearSimulationClick(null, null);
                btnClearSimulation.IsEnabled = true;

                //grid = new Belt[gridSize, gridSize];
                vertices = new List<Belt>();
                foreach (Belt b in input.Belts)
                {
                    Belt b1 = new Belt(b.location);
                    b1.Name = b.Name;
                    b1.Row = b.Row;
                    b1.Column = b.Column;
                    vertices.Add(b1);
                    beltCount++;
                    canvasSimulation.Children.Add(b1.getPath());
                    grid[b1.Row, b1.Column] = b1;
                }
                lblPbA.Content = input.TotalCartValues[0];
                lblPbB.Content = input.TotalCartValues[1];
                lblPbC.Content = input.TotalCartValues[2];
                int count = 0;

                for (int i = 0; i < cartsPb.Length; i++)
                {
                    foreach (ProgressBar pb in this.cartsPb[i])
                    {
                        pb.Value = input.CartsValues[count];

                        if (count == 0 || count == 3 || count == 6)
                        {
                            lblPbA.Content = Convert.ToDouble(lblPbA.Content) + pb.Value;
                        }
                        else if (count == 1 || count == 4 || count == 7)
                        {
                            lblPbB.Content = Convert.ToDouble(lblPbB.Content) + pb.Value;
                        }
                        else if (count == 2 || count == 5 || count == 8)
                        {
                            lblPbC.Content = Convert.ToDouble(lblPbC.Content) + pb.Value;
                        }
                        count++;
                    }
                }
                luggages[0] = Convert.ToInt32(lblPbA.Content);
                luggages[1] = Convert.ToInt32(lblPbB.Content);
                luggages[2] = Convert.ToInt32(lblPbC.Content);
                estimateEmployees = input.estimateEmployees;
                actualEmployees = input.actualEmployees;
                LoadBarChartData();
            }
        }

        private void btnRemoveClick(object sender, RoutedEventArgs e)
        {
            List<Belt> belts = vertices;
            try
            {
                if (vertices.Count > 6)
                {
                    Belt b = vertices[vertices.Count - 1];
                    this.canvasSimulation.Children.Remove(b.getPath());
                    vertices.Remove(b);
                    grid[b.Row, b.Column] = null;
                    beltCount -= 1;

                    if (vertices.Count == 6)
                    {
                        btnUndoStep.IsEnabled = false;
                        btnSaveSimulation.IsEnabled = false;
                        btnLoadSimulation.IsEnabled = true;
                        btnClearSimulation.IsEnabled = false;
                        btnSaveSimulationChanges.IsEnabled = false;
                        btnStartSimulation.IsEnabled = false;
                        btnSaveSimulation.IsEnabled = false;
                    }
                }
            }
            catch (MyException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void loadConfig()
        {
            btnClearSimulation.IsEnabled = true;
            vertices = new List<Belt>();
            foreach (Belt b in loadbelts)
            {
                Belt b1 = new Belt(b.location);
                b1.Name = b.Name;
                b1.Row = b.Row;
                b1.Column = b.Column;
                vertices.Add(b1);
                beltCount++;
                canvasSimulation.Children.Add(b1.getPath());
                grid[b1.Row, b1.Column] = b1;
            }
            int count = 0;
            lblPbA.Content = state.TotalCartValues[0];
            lblPbB.Content = state.TotalCartValues[1];
            lblPbC.Content = state.TotalCartValues[2];
            for (int i = 0; i < cartsPb.Length; i++)
            {
                foreach (ProgressBar pb in this.cartsPb[i])
                {
                    pb.Value = this.state.CartsValues[count];

                    if (count == 0 || count == 3 || count == 6)
                    {
                        lblPbA.Content = Convert.ToDouble(lblPbA.Content) + pb.Value;
                    }
                    else if (count == 1 || count == 4 || count == 7)
                    {
                        lblPbB.Content = Convert.ToDouble(lblPbB.Content) + pb.Value;
                    }
                    else if (count == 2 || count == 5 || count == 8)
                    {
                        lblPbC.Content = Convert.ToDouble(lblPbC.Content) + pb.Value;
                    }
                    count++;
                }
            }
            luggages[0] = Convert.ToInt32(lblPbA.Content);
            luggages[1] = Convert.ToInt32(lblPbB.Content);
            luggages[2] = Convert.ToInt32(lblPbC.Content);
            estimateEmployees = state.estimateEmployees;
            actualEmployees = state.actualEmployees;
            LoadBarChartData();
        }

        private void btnSaveChangesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(file_name))
                {
                    List<Belt> nowbelts = new List<Belt>();
                    foreach (Belt b in vertices)
                    {
                        nowbelts.Add(b);
                    }
                    state.Belts = nowbelts;
                    state.CartsValues = new List<double>();
                    for (int i = 0; i < this.cartsPb.Length; i++)
                    {
                        foreach (ProgressBar pb in this.cartsPb[i])
                        {
                            state.CartsValues.Add(pb.Value);
                        }
                    }
                    state.actualEmployees = actualEmployees;
                    state.estimateEmployees = estimateEmployees;
                    state.TotalCartValues.Add(Convert.ToDouble(lblPbA.Content));
                    state.TotalCartValues.Add(Convert.ToDouble(lblPbB.Content));
                    state.TotalCartValues.Add(Convert.ToDouble(lblPbC.Content));
                    XmlSerializer ser = new XmlSerializer(typeof(CurrentState));
                    ser.Serialize(sw, state);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong saving the changes!");
            }
        }

        private void ResetBeltColors()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Belt b = grid[i, j];
                    bool isCheckIn = this.airport.GetAllCheckInGates().Contains(b);
                    bool isDep = this.airport.GetAlldepartureGates().Contains(b);
                    if (!isCheckIn && !isDep && b != null)
                    {
                        b.getPath().Fill = Brushes.Transparent;
                    }
                }
            }
        }

        private void btnClearSimulationClick(object sender, RoutedEventArgs e)
        {
            btnClearSimulation.IsEnabled = false;
            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    Belt b = grid[i, j];
                    bool isCheckIn = this.airport.GetAllCheckInGates().Contains(b);
                    bool isDep = this.airport.GetAlldepartureGates().Contains(b);
                    if (!isCheckIn && !isDep && b != null && canvasSimulation.Children.Contains(b.getPath()))
                    {
                        this.canvasSimulation.Children.Remove(b.getPath());
                        grid[i, j] = null;
                    }
                }
            }
            ResetProgressBars();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (dialog.FileName != null)
                {
                    InsertRowIntoSheet(dialog.FileName + "\\statistics.xlsx", actualEmployees, totalCarts, totalflights);
                }
            }
        }
    }
}
