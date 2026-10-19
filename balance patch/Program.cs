using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;


class Program
{
    const string ProcessName = "Stronghold2";

    const int PROCESS_VM_READ = 0x0010;
    const int PROCESS_QUERY_INFORMATION = 0x0400;
    const int PROCESS_VM_WRITE = 0x0020;
    const int PROCESS_VM_OPERATION = 0x0008;
    const int PAGE_EXECUTE_READWRITE = 0x40;

    //units addresses
    const long PeasantAddress = 0xEB130C;
    const long SpearmanAddress = 0xEB23F4;
    const long ArcherAddress = 0xEB367C;
    const long CrossbowmanAddress = 0xEB16F4;
    const long MacemanAddress = 0xEB1E6C;
    const long PikemanAddress = 0xEB221C;
    const long SwordsmanAddress = 0xEB25CC;
    const long KnightAddress = 0xEB1AB4;

    const long MonkAddress = 0xEB2044;
    const long WarriorMonkAddress = 0xEB27A4;

    const long BoatWarriorAddress = 0xEB2F14;
    const long OutlawAddress = 0xEB30EC;
    const long BerserkerAddress = 0xEB2B5C;
    const long AxeThrowerAddress = 0xEB2984;
    const long LightCavalryAddress = 0xEB32CC;
    const long HorseArcherAddress = 0xEB2D3C;
    const long AssassinAddress = 0xEB14E4;
    const long ThiefAddress = 0xEB34A4;

    const long LaddermanAddress = 0xEB1C8C;
    const long EngineerAddress = 0xEB18D4;

    const long LordAddress = 0xEBCC7C;

    //units stats addresses
    const long SpeedAddress = 0x40;
    const long HPAddress = 0x4C;
    const long TakeDamageShootTypeAddress = 0x80;
    const long TakeDamageMeleeATypeAddress = 0x84;
    const long TakeDamageMeleeBTypeAddress = 0x88;
    const long DamageAddress = 0x8C;
    const long TitleAddress = 0xB8;


    //gold cost addresses
    const long CostOffsetAddress = 0x68D840;
    const long SpearmanCostAddress = 0x4;
    const long ArcherCostAddress = 0x8;
    const long SwordsmanCostAddress = 0x18;
    const long OutlawCostAddress = 0x2C;
    const long BoatWarriorCostAddress = 0x38;

    //Range damages (no need unit's struct)
    const long CowsDamageAddress = 0x50785C;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
        byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
    IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    static void Main()
    {
        Process process = Process.GetProcessesByName(ProcessName).FirstOrDefault();

        if (process == null)
        {
            Console.WriteLine("Process not found");
            return;
        }

        long baseAddress = process.MainModule.BaseAddress.ToInt64(); // Base address of Stronghold 2

        IntPtr hProc = OpenProcess(
            PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_QUERY_INFORMATION,
            false, process.Id);

        if (hProc == IntPtr.Zero)
        {
            Console.WriteLine($"OpenProcess failed. Run as admin.");
            return;
        }

        Patch_v01(baseAddress, hProc);
        //THEBESTPATCH(baseAddress, hProc);
    }

    // very-very strong spearmen
    static void THEBESTPATCH(long baseAddress, IntPtr hProc)
    {
        WriteFloatData(0.0f, baseAddress + SpearmanAddress + TakeDamageMeleeATypeAddress, hProc);
        WriteFloatData(0.0f, baseAddress + SpearmanAddress + TakeDamageMeleeBTypeAddress, hProc);
        WriteFloatData(0.0f, baseAddress + SpearmanAddress + TakeDamageShootTypeAddress, hProc);
        WriteFloatData(1000.0f, baseAddress + SpearmanAddress + DamageAddress, hProc);
    }

