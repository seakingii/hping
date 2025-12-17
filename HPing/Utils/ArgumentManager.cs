using CommandLine;

namespace HPing.Utils;

/// <summary>
/// 负责管理用户输入的参数
/// </summary>
public class ArgumentManager {


    public static ArgumentManager? Current;
    private ArgumentOptions options;
    
    private const string DEFAULT_TARGET     = "1.1.1.1";

    public ArgumentManager(string[] args) {
        var parserResult = Parser.Default.ParseArguments<ArgumentOptions>(args);
        Current = this;
        
        if (parserResult == null) {
            options = CreateDefaultOptions();
        }
        else {
            options        = parserResult.Value;
            options.Target = string.IsNullOrEmpty(options.Target) ? GetTargetFromEnvOrDefault() : options.Target;
        }

        IsUseCustomPayload = CheckUseCustomPayload();

    }

    /// <summary>
    /// 是否使用自定义数据包
    /// </summary>
    public bool IsUseCustomPayload { get; } = false;

    private bool CheckUseCustomPayload() {
        // 在 Linux 下,如果不是root用户，那么有在ping发送自定义数据,会有权限问题
        if (Juyi.OS.OperatingSystem.IsLinux) {
            if (Juyi.OS.OperatingSystem.IsRunningWithSudo()) {
                return true;
            }

            return false;
        }
        // windows 下好像不需要root 权限
        if (Juyi.OS.OperatingSystem.IsWindows) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 创建缺省的参数
    /// </summary>
    /// <returns></returns>
    private ArgumentOptions CreateDefaultOptions() {
        
        var target = GetTargetFromEnvOrDefault();
        
        return new ArgumentOptions() {
            Target = target
        };
    }

    /// <summary>
    /// 尝试从环境变量中获取目标地址，如果没有则使用默认值
    /// </summary>
    /// <returns></returns>
    private static string GetTargetFromEnvOrDefault() {
        var targetFromEnv = Environment.GetEnvironmentVariable("ping_default_target");
        var target        = !string.IsNullOrEmpty(targetFromEnv) ? targetFromEnv : DEFAULT_TARGET;
        return target;
    }
    
    public bool HasHelpArgument() {
        return options.Help;
    }

    public string Target => options.Target;
}

public class ArgumentOptions
{
    [Option('h', "help", Required = false, HelpText = "打印帮助文本")]
    public bool Help { get; set; }
    
    [Value(0, 
              MetaName = "MetaName",
              Required = false, 
              Default = "1.1.1.1",
              HelpText = "要ping的目标地址")]
    public string Target { get; set; }
}