using Mono.Cecil;
using System;
using System.Reflection;
using System.Linq;
using Mono.Cecil.Cil;

namespace CecilHooker
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = "C:/Program Files (x86)/Steam/SteamApps/common/Unturned/Unturned_Data/Managed/Assembly-CSharp.dll";
            string pathHere = "C:/users/redman/documents/visual studio 2013/Projects/CecilHooker/CecilHooker/bin/Debug/CecilHooker.exe";
            var mod = ModuleDefinition.ReadModule(path);
            var networkChatType = mod.Types.First(t => t.FullName == "NetworkChat");
            var tellChatMethod = networkChatType.Methods.First(m => m.Name == "tellChat");
            var ilp = tellChatMethod.Body.GetILProcessor();
            ilp.InsertBefore(tellChatMethod.Body.Instructions[0], ilp.Create(OpCodes.Call, mod.Types.First()));
            /*var tm = new MethodDefinition("tellChat_uh", tellChatMethod.Attributes, tellChatMethod.ReturnType);
            tm.Body = tellChatMethod.Body;

            var thisMod = ModuleDefinition.ReadModule(pathHere);
            var thisClass = thisMod.Types.First(t => t.Name == "Program");
            var preHookMethod = thisClass.Methods.First(m => m.Name == "hook");
            Console.WriteLine("Did this");
            var hookMethod = new MethodDefinition("tellChatHook", Mono.Cecil.MethodAttributes.Public, mod.TypeSystem.Void);
            hookMethod.Body = preHookMethod.Body;
            
            networkChatType.Methods.Add(tm);
            networkChatType.Methods.Add(hookMethod);

            var processor = tellChatMethod.Body.GetILProcessor();
            tellChatMethod.Body.Instructions.Clear();
            processor.Append(processor.Create(OpCodes.Call, hookMethod));
            Console.WriteLine("did that");
            Console.ReadLine();*/
        }

        public static void hook()
        {
            Console.WriteLine("Did Another Thing");
            typeof(NetworkChat).GetMethod("tellChat_uh").Invoke(null, new Object[] {"nigger", "negro", "giganigga", "redman", 0, 0, 0});
        }
    }
}