    static void Patch_v01(long baseAddress, IntPtr hProc)
    {
        //spearman
        WriteIntData(0, baseAddress + CostOffsetAddress + SpearmanCostAddress, hProc); // cost spearman 8->0

        //archer
        WriteIntData(0, baseAddress + CostOffsetAddress + ArcherCostAddress, hProc); // cost archer 12->0

        //outlaw
        WriteIntData(40, baseAddress + CostOffsetAddress + OutlawCostAddress, hProc); // cost outlaw 60->40
        WriteFloatData(6.0f, baseAddress + OutlawAddress + SpeedAddress, hProc); // 4->6 speed

        //boatwarrior
        WriteIntData(20, baseAddress + CostOffsetAddress + BoatWarriorCostAddress, hProc);
        WriteFloatData(5.0f, baseAddress + BoatWarriorAddress + SpeedAddress, hProc);

        //Maceman
        WriteFloatData(9000.0f, baseAddress + MacemanAddress + HPAddress, hProc);
        WriteFloatData(45.0f, baseAddress + MacemanAddress + DamageAddress, hProc);
        WriteFloatData(6.5f, baseAddress + MacemanAddress + SpeedAddress, hProc);
        WriteFloatData(0.4f, baseAddress + MacemanAddress + TakeDamageMeleeATypeAddress, hProc);

        //Pikeman
        WriteFloatData(13.0f, baseAddress + PikemanAddress + DamageAddress, hProc);
        WriteFloatData(0.08f, baseAddress + PikemanAddress + TakeDamageMeleeATypeAddress, hProc);
        WriteFloatData(0.15f, baseAddress + PikemanAddress + TakeDamageMeleeBTypeAddress, hProc);
        WriteFloatData(4.0f, baseAddress + PikemanAddress + SpeedAddress, hProc);

        //Swordsman
        WriteFloatData(12500.0f, baseAddress + SwordsmanAddress + HPAddress, hProc);
        WriteFloatData(55.0f, baseAddress + SwordsmanAddress + DamageAddress, hProc);
        WriteFloatData(0.15f, baseAddress + SwordsmanAddress + TakeDamageMeleeBTypeAddress, hProc);
        WriteFloatData(2.5f, baseAddress + SwordsmanAddress + SpeedAddress, hProc);
        WriteIntData(30, baseAddress + CostOffsetAddress + SwordsmanCostAddress, hProc);

        //Berserker
        WriteFloatData(90.0f, baseAddress + BerserkerAddress + DamageAddress, hProc);
        WriteFloatData(1.635f, baseAddress + BerserkerAddress + TakeDamageMeleeBTypeAddress, hProc);
        WriteFloatData(5.0f, baseAddress + BerserkerAddress + SpeedAddress, hProc);

        //Warrior Monk
        WriteFloatData(9000.0f, baseAddress + WarriorMonkAddress + HPAddress, hProc);
        WriteFloatData(20.0f, baseAddress + WarriorMonkAddress + DamageAddress, hProc);
        WriteFloatData(0.5f, baseAddress + WarriorMonkAddress + TakeDamageShootTypeAddress, hProc);
        WriteFloatData(0.25f, baseAddress + WarriorMonkAddress + TakeDamageMeleeATypeAddress, hProc);
        WriteFloatData(0.25f, baseAddress + WarriorMonkAddress + TakeDamageMeleeBTypeAddress, hProc);

        //Catapult (work but change other things, need some fixes)
        //WriteFloatData(100.0f, baseAddress + CowsDamageAddress, hProc);
    }

    static void WriteIntData(int newData, long address, IntPtr hProc)
    {
        byte[] buffer = BitConverter.GetBytes(newData);
        VirtualProtectEx(hProc, new IntPtr(address), new IntPtr(buffer.Length), PAGE_EXECUTE_READWRITE, out uint oldProtect);
        WriteProcessMemory(hProc, new IntPtr(address), buffer, buffer.Length, out IntPtr written);
        VirtualProtectEx(hProc, new IntPtr(address), new IntPtr(buffer.Length), oldProtect, out _);
    }
    static void WriteFloatData(float newData, long address, IntPtr hProc)
    {
        byte[] buffer = BitConverter.GetBytes(newData);
        VirtualProtectEx(hProc, new IntPtr(address), new IntPtr(buffer.Length), PAGE_EXECUTE_READWRITE, out uint oldProtect);

        WriteProcessMemory(hProc, new IntPtr(address), buffer, buffer.Length, out _);

        VirtualProtectEx(hProc, new IntPtr(address), new IntPtr(buffer.Length), oldProtect, out _);
    }
}