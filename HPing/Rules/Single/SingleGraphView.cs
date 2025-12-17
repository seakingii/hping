using System.Drawing;
using System.Globalization;
using System.Text;
using HPing.Utils;
using Juyi.Math;
using Juyi.Types;
using Terminal.Gui.Views;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Attribute = Terminal.Gui.Drawing.Attribute;
using Color = Terminal.Gui.Drawing.Color;

namespace HPing.Rules.Single;

public class SingleGraphView {

    private readonly GraphView                   graphView;
    private readonly Attribute                   cyan          = new Attribute (Color.BrightCyan,    Color.Black);
    private          List<PointData>             pointsForUI   = new ();
    private          FixedSizeLinkedList<double> pointsForStat = new FixedSizeLinkedList<double>(60 * 60 * 3);//缓存10000多个数据用来统计,大概3小时
    
    // Attribute         magenta = new Attribute (Color.BrightMagenta, Color.Black);
    // Attribute         red     = new Attribute (Color.BrightRed,     Color.Black);
    
    public SingleGraphView(GraphView graphView) {
        this.graphView = graphView;
        Init();
    }

    private void Init() {
        this.graphView.Title = "趋势";
        graphView.Reset ();
        graphView.Title = "";

        var black = new Attribute (graphView.GetAttributeForRole (VisualRole.Normal).Foreground, Color.Black, graphView.GetAttributeForRole (VisualRole.Normal).Style);
       
        graphView.GraphColor = black;

        // How much graph space each cell of the console depicts
        graphView.CellSize = new (2, 10);

        // leave space for axis labels
        graphView.MarginBottom = 3;
        graphView.MarginLeft = 5;

        // One axis tick/label per
        graphView.AxisX.Increment       = 20;
        graphView.AxisX.ShowLabelsEvery = 1;
        graphView.AxisX.Text            = "";//X →
        //_graphView.AxisX.Visible         = false;
        graphView.AxisX.LabelGetter = render => {
                                           var p = pointsForUI.FirstOrDefault(v => v.Point.X == render.Value);
                                           return p != null ? p.Time.ToString("mm:ss") : "";
                                       };

        graphView.AxisY.Increment = 20;
        graphView.AxisY.ShowLabelsEvery = 1;
        graphView.AxisY.Text = "ms";
        graphView.SetNeedsDraw ();
    }

    /// <summary>
    /// 外部更新,新的 ping 数据
    /// </summary>
    /// <param name="value">当前PING要用多少 ms </param>
    public void Update(long  value) {
        
        pointsForStat.Add(value);
        
        // 当前界面,并不显示从刚开始PING到现在的所有数据,而是只显示当前页面显示的下的部分数据
        var maxCount = this.graphView.Viewport.Width / this.graphView.CellSize.X;
        if (pointsForUI.Count >= maxCount) {
            pointsForUI = pointsForUI[1..];
        }

        pointsForUI.Add (new PointData() {
                                        Point =  new PointF(0, value),
                                        Time = DateTime.Now,
                                    });

        for (var i = 0; i < pointsForUI.Count; i++) {
            pointsForUI[i].Point = new PointF(i * 5 + 10, pointsForUI[i].Point.Y);
        }
        
        
        Refresh();
    } 
    
    

    /// <summary>
    /// 内部刷新界面
    /// </summary>
    private void Refresh() {

        var pp = this.pointsForUI.Select(v => v.Point).ToList();
       
        var items = new ScatterSeries { Points = pp };

        var line = new PathAnnotation
                   {
                       LineColor = cyan, Points = pp, BeforeSeries = true
                   };

        graphView.Series.Clear();
        graphView.Annotations.Clear();

        
        var summary = GlobalSummary.Result;
        var span    = DateTime.Now - summary.StartTime;
        
        var yCellSize = ComputeYCellSize(pp);
        
        graphView.CellSize = new (2, yCellSize);

        //var    pointsForCalc     = pointsForUI.Select(v => Convert.ToDouble(v.Point.Y)).ToList();
        var    pointsForCalc     = pointsForStat.Items;
        double median            = 0; //中位数
        double standardDeviation = 0; //标准差
        double mode              = 0; //众数
        if (pointsForUI.Count > 0) {
            var calcResult    = Juyi.Math.StatisticsCalculator.Calculate(new StatisticsCalculator.Config() {
                                                                                                               IsMedian            = true,
                                                                                                               IsStandardDeviation = true
                                                                                                           }, pointsForCalc);
            median = calcResult.Median;
            standardDeviation = calcResult.StandardDeviation;

            mode = Juyi.Math.StatisticsCalculator.Mode(pointsForCalc).Modes.First();
        }
        
        graphView.AxisX.Minimum = pointsForUI.Count > 0 ? pointsForUI.First().Point.X : 0;
        graphView.AxisX.Text    = $"用时 {span.Hours:00}:{span.Minutes:00}:{span.Seconds:00} , 成功 {summary.SuccessCount}, 失败 {summary.FailCount},丢包 {(summary.TotalCount - summary.SuccessCount -0.0m)/summary.TotalCount * 100:F1}%, 平均 {summary.AvgTimeMS}ms,  最快 {summary.MinTimeMS}ms, 最慢 {summary.MaxTimeMS}ms , 中位 {median}ms, 众数 {mode}ms, 标准差 {standardDeviation:F2}";
        
        graphView.Series.Add (items);
        graphView.Annotations.Add (line);


       
        
        graphView.SetNeedsDraw ();
    }
    
    /// <summary>
    /// 动态计算Y轴单元格大小
    /// </summary>
    /// <returns></returns>
    private float ComputeYCellSize(List<PointF> pp) {
        var max = pp.Max (p => p.Y)  ; 
        if (max == 0) {
            max = 100f;
        }

        var cellSize = 10f;
        var inc      = 10f; // 刻度间隔
        
        if (max <= 10) {
            cellSize = 1;
            inc      = 1;
        }else if (max <= 100) {
            cellSize = 10;
            inc      = 10;
        }
        // else if (cellSize <= 15) {   //如果不能被10整除,会出现Y轴刻度位置错乱
        //     cellSize = 15;
        // }
        else if (cellSize <= 200) {
            cellSize = 20;
            inc = 20;
        }
        else if (cellSize <= 500) {
            cellSize = 50;
            inc = 50;
        }
        else if (cellSize <= 1000) {
            cellSize = 100;
            inc = 100;
        }

        graphView.AxisY.Increment = inc;
        
        
        return cellSize;
    }


    internal class PointData {
        public PointF Point;
        public DateTime Time;
    }

}