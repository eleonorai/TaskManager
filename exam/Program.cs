using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

public class ProcessManager
{
    private static readonly Mutex _mutex = new Mutex(false, "ProcessManagerMutex");

    public static List<Process> GetAllProcesses()
    {
        _mutex.WaitOne();
        try
        {
            return Process.GetProcesses().ToList();
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    public static void KillProcess(int pid)
    {
        _mutex.WaitOne();
        try
        {
            Process process = Process.GetProcessById(pid);
            process.Kill();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при завершенні процесу з PID {pid}: {ex.Message}");
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    public static void StartProcess(string pathOrCommand)
    {
        _mutex.WaitOne();
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = pathOrCommand;
            process.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при запуску нового процесу: {ex.Message}");
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    public static void StartProcessAsync(string pathOrCommand)
    {
        new Thread(() => StartProcess(pathOrCommand)).Start();
    }

    public static bool IsProcessRunning(int pid)
    {
        _mutex.WaitOne();
        try
        {
            Process.GetProcessById(pid);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    public static Process GetProcessById(int pid)
    {
        _mutex.WaitOne();
        try
        {
            return Process.GetProcessById(pid);
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }

    public static void KillAllProcessesByName(string processName)
    {
        _mutex.WaitOne();
        try
        {
            foreach (Process process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
        finally
        {
            _mutex.ReleaseMutex();
        }
    }
}


namespace TaskManager
{
    internal class Program
    {

        static void StartProcess()
        {
            Console.WriteLine("Введіть шлях до виконуваного файлу або команду:");
            string pathOrCommand = Console.ReadLine();

            Console.WriteLine("Запускаємо процес...");
            ProcessManager.StartProcessAsync(pathOrCommand);

            Console.WriteLine("Процес успішно розпочато (асинхронно).");
        }

        static void KillProcess()
        {
            Console.WriteLine("Введіть PID процесу, який потрібно завершити:");
            int pid = int.Parse(Console.ReadLine());

            Console.WriteLine("Завершуємо процес...");
            ProcessManager.KillProcess(pid);

            Console.WriteLine("Процес успішно завершено (асинхронно).");
        }


        static void Main(string[] args)
        {

            while (true)
            {
                Console.Clear();
                ShowMenu();

                int option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        ShowProcesses();
                        break;
                    case 2:
                        KillProcess();
                        break;
                    case 3:
                        StartProcess();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Невідома опція!");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("Менеджер завдань");
            Console.WriteLine("------------------");
            Console.WriteLine("1. Переглянути список процесів");
            Console.WriteLine("2. Завершити процес");
            Console.WriteLine("3. Створити новий процес");
            Console.WriteLine("0. Вихід");
        }

        static void ShowProcesses()
        {
            List<Process> processes = ProcessManager.GetAllProcesses();

            Console.WriteLine("Список процесів:");
            foreach (Process process in processes)
            {
                Console.WriteLine($"{process.Id} - {process.ProcessName} - {process.StartTime} - {process.WorkingSet64}");
            }
        }

        static void KillProces()
        {
            Console.WriteLine("Введіть PID процесу, який потрібно завершити:");
            int pid = int.Parse(Console.ReadLine());

            ProcessManager.KillProcess(pid); // Call the improved KillProcess method
        }

        static void StartProces()
        {
            Console.WriteLine("Введіть шлях до виконуваного файлу або команду:");
            string pathOrCommand = Console.ReadLine();

            ProcessManager.StartProcess(pathOrCommand); // Call the improved StartProcess method
        }

    }

}

