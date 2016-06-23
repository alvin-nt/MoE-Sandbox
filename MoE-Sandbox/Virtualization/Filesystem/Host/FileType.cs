namespace MoE_Sandbox.Virtualization.Filesystem.Host
{
    public enum FileType : uint
    {
        Unknown = 0x0000, // FILE_TYPE_UNKNOWN
        Disk = 0x0001, // FILE_TYPE_DISK
        Char = 0x0002, // FILE_TYPE_CHAR
        Pipe = 0x0003, // FILE_TYPE_PIPE
        Remote = 0x8000 // FILE_TYPE_REMOTE
    }
}
