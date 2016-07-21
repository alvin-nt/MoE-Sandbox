using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SimpleSandboxShell.InputFilter
{
    /// <summary>
    /// Contains information about low-level keyboard input event.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KeyboardLowLevelHook
    {
        /// <summary>
        /// Virtual key code.
        /// </summary>
        public Keys KeyCode;

        /// <summary>
        /// A hardware scan code for the key.
        /// </summary>
        public uint ScanCode;

        /// <summary>
        /// The extended-key flag, event-injected flags, context code, and transition-state flag.
        /// </summary>
        public EventFlags Flags;

        /// <summary>
        /// Time stamp of the event message.
        /// </summary>
        public uint Time;

        /// <summary>
        /// Additional information associated with the message.
        /// </summary>
        public IntPtr PtrExtraInfo;

        [Flags]
        public enum EventFlags : uint
        {
            Extended = 0x01,
            LowerInputLevelInjected = 0x00000002,
            Injected = 0x10,
            AltDown = 0x20,
            Up = 0x80,
        }
    }

    internal enum KeyboardEvent
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        SysKeyDown = 0x0104,
        SysKeyUp = 0x0105,
        SysChar = 0x0106,
    }

    /// <summary>
    /// Represents cursor location on the screen
    /// </summary>
    /// <remarks>
    /// Original source from http://www.pinvoke.net/default.aspx/Structures/POINT.html
    /// </remarks>
    internal struct MousePoint
    {
        public int X, Y;

        public MousePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public MousePoint(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

        public static implicit operator System.Drawing.Point(MousePoint p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator MousePoint(System.Drawing.Point p)
        {
            return new MousePoint(p.X, p.Y);
        }
    }

    internal struct MouseLowLevelHook
    {
        /// <summary>
        /// The x, y location of the cursor
        /// </summary>
        public MousePoint Point;

        /// <summary>
        /// Data associated with the evnet
        /// </summary>
        public uint MouseData;
        public EventFlags Flags;
        public uint Time;
        public IntPtr PtrExtraInfo;

        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum EventFlags
        {
            Injected = 0x00000001,
            LowerIntegrityLevelInjected = 0x00000002
        }
    }

    internal enum MouseEvent
    {
        LeftButtonDown = 0x0201,
        LeftButtonUp = 0x0202,
        MouseMove = 0x0200,
        MouseWheel = 0x020A,
        RightButtonDown = 0x0204,
        RightButtonUp = 0x0205
    }
}
