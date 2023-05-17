using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using static Bewake.EXECUTION_STATE;

namespace Bewake
{
    [Flags]
    public enum EXECUTION_STATE : uint
    {
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002
    }

    internal static partial class Program
    {
        [LibraryImport("kernel32.dll", SetLastError = true)]
        private static partial EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        [SupportedOSPlatform("windows")]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Container container = new();

            ContextMenuStrip contextMenu = new(container);
            contextMenu.Items.Add("E&xit", null, ExitApplication);

            Icon offIcon = new(@"ico\off.ico");
            Icon onIcon = new(@"ico\on.ico");

            NotifyIcon trayIcon = new(container)
            {
                Icon = offIcon,
                Text = "Bewake is iddle",
                ContextMenuStrip = contextMenu,
                Visible = true
            };

            // keep state to prevent unnecessary calls to win api
            bool awake = false;

            // timer to check number of screens at intervals and prevent screen sleep
            const int CheckInterval = 1000; // ms
            System.Windows.Forms.Timer timer = new(container)
            {
                Interval = CheckInterval
            };
            timer.Tick += CheckScreens;
            timer.Start();

            Application.Run();

            void CheckScreens(object? sender, EventArgs e)
            {
                if (Screen.AllScreens.Length > 1)
                {
                    if (!awake)
                    {
                        Console.WriteLine($"Keep display awake on {Screen.AllScreens.Length} screens");
                        if (SetState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED))
                        {
                            awake = true;
                            trayIcon.Icon = onIcon;
                            trayIcon.Text = "Bewake is active";
                        }
                    }
                }
                else if (awake)
                {
                    Console.WriteLine("Release display state for single screen");
                    if (ResetState())
                    {
                        awake = false;
                        trayIcon.Icon = offIcon;
                        trayIcon.Text = "Bewake is iddle";
                    }
                }
            }

            void ExitApplication(object? sender, EventArgs e)
            {
                container.Dispose();

                Console.WriteLine("Release display state before exit");
                ResetState();

                Application.Exit();
            }
        }

        static bool ResetState()
        {
            return SetState(ES_CONTINUOUS);
        }

        static bool SetState(EXECUTION_STATE flags)
        {
            bool succeeded = SetThreadExecutionState(flags) != 0;
            if (!succeeded) Console.WriteLine($"Error: unable to set thread execution state [{flags}]");
            return succeeded;
        }
    }
}
