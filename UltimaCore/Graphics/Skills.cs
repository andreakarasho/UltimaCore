using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UltimaCore.Graphics
{
    public static class Skills
    {
        private static UOFileMul _file;

        public static void Load()
        {
            string path = Path.Combine(FileManager.UoFolderPath, "Skills.mul");
            string pathidx = Path.Combine(FileManager.UoFolderPath, "Skills.idx");

            if (!File.Exists(path) || !File.Exists(pathidx))
                throw new FileNotFoundException();

            _file = new UOFileMul(path, pathidx, 55, 16);
        }

        public static SkillEntry GetSkill(int index)
        {
            (int length, int extra, bool patched) = _file.SeekByEntryIndex(index);
            if (length == 0)
                return default;

            return new SkillEntry()
            {
                HasButton = _file.ReadBool(),
                Name = Encoding.UTF8.GetString(_file.ReadArray(length - 1)),
                Index = index
            };
        }
    }

    public struct SkillEntry
    {
        public int Index;
        public string Name;
        public bool HasButton;
    }

}
