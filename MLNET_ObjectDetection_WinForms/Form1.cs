using Microsoft.ML;
using Microsoft.ML.Transforms.Image;
using MLNET_ObjectDetection_WinForms.Models;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MLNET_ObjectDetection_WinForms
{
	public partial class Form1 : Form
    {
        public const int rowCount = 13, columnCount = 13;
        public const int featuresPerBox = 5;
        private static readonly (float x, float y)[] boxAnchors = { (0.573f, 0.677f), (1.87f, 2.06f), (3.34f, 5.47f), (7.88f, 3.53f), (9.77f, 9.17f) };
        private PredictionEngine<WineInput, WinePredictions> _predictionEngine;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] snapshotCapabilities;
        private ArrayList listCamera = new ArrayList();
        public string pathFolder = Directory.GetCurrentDirectory() + @"\sample\";   //Application.StartupPath + @"\ImageCapture\";
        private Stopwatch stopWatch = null;
        private static bool needSnapshot = false;
        Server tcpServer = new Server(IPAddress.Any, 9002);



        public Form1()
        {
            InitializeComponent();
            getListCameraUSB();
            ShowThreadInfo("Form");
            Task.Run(() => StartTCPCommunication());
            picPrediction.Visible = true;
            btnNewPrediction.Visible = false;
            btnSelectImage.Visible = false;
            
            var context = new MLContext();
            
            var emptyData = new List<WineInput>();

            var data = context.Data.LoadFromEnumerable(emptyData);

            var pipeline = context.Transforms.ResizeImages(resizing: ImageResizingEstimator.ResizingKind.Fill, outputColumnName: "data", imageWidth: ImageSettings.imageWidth, imageHeight: ImageSettings.imageHeight, inputColumnName: nameof(WineInput.Image))
                            .Append(context.Transforms.ExtractPixels(outputColumnName: "data"))
                            .Append(context.Transforms.ApplyOnnxModel(modelFile: @"C:\Users\bence\Desktop\MachineLearning\MLNET_ObjectDetection_WinForms-main\MLNET_ObjectDetection_WinForms\MLModel\model.onnx", outputColumnName: "model_outputs0", inputColumnName: "data"));
                                                                                 //modell elérési útja
            var model = pipeline.Fit(data);

            _predictionEngine = context.Model.CreatePredictionEngine<WineInput, WinePredictions>(model);
        }

        //camera rész

        private static string _usbcamera;
        public string usbcamera
        {
            get { return _usbcamera; }
            set { _usbcamera = value; }
        }
        
        private void Satart_button_Click_1(object sender, EventArgs e)
        {
            OpenCamera();
        }

        private void OpenCamera()
        {
            try
            {
                ShowThreadInfo("Camera");
                usbcamera = CameraList_comboBox.SelectedIndex.ToString();
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count != 0)
                {
                    // add all devices to combo
                    foreach (FilterInfo device in videoDevices)
                    {
                        listCamera.Add(device.Name);

                    }
                }
                else
                {
                    MessageBox.Show("Camera devices found");
                }

                videoDevice = new VideoCaptureDevice(videoDevices[Convert.ToInt32(usbcamera)].MonikerString);
                snapshotCapabilities = videoDevice.SnapshotCapabilities;
                if (snapshotCapabilities.Length == 0)
                {
                    //MessageBox.Show("Camera Capture Not supported");
                }

                OpenVideoSource(videoDevice);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        //Delegate Untuk Capture, insert database, update ke grid 
        public delegate void CaptureSnapshotManifast(Bitmap image);
        public void UpdateCaptureSnapshotManifast(Bitmap image)
        {
            try
            {
                needSnapshot = false;
                string nameImage = "sampleImage";
                string nameCapture = nameImage + ".jpg";    // "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                String predText = "";
                    var prediction = _predictionEngine.Predict(new WineInput { Image = image });

                    var labels = File.ReadAllLines(@"C:\Users\bence\Desktop\MachineLearning\MLNET_ObjectDetection_WinForms-main\MLNET_ObjectDetection_WinForms\MLModel\labels.txt");
                                    //labelek elérésí útja
                    var boundingBoxes = new List<BoundingBox>();
                    var UnCorrectedList = ParseOutputs(prediction.WineType, labels);
                    Console.WriteLine("bounding boxes:");
                    Console.WriteLine(boundingBoxes.Count);
                    var originalWidth = image.Width;
                    var originalHeight = image.Height;
                    //foreach (var boundingBox in UnCorrectedList)
                    //{
                    //    Console.WriteLine("X: " + boundingBox.Dimensions.X + " Y: " + boundingBox.Dimensions.Y + " Prediction: " + boundingBox.Label + " Confidence: " + boundingBox.Confidence);
                    //}
                    //Console.WriteLine("------------------------");
                    var removeList = new List<int>();
                    for (int i = 0; i < (UnCorrectedList.Count - 1); i++)
                    {
                        for (int j = 1; j < UnCorrectedList.Count; j++)
                        {
                            if (i != j && Math.Abs(UnCorrectedList[i].Dimensions.X - UnCorrectedList[j].Dimensions.X) <= 30)
                            {
                                //Console.WriteLine("hasonlitaas: index: " + i + ". X: " + UnCorrectedList[i].Dimensions.X + " vs index: " + j + ". X: " + UnCorrectedList[j].Dimensions.X);
                                if (UnCorrectedList[i].Confidence > UnCorrectedList[j].Confidence)
                                {
                                    //Console.WriteLine("---Kivetel: " + j);
                                    removeList.Add(j);
                                }
                                else
                                {
                                    //Console.WriteLine("---Kivetel: " + i);
                                    removeList.Add(i);
                                }
                            }
                        }
                    }

                    //foreach (var boundingBox in UnCorrectedList)
                    //{
                    //    Console.WriteLine("X: " + boundingBox.Dimensions.X + " Y: " + boundingBox.Dimensions.Y + " Prediction: " + boundingBox.Label + " Confidence: " + boundingBox.Confidence);
                    //}
                    removeList = removeList.Distinct().ToList();

                    for (int i = 0; i < UnCorrectedList.Count; i++)
                    {
                        if (!removeList.Contains(i))
                        {
                            boundingBoxes.Add(UnCorrectedList[i]);
                        }
                    }

                    if (boundingBoxes.Count < 1)
                    {
                       lbl_prediction.Text="No prediction!";
                        tcpServer.SendMessage("No prediction");
                       return;

                    }
                    boundingBoxes.Sort((x, y) => x.Dimensions.X.CompareTo(y.Dimensions.X));

                foreach (var boundingBox in boundingBoxes)
                    {
                        float x = Math.Max(boundingBox.Dimensions.X, 0);
                        float y = Math.Max(boundingBox.Dimensions.Y, 0);
                        float width = Math.Min(originalWidth - x, boundingBox.Dimensions.Width);
                        float height = Math.Min(originalHeight - y, boundingBox.Dimensions.Height);

                        // fit to current image size
                        x = originalWidth * x / ImageSettings.imageWidth;
                        y = originalHeight * y / ImageSettings.imageHeight;
                        width = originalWidth * width / ImageSettings.imageWidth;
                        height = originalHeight * height / ImageSettings.imageHeight;

                        using (var graphics = Graphics.FromImage(image))
                        {
						if (boundingBox.Label.Equals("N"))
						{
                            graphics.DrawRectangle(new Pen(Color.Green, 3), x, y, width, height);
                            graphics.DrawString(boundingBox.Description, new Font(FontFamily.Families[0], 30f), Brushes.Green, x + 5, y + 5);

                        }
                        else
						    {
                                graphics.DrawRectangle(new Pen(Color.Red, 3), x, y, width, height);
                                graphics.DrawString(boundingBox.Description, new Font(FontFamily.Families[0], 30f), Brushes.Red, x + 5, y + 5);

                            }
                        }
                    }

                    picPrediction.Image = image;
                    //image.Save(@"c:\temp\lastSample.jpg");
                    //picPrediction.Image.Save(pathFolder+"lastSample.jpg");
                    
                    foreach(var boundingBox in boundingBoxes)
				    {
                        predText+=(boundingBox.Label);     
				    }
                    lbl_prediction.Text = predText;
                    tcpServer.SendMessage(predText);
                   
                    

			}
            catch (Exception ex) 
            {   
                Console.WriteLine(ex.Message);
                //return "error";
            }

        }

        public void OpenVideoSource(IVideoSource source)
        {
            try
            {
                // set busy cursor
                this.Cursor = Cursors.WaitCursor;

                // stop current video source
                CloseCurrentVideoSource();

                // start new video source
                videoSourcePlayer1.VideoSource = source;
                videoSourcePlayer1.Start();

                // reset stop watch
                stopWatch = null;


                this.Cursor = Cursors.Default;
            }
            catch { }
        }

        private void getListCameraUSB()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count != 0)
            {
                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    CameraList_comboBox.Items.Add(device.Name);
                }
            }
            else
            {
                CameraList_comboBox.Items.Add("No DirectShow devices found");
            }

            CameraList_comboBox.SelectedIndex = 0;

        }

        public void CloseCurrentVideoSource()
        {
            try
            {

                if (videoSourcePlayer1.VideoSource != null)
                {
                    videoSourcePlayer1.SignalToStop();

                    // wait ~ 3 seconds
                    for (int i = 0; i < 30; i++)
                    {
                        if (!videoSourcePlayer1.IsRunning)
                            break;
                        System.Threading.Thread.Sleep(100);
                    }

                    if (videoSourcePlayer1.IsRunning)
                    {
                        videoSourcePlayer1.Stop();
                    }

                    videoSourcePlayer1.VideoSource = null;
                }
            }
            catch { }
        }

        private void Snapshot_button_Click(object sender, EventArgs e)
        {
            needSnapshot = true;
        }

        private void videoSourcePlayer1_NewFrame(object sender, ref Bitmap image)
        {
            try
            {
                DateTime now = DateTime.Now;
                Graphics g = Graphics.FromImage(image);

                // paint current time
                SolidBrush brush = new SolidBrush(Color.Red);
                g.DrawString(now.ToString(), this.Font, brush, new PointF(5, 5));
                brush.Dispose();
                if (needSnapshot)
                {
                    this.Invoke(new CaptureSnapshotManifast(UpdateCaptureSnapshotManifast), image);
                }
                g.Dispose();
            }
            catch
            { }

        }

        //camera end

        public static List<BoundingBox> ParseOutputs(float[] modelOutput, string[] labels, float probabilityThreshold = .5f)
        {
            var boxes = new List<BoundingBox>();

            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    for (int box = 0; box < boxAnchors.Length; box++)
                    {
                        var channel = box * (labels.Length + featuresPerBox);

                        var boundingBoxPrediction = ExtractBoundingBoxPrediction(modelOutput, row, column, channel);
                       
                        var mappedBoundingBox = MapBoundingBoxToCell(row, column, box, boundingBoxPrediction);

                        if (boundingBoxPrediction.Confidence < probabilityThreshold)
                            continue;

                        float[] classProbabilities = ExtractClassProbabilities(modelOutput, row, column, channel, boundingBoxPrediction.Confidence, labels);

                        var (topProbability, topIndex) = classProbabilities.Select((probability, index) => (Score: probability, Index: index)).Max();

                        if (topProbability < probabilityThreshold)
                            continue;

                        boxes.Add(new BoundingBox
                        {
                            Dimensions = mappedBoundingBox,
                            Confidence = topProbability,
                            Label = labels[topIndex]
                        });
                    }
                }
            }

            return boxes;
        }

        private static BoundingBoxDimensions MapBoundingBoxToCell(int row, int column, int box, BoundingBoxPrediction boxDimensions)
        {
            const float cellWidth = ImageSettings.imageWidth / columnCount;
            const float cellHeight = ImageSettings.imageHeight / rowCount;

            var mappedBox = new BoundingBoxDimensions
            {
                X = (row + Sigmoid(boxDimensions.X)) * cellWidth,
                Y = (column + Sigmoid(boxDimensions.Y)) * cellHeight,
                Width = (float)Math.Exp(boxDimensions.Width) * cellWidth * boxAnchors[box].x,
                Height = (float)Math.Exp(boxDimensions.Height) * cellHeight * boxAnchors[box].y,
            };

            // The x,y coordinates from the (mapped) bounding box prediction represent the center
            // of the bounding box. We adjust them here to represent the top left corner.
            mappedBox.X -= mappedBox.Width / 2;
            mappedBox.Y -= mappedBox.Height / 2;

            return mappedBox;
        }

        private static BoundingBoxPrediction ExtractBoundingBoxPrediction(float[] modelOutput, int row, int column, int channel)
        {
            return new BoundingBoxPrediction
            {
                X = modelOutput[GetOffset(row, column, channel++)],
                Y = modelOutput[GetOffset(row, column, channel++)],
                Width = modelOutput[GetOffset(row, column, channel++)],
                Height = modelOutput[GetOffset(row, column, channel++)],
                Confidence = Sigmoid(modelOutput[GetOffset(row, column, channel++)])
            };
        }

        public static float[] ExtractClassProbabilities(float[] modelOutput, int row, int column, int channel, float confidence, string[] labels)
        {
            var classProbabilitiesOffset = channel + featuresPerBox;
            float[] classProbabilities = new float[labels.Length];
            for (int classProbability = 0; classProbability < labels.Length; classProbability++)
                classProbabilities[classProbability] = modelOutput[GetOffset(row, column, classProbability + classProbabilitiesOffset)];
            return Softmax(classProbabilities).Select(p => p * confidence).ToArray();
        }

        private static float Sigmoid(float value)
        {
            var k = (float)Math.Exp(value);
            return k / (1.0f + k);
        }

        private static float[] Softmax(float[] classProbabilities)
        {
            var max = classProbabilities.Max();
            var exp = classProbabilities.Select(v => Math.Exp(v - max));
            var sum = exp.Sum();
            return exp.Select(v => (float)v / (float)sum).ToArray();
        }

        private void btnNewPrediction_Click(object sender, EventArgs e)
        {
            btnNewPrediction.Visible = false;
            
            btnSelectImage.Visible = false;
        }

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void fileDialog_FileOk(object sender, CancelEventArgs e)
		{

		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
            CloseCurrentVideoSource();
            tcpServer.Stop();
        }

		private void Snapshot_button_Click_1(object sender, EventArgs e)
		{
            needSnapshot = true;
        }

		private void fileWatcher_Changed(object sender, FileSystemEventArgs e)
		{

		}

		private static int GetOffset(int row, int column, int channel)
        {
            const int channelStride = rowCount * columnCount;
            return (channel * channelStride) + (column * columnCount) + row;
        }

        static void ShowThreadInfo(String s)
        {
            Console.WriteLine("{0} thread ID: {1}",s, Thread.CurrentThread.ManagedThreadId);
        }

        //Network communication begin
        private void StartTCPCommunication()
		{
            Console.WriteLine("Starting TCP server!");
            ShowThreadInfo("TCP server");
            tcpServer.Connected += TCPConnected;
            tcpServer.DataReceived += TCPDataRecieved;
            tcpServer.Start();
            Console.WriteLine("TCP server running!");
        }

        private void TCPConnected(TcpClient remote)
		{
            Console.WriteLine("Connected to ", remote.Client.RemoteEndPoint);
            //myTCPclient = remote;
        }

        private void TCPDataRecieved(TcpClient remote, string msg)
		{
            Console.WriteLine("Data Recieved");
			switch(msg){
                case "NeedSnapshot": needSnapshot = true; break;
                default: tcpServer.SendMessage("Not a valid command!"); break;
            }
		}


        //Network communication end
    }

        

    class BoundingBoxPrediction : BoundingBoxDimensions
    {
        public float Confidence { get; set; }
    }
}
