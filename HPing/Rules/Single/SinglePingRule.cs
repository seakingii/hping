using System.Net.NetworkInformation;
using System.Text;
using HPing.Utils;

namespace HPing.Rules.Single;

public class SinglePingRule {
    private string                  target;
    public event Action<PingReply>? OnPing;
    public event Action<Exception>? OnError;

    private byte[] buffer;

    private Timer? timer;

    public SinglePingRule(string target) {
        this.target = target;

        if (ArgumentManager.Current!.IsUseCustomPayload) {
            buffer = InitBufferForRootUser();
        }
        else {
            buffer = [];
        }
    }

    private byte[] InitBufferForRootUser() {
        string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        return Encoding.ASCII.GetBytes(data);
    }

    private void OnTimer(object? state) {
        PingOne();
    }

    public void Run() {
        if (timer != null) {
            timer.Dispose();
            timer = null;
        }

        timer = new Timer(OnTimer, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private void PingOne() {
        AutoResetEvent waiter = new AutoResetEvent(false);

        Ping pingSender = new Ping();

        // When the PingCompleted event is raised,
        // the PingCompletedCallback method is called.
        pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);


        // Create a buffer of 32 bytes of data to be transmitted.


        // Wait 12 seconds for a reply.
        int timeout = 12000;

        // Set options for transmission:
        // The data can go through 64 gateways or routers
        // before it is destroyed, and the data packet
        // cannot be fragmented.
        PingOptions options = new PingOptions(64, true);

        // Console.WriteLine("Time to live: {0}",   options.Ttl);
        // Console.WriteLine("Don't fragment: {0}", options.DontFragment);

        // Send the ping asynchronously.
        // Use the waiter as the user token.
        // When the callback completes, it can wake up this thread.
        pingSender.SendAsync(target, timeout, buffer, options, waiter);
    }

    private void PingCompletedCallback(object sender, PingCompletedEventArgs e) {
        // If the operation was canceled, display a message to the user.
        if (e.Cancelled) {
            //Console.WriteLine("Ping canceled.");

            // Let the main thread resume.
            // UserToken is the AutoResetEvent object that the main thread
            // is waiting for.
            ((AutoResetEvent)e.UserState).Set();
            return;
        }

        // If an error occurred, display the exception to the user.
        if (e.Error != null) {
            //Console.WriteLine ("Ping failed:");
            //Console.WriteLine (e.Error.ToString ());
            OnError?.Invoke(e.Error);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
            return;
        }

        PingReply? reply = e.Reply;

        if (reply.Status != IPStatus.Success) {
            //Console.WriteLine ("Ping failed.");
            OnError?.Invoke(new Exception(FormatStatus(reply.Status)));

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            return;
        }

        DisplayReply(reply);

        // Let the main thread resume.
        ((AutoResetEvent)e.UserState).Set();
    }

    private static string FormatStatus(IPStatus status) {
        switch (status) {
            case IPStatus.Unknown:
                return "未知";

            case IPStatus.Success:
                return "成功";
                break;
            case IPStatus.DestinationNetworkUnreachable:
                return "目标网络不可达";
            case IPStatus.DestinationHostUnreachable:
                return "目标主机不可达";
                break;
            case IPStatus.DestinationProhibited:
                return "目标主机被拒绝";
                break;
            case IPStatus.DestinationPortUnreachable:
                return "目标端口不可达";
                break;
            case IPStatus.NoResources:
                return "没有资源";
                break;
            case IPStatus.BadOption:
                return "无效的选项";
            case IPStatus.HardwareError:
                return "硬件错误";
                break;
            case IPStatus.PacketTooBig:
                return "数据包过大";
                break;
            case IPStatus.TimedOut:
                return "超时";
                break;
            case IPStatus.BadRoute:
                return "路由不可达";
                break;
            case IPStatus.TtlExpired:
                return "TTL超时";
                break;
            case IPStatus.TtlReassemblyTimeExceeded:
                return "TTL超时";
                break;
            case IPStatus.ParameterProblem:
                return "参数错误";
                break;
            case IPStatus.SourceQuench:
                return "源被限流";
                break;
            case IPStatus.BadDestination:
                return "目标不可达";
                break;
            case IPStatus.DestinationUnreachable:
                return "目标不可达";
                break;
            case IPStatus.TimeExceeded:
                return "超时";
                break;
            case IPStatus.BadHeader:
                return "无效的报头";
                break;
            case IPStatus.UnrecognizedNextHeader:
                return "未知的下一个报头";
                break;
            case IPStatus.IcmpError:
                return "ICMP错误";
                break;
            case IPStatus.DestinationScopeMismatch:
                return "目标范围不匹配";
                break;
            //default:
            //throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        return "未知";
    }

    public void DisplayReply(PingReply? reply) {
        if (reply == null)
            return;

        // Console.WriteLine ("ping status: {0}", reply.Status);
        // if (reply.Status == IPStatus.Success)
        // {
        //     Console.WriteLine ("Address: {0}",        reply.Address.ToString ());
        //     Console.WriteLine ("RoundTrip time: {0}", reply.RoundtripTime);
        //     Console.WriteLine ("Time to live: {0}",   reply.Options.Ttl);
        //     Console.WriteLine ("Don't fragment: {0}", reply.Options.DontFragment);
        //     Console.WriteLine ("Buffer size: {0}",    reply.Buffer.Length);
        // }
        OnPing?.Invoke(reply);
    }
}