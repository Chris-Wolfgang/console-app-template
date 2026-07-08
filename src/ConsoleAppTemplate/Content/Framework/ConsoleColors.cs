namespace ConsoleAppTemplate.Framework
{
    internal static class ConsoleColors
    {
        // https://ss64.com/nt/syntax-ansi.html

        public static readonly string Reset         = "\u001b[0m";
        public static readonly string Bold          = "\u001b[1m";
        public static readonly string Underline     = "\u001b[4m";
        public static readonly string NoUnderline   = "\u001b[24m";
        public static readonly string ReverseText   = "\u001b[7m";
        public static readonly string PositiveText  = "\u001b[27m";



        internal static class Background
        {
            public static readonly string Black        = "\u001b[40m";
            public static readonly string Red          = "\u001b[41m";
            public static readonly string Green        = "\u001b[42m";
            public static readonly string Yellow       = "\u001b[43m";
            public static readonly string Blue         = "\u001b[44m";
            public static readonly string Magenta      = "\u001b[45m";
            public static readonly string Cyan         = "\u001b[46m";
            public static readonly string LightGray    = "\u001b[47m";
            public static readonly string Gray         = "\u001b[100m";
            public static readonly string LightRed     = "\u001b[101m";
            public static readonly string LightGreen   = "\u001b[102m";
            public static readonly string LightYellow  = "\u001b[103m";
            public static readonly string LightBlue    = "\u001b[104m";
            public static readonly string LightMagenta = "\u001b[105m";
            public static readonly string LightCyan    = "\u001b[106m";
            public static readonly string White        = "\u001b[107m";
        }

        internal static class Foreground
        {
            public static readonly string Black        = "\u001b[30m";
            public static readonly string Red          = "\u001b[31m";
            public static readonly string Green        = "\u001b[32m";
            public static readonly string Yellow       = "\u001b[33m";
            public static readonly string Blue         = "\u001b[34m";
            public static readonly string Magenta      = "\u001b[35m";
            public static readonly string Cyan         = "\u001b[36m";
            public static readonly string LightGray    = "\u001b[37m";
            public static readonly string DarkGray     = "\u001b[90m";
            public static readonly string LightRed     = "\u001b[91m";
            public static readonly string LightGreen   = "\u001b[92m";
            public static readonly string LightYellow  = "\u001b[93m";
            public static readonly string LightBlue    = "\u001b[94m";
            public static readonly string LightMagenta = "\u001b[95m";
            public static readonly string LightCyan    = "\u001b[96m";
            public static readonly string White        = "\u001b[97m";
        }
    }
}
