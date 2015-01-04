using System;
using System.Diagnostics;
using System.Threading;
using Sandbox.ModAPI;

namespace SEInject
{
    using System;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System.Reflection;
    using System.Collections.Generic;
    using VRageMath;
    using System.IO;

    namespace Injector
    {
        public class MainClass
        {
            public static void PrintPlayers()
            {
                List<IMyPlayer> players = new List<IMyPlayer>();
                MyAPIGateway.Players.GetPlayers(players);
                foreach (IMyPlayer player in players)
                {
                    Console.WriteLine(player.DisplayName + " - " + player.GetPosition());
                }
            }
            public static void DoStuff()
            {
                new Thread((obj) =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        if (MyAPIGateway.Session != null && MyAPIGateway.Session.Player != null && MyAPIGateway.Multiplayer != null && MyAPIGateway.Players != null) break;
                    }

                    MyAPIGateway.Session.RegisterComponent(new TestComponent(), Sandbox.Common.MyUpdateOrder.AfterSimulation, 0);

                    while (true)
                    {
                        Thread.Sleep(10000);
                        outPutFields();
                        PrintPlayers();
                        HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
                        MyAPIGateway.Entities.GetEntities(entities);
                    }
                }).Start();
            }
            //STRING STUFF
            public static void outPutFields()
            {
                String names = "";
                Assembly[] a = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ass in a)
                {
                    foreach (Type t in ass.GetTypes())
                    {
                        foreach (FieldInfo field in t.GetFields())
                        {
                            if (field.FieldType == typeof(Vector3)&&field.IsStatic)
                            {
                                try
                                {
                                    names += field.Name + " . " + t.Name + "\n" + field.GetValue(null) + "\n";
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                writeStringToFile(names,"Vector3Fields");            }
            public static void writeStringToFile(String s, String filename)
            {
                string path = @"C:\Users\Public\Desktop\"+filename+".txt";
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(s);
                tw.Close();
            }
            public static void outPutMethods()
            {
                String names = "";
                Assembly[] a = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ass in a)
                {
                    foreach (Type t in ass.GetTypes())
                    {
                        foreach (MethodInfo method in t.GetMethods())
                        {
                            Type methodReturn = method.ReturnType;
                            if (methodReturn.Equals(typeof(System.String)))
                            {
                                if (method.GetParameters().Length == 0 && method.IsStatic && method.IsPublic)
                                {
                                    try
                                    {
                                        var s = method.Invoke(null, null);
                                        names += method.Name+" . "+t.Name + "\n" + s + "\n";
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                writeStringToFile(names,"StringMethods");
            }
            public static void InjectedCode()
            {
                Console.WriteLine("Hai, I'm all up in there");
                DoStuff();
                /*
                Assembly assembly = Assembly.Load ("Bin64\\SEInject.exe");
                assembly.EntryPoint.Invoke (null, new object[] { new string[] { "injected" } });
                */
            }
            public static void Main(string[] args)
            {
                if (args.Length > 0 && args[0] == "injected")
                {
                    DoStuff();
                    return;
                }
                String dir = @"C:\Program Files (x86)\Steam\SteamApps\common\SpaceEngineers\Bin64\";
                if (!System.IO.File.Exists(dir + "SpaceEngineers.exe.bak"))
                {
                    System.IO.File.Copy(dir + "SpaceEngineers.exe", dir + "SpaceEngineers.exe.bak");
                }
                AssemblyDefinition sourceAssembly = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location);
                AssemblyDefinition targetAssembly = AssemblyDefinition.ReadAssembly(dir + "SpaceEngineers.exe.bak");
                MethodDefinition sourceMethod = getMethod(sourceAssembly.EntryPoint.DeclaringType, "InjectedCode");
                MethodDefinition targetMethod = targetAssembly.EntryPoint;
                int index = 0;
                foreach (Instruction instruction in sourceMethod.Body.Instructions)
                {
                    if (instruction.Operand is TypeReference)
                    {
                        instruction.Operand = targetAssembly.MainModule.Import((TypeReference)instruction.Operand);
                    }

                    if (instruction.Operand is MethodReference)
                    {
                        instruction.Operand = targetAssembly.MainModule.Import((MethodReference)instruction.Operand);
                    }

                    if (instruction.Operand is FieldReference)
                    {
                        instruction.Operand = targetAssembly.MainModule.Import((FieldReference)instruction.Operand);
                    }

                    // don't copy last instruction (return)
                    if (instruction.Next == null)
                        break;
                    targetMethod.Body.Instructions.Insert(index++, instruction);
                    Console.WriteLine(instruction);
                }
                targetAssembly.MainModule.Kind = ModuleKind.Console;
                //targetAssembly.Write(dir + "SpaceEngineers.exe");
                System.IO.File.Copy("SEInject.exe", dir + "SEInject.exe", true);
                Process.Start(new ProcessStartInfo()
                {
                    FileName = dir + "SpaceEngineers.exe",
                    WorkingDirectory = dir + @"..\"
                });
            }
            public static MethodDefinition getMethod(TypeDefinition type, String name)
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.Name != name)
                        continue;

                    return method;
                }
                return null;
            }
        }
    }
}
