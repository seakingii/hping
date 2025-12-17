namespace HPing.Utils;

public class Help {
    public static void PrintHelp() {
        Console.WriteLine("hping 帮助");
        Console.WriteLine("使用方法: hping [options] [target]");
        Console.WriteLine("参数:");
        Console.WriteLine("  <target>                 目标地址，可以是域名或者IP地址。如果没有指定，将会使用默认地址 1.1.1.1.也可以用环境变量 hping_default_target 来指定默认目标");
        Console.WriteLine("选项:");
        Console.WriteLine("  -h, --help              打印帮助信息");
        Console.WriteLine("  -v, --verbose           冗长输出");
        Console.WriteLine("  -c, --count <count>     指定ping的次数");
        Console.WriteLine("  -i, --interval <ms>     指定ping的间隔时间");
        Console.WriteLine("  -t, --timeout <ms>      指定ping的超时时间");
        Console.WriteLine("  -s, --size <size>        指定ping的包大小");
        Console.WriteLine("  -p, --pattern <pattern>  指定ping的包内容");
        Console.WriteLine("  -w, --wait <ms>          指定ping的等待时间");
        Console.WriteLine("  -r, --repeat <count>     指定ping的循环次数");
        Console.WriteLine("  -d, --delay <ms>         延迟指定时间后开始ping");
        Console.WriteLine("  -a, --address <address>  指定ping的目标地址");
        Console.WriteLine("  -n, --name <name>        指定ping的目标名称");
        Console.WriteLine("  -l, --list <list>        指定ping的目标列表");
        Console.WriteLine("  -f, --file <file>        指定ping的目标文件");
        Console.WriteLine("  -o, --output <file>      指定ping的结果输出文件");
        Console.WriteLine("  -y, --yaml <file>        指定ping的yaml文件");
        Console.WriteLine("  -j, --json <file>        指定ping的json文件");
        Console.WriteLine("  -x, --xml <file>         指定ping的xml文件");
    }
}