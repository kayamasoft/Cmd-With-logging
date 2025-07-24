using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;
using System.Configuration;
using System.Text;

class Program
{
    public static string ver = "3.1";
    public static bool UserCancel = false;
    public static bool cmdstatus = false;
    public static bool UserCancelok = true;
    public static bool sodoflg = false;
    public static bool sodoflg2 = false;
    public static bool cmdwriteflg = false;
    public static bool KSrunas = false;
    public static string username = Environment.UserName;
    public static string CmdDir = @"C:\";
    public static string command = "echo test";
    public static string LogFile = @"C:\cmd_log.txt";
    public static string cmdline = "";

	[STAThread]
    static void Main(string[] args)
    {   
        //Console.WriteLine("→"+args.Length);//引数

        if (args.Length < 2){
            //オープニング
            opening();

            //ログファイル指定
            LogFile = BrowseFolder();

            // Ctrl+C の処理を追加
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKeyPressHandler);

            //ログに日付を記載
            File.AppendAllText(LogFile, "Cmd with logging v"+ver+"\r\n");
            File.AppendAllText(LogFile, timezonename()+"\r\n\r\n");

            //ようこそ画面
            Console.Clear();
        }

        //引数にファイルが指定されている
        if (args.Any())
        {

                if (args[0] == "KSrunas"){//sudoで実行
                    CmdDir = args[2];
                    command = args[1];
                    LogFile = args[3];
                    KSrunas = true;
                    Console.WriteLine(CmdDir+"># "+command);

                }else if (args[0] == "KSrunas2"){
                    CmdDir = args[1];
                    LogFile = args[2];

                } else if (args[0] == "mun") {
                    command = args[1];
                    LogFile = args.Length > 2 ? args[2] : @"C:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\cmd_tmp\"+logdatetime2()+"_cmd-with-logging.txt";;
                    CmdDir = args.Length > 3 ? args[3] : @"C:\Windows\System32";
                    KSrunas = true;
                    Console.WriteLine("Welcome to Cmd with logging v"+ver);
                    Console.WriteLine("Log File > "+LogFile);
                    Console.WriteLine("ログの保存先を変更する時は\"log change\"と入力してください。詳細は\"log help\"をご確認ください。");
                    Console.WriteLine("");
                    Console.WriteLine(CmdDir+"> "+command);
                    File.AppendAllText(LogFile, "Cmd with logging v"+ver+"\r\n");
                    File.AppendAllText(LogFile, timezonename()+"\r\n\r\n");
                    File.AppendAllText(LogFile,  "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+"\r\n");

                } else { //ファイルをクリックして実行
                string filePath = args[0];
                string tmpdatetime = logdatetime2();
                string tmppath = @"C:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\bat_tmp\"+tmpdatetime+"_"+System.IO.Path.GetFileNameWithoutExtension(filePath)+".bat";
                string path = @"C:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\bat_tmp";
                if (Directory.Exists(path)) {
                }
                else{
                    System.IO.DirectoryInfo di =
                        System.IO.Directory.CreateDirectory(path);
                }
                File.Copy(filePath, tmppath, true);
                UserCancelok = false;
                ExecuteCommand("\""+tmppath+"\"",@"C:\Windows\System32",LogFile);        
            }
        }
        else
        {
            Console.WriteLine("Welcome to Cmd with logging v"+ver);
            Console.WriteLine("Log File > "+LogFile);
            Console.WriteLine("ログの保存先を変更する時は\"log change\"と入力してください。詳細は\"log help\"をご確認ください。");
            Console.WriteLine("");


            //カレントディレクトリ取得
            CmdDir = System.IO.Directory.GetCurrentDirectory();
            File.AppendAllText(LogFile,  "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+"\r\n");
        }

        //コマンド受付
        while (true)
        {
            cmdstatus = false;//cancelフラグ
            sodoflg = false;//sudoフラグ
            sodoflg2 = false;//sudo su フラグ
            cmdwriteflg = false;//コマンド記載フラグ

            if(IsAdministrator()){
                cmdline = "#";
            }else{
                cmdline = "";
            }

            if (KSrunas){
                KSrunas = false;
                goto sudoKSrunas;
            }
            
            Console.Title = cmdline+"Cmd with logging v"+ver+"  "+CmdDir;
            Console.Write(CmdDir+">"+cmdline);
            command = Console.ReadLine();

            //未入力なら最初から
            if (string.IsNullOrEmpty(command))
            {
                File.AppendAllText(LogFile,  "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+"\r\n");
                continue;
            }

            //sudoコマンド
            if (command.StartsWith("sudo su"))
            {   
                sodoflg2 = true;
                cmdwriteflg = true;
                File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+command+"\r\n");
                if (command.Substring(2) == ""){
                    command = "KSrunas2";
                }else{
                    command = command.Substring(3);
                }
            }

            //sudoコマンド
            if (command.StartsWith("sudo"))
            {   
                sodoflg = true;
                cmdwriteflg = true;
                File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+command+"\r\n");
                if (command.Substring(2) == ""){
                    command = "KSrunas";
                }else{
                    command = command.Substring(5);
                }
            }

            //cls
            if (command.ToLower() == "cls")
            {
                Console.Clear();
                File.AppendAllText(LogFile,  "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+"cls\r\n");
                continue;
            }

            //exitは終了
            if (command.ToLower() == "exit")
            {
                File.AppendAllText(LogFile,  "["+logdatetime()+"]"+cmdline+" exit\r\n");
                break;
            }

            //ログコマンド
            if (command.ToLower() == "log bat history")
            {   
                cmdwriteflg = true;
                File.AppendAllText(LogFile,  "["+logdatetime()+"]"+cmdline+" log bat history\r\n");
                command = @"dir /b /a-d C:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\bat_tmp";
                File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" バッチファイルの実行履歴\r\n");
                Console.WriteLine("バッチファイルの実行履歴");
            }

            //コマンド記載
            if (!cmdwriteflg)
            {
                File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+command+"\r\n");
            }

            //ログコマンド
            if (command.ToLower() == "log change")
            {   
                string oldLogFile = LogFile;
                LogFile = BrowseFolder();
                File.AppendAllText(LogFile, "Cmd with logging v"+ver+"\r\n");
                File.AppendAllText(LogFile, timezonename()+"\r\n");
                File.AppendAllText(LogFile, "このログは次のログの続きです。:"+oldLogFile+"\r\n\r\n");
                Console.WriteLine("Welcome to Cmd with logging v"+ver);
                Console.WriteLine("Log File > "+LogFile);
                Console.WriteLine("ログの保存先を変更する場合は\"log change\"と入力してください。");
                Console.WriteLine("");
                continue;
            }
            if (command.ToLower() == "log clear")
            {   
                // FileInfoのインスタンスを生成する
                FileInfo fileInfo = new FileInfo(LogFile);
                // ファイルを削除する
                fileInfo.Delete();
                File.AppendAllText(LogFile, "Cmd with logging v"+ver+"\r\n\r\n");
                Console.WriteLine("ログファイルをクリアしました。");
                Console.WriteLine("");
                continue;
            }
            if (command.ToLower() == "log bat run")
            {   
                Console.Write("実行するファイルを入力してください。"+">"+cmdline);
                string runfile = Console.ReadLine();
                string runfilepath = @"C:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\bat_tmp\"+runfile;
                File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" バッチファイルの実行\r\n");
                Console.WriteLine(cmdline+"バッチファイルの実行");
                File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" 次を実行します："+runfile+"\r\n");
                Console.WriteLine(cmdline+"次を実行します："+runfile);
                ExecuteCommand(runfilepath,@"C:\Windows\System32",LogFile);
                continue;
            }
            if (command.ToLower() == "log ver")
            {   
                File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+"log ver\r\n");
                Console.WriteLine("");
                Console.WriteLine("");
                verinfo(LogFile);
                continue;
            }
            if (command.ToLower() == "log help")
            {   
                File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+CmdDir+">"+cmdline+"log help\r\n");
                help(LogFile);
                cominfo(LogFile);
                continue;
            }
            if (command.ToLower() == "log logo")
            {   
                opening();
                continue;
            }
            if (command.StartsWith("log"))
            {   
                cominfo(LogFile);
                continue;
            }

            //cdならディレクトリ移動
            if (command.StartsWith("cd "))
            {   

                if (command.ToLower() == "cd ..")//..の場合
                {
                    CmdDir = CmdDir.Substring( 0, CmdDir.LastIndexOf( @"\" ));
                    Console.WriteLine("");
                    File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" \r\n");
                    continue;
                }
                else if (Directory.Exists(command.Substring(3)))//絶対パス
                {
                    CmdDir = command.Substring(3);
                    continue;
                }
                else if (Directory.Exists(CmdDir+@"\"+command.Substring(3)))//相対パス
                {
                    CmdDir = CmdDir+@"\"+command.Substring(3);
                    continue;
                }

                else
                {
                    Console.WriteLine(cmdline+"指定されたパスが見つかりません。");
                    Console.WriteLine("");
                    File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+"指定されたパスが見つかりません。\r\n");
                    File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" \r\n");
                    continue;
                }

            };

        sudoKSrunas:
            //コマンド実行
            UserCancelok = false;
            if (sodoflg)
            {
                //管理者権限で実行
                Runsudo(command,CmdDir,LogFile);
            }
            else if (sodoflg2)
            {
                //管理者権限で実行
                Runsudo2(command,CmdDir,LogFile);
            }
            else
            {
                //通常権限で実行
                ExecuteCommand(command,CmdDir,LogFile);
            }
            //実行完了
            UserCancelok = true;
        }

    }

    //Ctrl+C押下時の処理
    static void CancelKeyPressHandler(object sender, ConsoleCancelEventArgs args)
    {
        if (UserCancelok)
        {
            args.Cancel = false;
            Application.Exit();
        }
        else
        {
            args.Cancel = true;
            UserCancel = true;
        }

    }

    //保存先を指定
    static string BrowseFolder()
    {
        using (var ofd = new OpenFileDialog() { Title = "Cmd with logging v"+ver+"  "+"ログを保存先を選択してください。", FileName = "CWL_"+logdate(), Filter = "Cmd with logging|*.txt", CheckFileExists = false })
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                return ofd.FileName;
            }
            else
            {
                Application.Exit();

            }
        }
        Application.Exit();

        if (Directory.Exists(@"C:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\cmd_tmp\")) {
        }
        else {
            System.IO.DirectoryInfo di =
                System.IO.Directory.CreateDirectory(@"C:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\cmd_tmp\");
        }

        return @"C:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\cmd_tmp\"+logdatetime2()+"_cmd-with-logging.txt";
    }

    //ログファイル名を指定
    static string logdate()
    {
        //DateTime 構造体を用意
        DateTime objDateTime = DateTime.Now;

        //書式設定してstring型変数に代入
        string strDateTime = objDateTime.ToString("yyyyMMdd");
        
        //返す
        return strDateTime;
    }

    //Datetimeを取得
    static string logdatetime()
    {
        //返す
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    static string timezonename()
    {
        System.TimeZoneInfo timeZone = System.TimeZoneInfo.Local;

        //返す
        return "TimeZone="+timeZone.BaseUtcOffset+"("+timeZone.Id+")";
    }

    //Datetimeを取得
    static string logdatetime2()
    {
        //返す
        return DateTime.Now.ToString("yyyyMMdd-HHmmss");
    }

    //コマンドを実行
    static void ExecuteCommand(string command,string CmdDir,string LogFile)
    {
        try {
        var processStartInfo = new ProcessStartInfo("cmd", "/c " + "cd "+CmdDir+" && "+command)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process
        {
            StartInfo = processStartInfo,
        };

        process.OutputDataReceived += (sender, args) =>
        {
            cmdstatus = true;
            Console.WriteLine(args.Data);
            File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+args.Data + Environment.NewLine);
            if (UserCancel == true)
            {
                Console.WriteLine("Ctrl+C");
                File.AppendAllText(LogFile, "["+logdatetime()+"] "+">"+cmdline+"Ctrl+C\r\n");
                if (!process.HasExited) // プロセスがまだ実行中であることを確認
                {
                    KillProcessTree(process.Id); // プロセスツリー全体を終了
                }
                UserCancel = false;
                return;
            }
            
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            if (cmdstatus)
            {
                return;
            }
            Console.WriteLine(args.Data);
            File.AppendAllText(LogFile, "[" + logdatetime() + "]"+cmdline+" " + args.Data + Environment.NewLine);

            process.CancelOutputRead();
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        } catch {
            Console.WriteLine("cwl execution error");
            Console.WriteLine("");
            File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" cwl execution error\r\n");
            File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" \r\n");
        }

    }

    //プロセスツリー全体を終了
    static void KillProcessTree(int pid)
    {
        var psi = new ProcessStartInfo("taskkill", "/F /T /PID " + pid)
        {
            CreateNoWindow = true,
            UseShellExecute = false
        };
        Process.Start(psi);
    }
    //verコマンド
    static void verinfo(string LogFile)
    {
        string vermsg = "                                Cmd with logging v"+ver+@"

                Copyright (c) Cmd with logging , 2025 KayamaSoft. Co., Ltd. All rights reserved.
		Licensed under the Apache License, Version 2.0

                            Contact us : hello@kayamasoft.org
			    https://www.apache.org/licenses/LICENSE-2.0

";
        File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+vermsg);
        Console.WriteLine(vermsg);
    }

    //ヘルプコマンド
    static void help(string LogFile)
    {
        string helpmsg = @"ようこそCmd with loggingへ!

このツールはコマンドプロンプトのログを取得できるツールです。

●このツールはコマンドプロンプトと高い互換性がありますが、一部コマンドは正常に動作しません。
##サポートしていないコマンド##
pauseコマンド
対話型のコマンド(diskpartなど)
titleコマンド
echoコマンド(echoはON固定です)
cdコマンド(cd ../..のように連続して..を使用する場合)

●バッチファイル実行方法
  任意のバッチファイルにこのツールを関連付け実行することができます。
  .batのままでは強制的にコマンドプロンプトで実行されるため、.cwlなどの任意の拡張子を使用してください。
  バッチファイル実行時はバッチファイルに記載のコマンドが、このツールでサポートされているかご確認ください。

●このツールには下記の専用コマンドが用意されています。";
        File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+helpmsg);
        Console.WriteLine(helpmsg);
    }
    static void cominfo(string LogFile)
    {
string cmdmsg = @"
logコマンドのオプション:

log change
  記録しているログファイルを変更する際に使用します。

log clear
  記録しているログファイルを削除し、新たに記録を開始します。

log bat history
  これまでにファイルから実行したバッチファイルの履歴を確認できます。
  これまでに実行したバッチファイルはC:\Users\"+username+@"\AppData\Local\KayamaSoft\Cwl\bat_tmpに保存されています。

log bat run
  これまでファイルから実行したバッチファイルを再度実行します。

log help
  ヘルプを確認できます。

log ver
  このツールのバージョンを確認できます。

log logo
  このツールのロゴマークを確認できます。

sudo <command>
  コマンドを管理者権限で実行することができます。

sudo su
  コマンドを管理者権限で実行することができます。
";
        File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+cmdmsg);
        Console.WriteLine(cmdmsg);

    }

    //起動時の処理
    static void opening()
    {
        //オープニング
        string opening = @"                                                                                
  /$$$$$$                      /$$       /$$      /$$ /$$   /$$     /$$         
 /$$__  $$                    | $$      | $$  /$ | $$|__/  | $$    | $$         
| $$  \__/ /$$$$$$/$$$$   /$$$$$$$      | $$ /$$$| $$ /$$ /$$$$$$  | $$$$$$$    
| $$      | $$_  $$_  $$ /$$__  $$      | $$/$$ $$ $$| $$|_  $$_/  | $$__  $$   
| $$      | $$ \ $$ \ $$| $$  | $$      | $$$$_  $$$$| $$  | $$    | $$  \ $$   
| $$    $$| $$ | $$ | $$| $$  | $$      | $$$/ \  $$$| $$  | $$ /$$| $$  | $$   
|  $$$$$$/| $$ | $$ | $$|  $$$$$$$      | $$/   \  $$| $$  |  $$$$/| $$  | $$   
 \______/ |__/ |__/ |__/ \_______/      |__/     \__/|__/   \___/  |__/  |__/   
                                                                                
                                                                                
           /$$                               /$$                                
          | $$                              |__/                                
          | $$  /$$$$$$   /$$$$$$   /$$$$$$  /$$ /$$$$$$$   /$$$$$$             
          | $$ /$$__  $$ /$$__  $$ /$$__  $$| $$| $$__  $$ /$$__  $$            
          | $$| $$  \ $$| $$  \ $$| $$  \ $$| $$| $$  \ $$| $$  \ $$            
          | $$| $$  | $$| $$  | $$| $$  | $$| $$| $$  | $$| $$  | $$            
          | $$|  $$$$$$/|  $$$$$$$|  $$$$$$$| $$| $$  | $$|  $$$$$$$            
          |__/ \______/  \____  $$ \____  $$|__/|__/  |__/ \____  $$            
                         /$$  \ $$ /$$  \ $$               /$$  \ $$            
                        |  $$$$$$/|  $$$$$$/              |  $$$$$$/            
                         \______/  \______/                \______/             
                                                                                
";

        Console.Write(opening);
    }

    static void Runsudo(string command, string CmdDir, string LogFile)
    {
        //管理者として自分自身を起動する
        System.Diagnostics.ProcessStartInfo psi =
            new System.Diagnostics.ProcessStartInfo();

        psi.UseShellExecute = true;

        psi.FileName = Application.ExecutablePath;
        
        psi.Verb = "runas";
        //引数を設定する
        psi.Arguments = "KSrunas \""+command+"\" \""+CmdDir+"\" \""+LogFile+"\" \"";

        try
        {
            //起動する
            System.Diagnostics.Process.Start(psi);
            Environment.Exit(0);
        }
        catch (System.ComponentModel.Win32Exception)// ex
        {
            Console.WriteLine("Permission denied");
            Console.WriteLine("");
            File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+"Permission denied\r\n");
            File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" \r\n");
        }
    }

    static void Runsudo2(string command, string CmdDir, string LogFile)
    {
        //管理者として自分自身を起動する
        System.Diagnostics.ProcessStartInfo psi =
            new System.Diagnostics.ProcessStartInfo();

        psi.UseShellExecute = true;

        psi.FileName = Application.ExecutablePath;
        
        psi.Verb = "runas";
        //引数を設定する
        psi.Arguments = "KSrunas2 \""+CmdDir+"\" \""+LogFile+"\" \"";

        try
        {
            //起動する
            System.Diagnostics.Process.Start(psi);
            Environment.Exit(0);
        }
        catch (System.ComponentModel.Win32Exception)// ex
        {
            Console.WriteLine("Permission denied");
            Console.WriteLine("");
            File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" "+"Permission denied\r\n");
            File.AppendAllText(LogFile, "["+logdatetime()+"]"+cmdline+" \r\n");
        }
    }

    public static bool IsAdministrator()
    {
        //現在のユーザーを表すWindowsIdentityオブジェクトを取得する
        System.Security.Principal.WindowsIdentity wi =
            System.Security.Principal.WindowsIdentity.GetCurrent();
        //WindowsPrincipalオブジェクトを作成する
        System.Security.Principal.WindowsPrincipal wp =
            new System.Security.Principal.WindowsPrincipal(wi);
        //Administratorsグループに属しているか調べる
        return wp.IsInRole(
            System.Security.Principal.WindowsBuiltInRole.Administrator);
    }

}
