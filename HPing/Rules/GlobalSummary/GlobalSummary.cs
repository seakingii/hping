using System.Net.NetworkInformation;

namespace HPing.Rules;

/// <summary>
/// 全局统计
/// </summary>
public static class GlobalSummary {
    
    
    public static GlobalSummaryResult Result { get; } = new GlobalSummaryResult();

    private static bool isFirst = true;
    
    public static void Init() {

        isFirst = true;
        
        Result.StartTime = DateTime.Now;
        Result.MaxTimeMS = 0;
        Result.MinTimeMS = 0;
        Result.AvgTimeMS = 0;
        Result.TotalCount = 0;
        Result.SuccessCount = 0;
        Result.FailCount = 0;
    }
    
    public static void Add(PingReply reply) { 
        
        Result.TotalCount++;
        Result.TotalTimeMS += reply.RoundtripTime;
        Result.SuccessCount++;
        
        if (isFirst) {
            Result.MinTimeMS = reply.RoundtripTime;
            Result.MaxTimeMS = reply.RoundtripTime;
            isFirst = false;
        }
        else {
            Result.MinTimeMS = Math.Min(Result.MinTimeMS, reply.RoundtripTime);
            Result.MaxTimeMS = Math.Max(Result.MaxTimeMS, reply.RoundtripTime);
            Result.AvgTimeMS = Result.TotalTimeMS / Result.SuccessCount;
        }
        
       
        
    }
    
    public static void Fail() {
        Result.TotalCount++;
        Result.FailCount++;
    }
    
    
    
    



    #region class

    public class GlobalSummaryResult {
        public int  TotalCount   { get; set; }
        public int  SuccessCount { get; set; }
        public int  FailCount    { get; set; }
        public long TotalTimeMS  { get; set; }
        public long MinTimeMS    { get; set; }
        public long MaxTimeMS    { get; set; }
        public long AvgTimeMS    { get; set; }
        public DateTime StartTime { get; set; }
        
    }

    #endregion
    
}