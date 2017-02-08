using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace R6GameService_Patcher
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(@"                      ,--------------,");
            Console.WriteLine(@"                      L.,------, ,--'");
            Console.WriteLine(@"                ______/(______( (________,------==========.");
            Console.WriteLine(@"    (###========\_________--===-----=======================|");
            Console.WriteLine(@"                 ,',""""\ ,-+-_   __,___,____,____,____,___|");
            Console.WriteLine(@"                 |=|     \ \ )_) (  )__,____,____,____,____|");
            Console.WriteLine(@"                 |=|      \ \  | |    )====)     `:__      |");
            Console.WriteLine(@"                 |__\      \ \_) |   /====/          \     |");
            Console.WriteLine(@"                            `----'  /====/            \    |");
            Console.WriteLine(@"                                    `-._/             :____|");
            Console.WriteLine();
            Console.WriteLine(@"     R6GameService.dll ""Try Again Time"" patch v1 by Chriswak");
            Console.WriteLine();
            Console.WriteLine();

            //PatchFile(@"C:\Temp\R6GameService.dll", @"C:\Temp\R6GameService.dll");
            PatchFile(@"R6GameService.dll", @"R6GameService.dll");
        }

        private static readonly byte[] PatchFind = { 0xDF, 0xE0, 0xF6, 0xC4, 0x41, 0x75, 0x30 };
        private static readonly byte[] PatchReplace = { 0xDF, 0xE0, 0xF6, 0xC4, 0x41, 0xEB, 0x30 };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool DetectPatch(byte[] sequence, int position)
        {
            if (position + PatchFind.Length > sequence.Length)
            {
                return false;
            }
            for (int p = 0; p < PatchFind.Length; p++)
            {
                if (PatchFind[p] != sequence[position + p])
                {
                    return false;
                }
            }
            return true;
        }

        private static void PatchFile(string originalFile, string patchedFile)
        {
            if (!File.Exists(originalFile))
            {
                Console.WriteLine("DLL not found! Run this program from the same directory where the R6GameService.dll exists.");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            // Read file bytes.
            byte[] fileContent = File.ReadAllBytes(originalFile);
            Console.WriteLine("File read: SUCCESS");
            Console.WriteLine();

            bool canBePatched = false;
            // Detect insert points and patch DLL.
            for (int p = 0; p < fileContent.Length; p++)
            {
                if (!DetectPatch(fileContent, p))
                {
                    continue;
                }
                for (int w = 0; w < PatchFind.Length; w++)
                {
                    canBePatched = true;
                    fileContent[p + w] = PatchReplace[w];
                }
            }
            
            if(canBePatched)
            {
                Console.WriteLine("Patch data: READY");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("!!!ERROR!!!: No patchable code found. Is DLL already patched?");
                Console.WriteLine();
                Console.WriteLine("No patches were applied! Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            // Backup/Save DLL to location.
            Console.WriteLine("!!!WARNING!!! The current R6GameService.dll will be backed up to R6GameService.dll.bak.");
            Console.WriteLine("If you continue, changes will be permanently written to R6GameService.dll!");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press any key to continue -OR- close this window to cancel.");
            Console.ReadKey();
            Console.WriteLine();
            File.Copy(originalFile, originalFile + ".bak", true);
            Console.WriteLine("Backed up current R6GameService.dll to R6GameService.dll.bak...");
            Console.WriteLine();

            try
            {
                File.WriteAllBytes(patchedFile, fileContent);
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("!!!ERROR!!!: Server is running, or file is otherwise read-only.");
                Console.WriteLine();
                Console.WriteLine("No patches were applied! Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Console.WriteLine("Patching: DONE! Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
