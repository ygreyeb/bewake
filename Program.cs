using Bewake;
using System.Runtime.InteropServices;
using System.Timers;
using static Bewake.EXECUTION_STATE;

[DllImport("kernel32.dll", SetLastError = true)]
static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

// keep state to prevent unnecessary calls to win api
bool awake = false;

// create timer to check the number of screens at intervals and prevent screen sleep accordingly
const int CheckInterval = 1000; // ms
System.Timers.Timer timer = new(CheckInterval);
timer.Elapsed += CheckScreens;
// start the timer
timer.Enabled = true;

// catch program flow
Console.WriteLine("Press the Enter key to exit the program at any time... ");
Console.ReadLine();

Console.WriteLine("Release display state before exit");
ResetState();

void CheckScreens(object? sender, ElapsedEventArgs e)
{
    if (Screen.AllScreens.Length > 1)
    {
        if (!awake)
        {
            Console.WriteLine($"Keep display awake on {Screen.AllScreens.Length} screens");
            awake = SetState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED);
        }
    }
    else if (awake)
    {
        Console.WriteLine("Release display state for single screen");
        awake = !ResetState();
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

namespace Bewake
{
    [Flags]
    public enum EXECUTION_STATE : uint
    {
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002
    }
}
