using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace UltimaCore.Graphics
{
    public static class TileData
    {
        public static void Load()
        {
            int staticscount = 512;


        }
    }


    // old

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LandGroupOld
    {
        public uint Unknown;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public LandTilesOld Tiles;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LandTilesOld
    {
        public uint Flags;
        public ushort TexID;
        [MarshalAs(UnmanagedType.LPStr, SizeConst = 20)]
        public string Name;
    }




    // new 

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LandGroupNew
    {
        public uint Unknown;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public LandTilesNew Tiles;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LandTilesNew
    {
        public ulong Flags;
        public ushort TexID;
        [MarshalAs(UnmanagedType.LPStr, SizeConst = 20)]
        public string Name;
    }
}
