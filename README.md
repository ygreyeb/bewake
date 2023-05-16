# Bewake

There are times when working on a computer that requires the user to keep their screen on and active for an extended period, but at
the same time, they need to switch their attention to a second monitor, sometimes even going AFK for those intervals. In such
situations, the computer's screen may go to sleep or lock due to inactivity, causing inconvenience and interrupting the user's
workflow.

Bewake solves this problem by keeping the computer's screen awake when there are secondary screens available. It will detect the
availability of multiple monitors and maintain the display's active state even when the user is not interacting with it.

Bewake will run in the background, constantly detecting the presence of secondary monitors. Once it detects the secondary screens,
it will inform the system that an application is in use, thereby preventing the system from entering sleep or turning off the
display while the application is running.

Bewake is designed as a small .Net application. As such, it is not as lightweight as a program with this capability can be, but it
is unobtrusive, running in the background without affecting the computer's performance or causing any significant system overhead.
