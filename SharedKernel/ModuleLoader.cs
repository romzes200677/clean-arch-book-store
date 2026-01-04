using System.Reflection;
using System.Runtime.Loader;

namespace SharedKernel;

public static class ModuleLoader
{
    public static List<Assembly> LoadModuleAssemblies(string modulesPath)
    {
        var assemblies = new List<Assembly>();
        
        // Добавляем саму сборку приложения (где Program.cs)
        assemblies.Add(Assembly.GetEntryAssembly()!);

        if (!Directory.Exists(modulesPath)) return assemblies;

        // Ищем все dll, имена которых начинаются с вашего префикса (например, "BookStore.")
        // Это важно, чтобы не пытаться загрузить системные dll
        var dlls = Directory.GetFiles(modulesPath, "BookStore.*.dll", SearchOption.AllDirectories);

        foreach (var dll in dlls)
        {
            try
            {
                // Загружаем сборку в текущий контекст
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
                if (!assemblies.Contains(assembly))
                {
                    assemblies.Add(assembly);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить модуль {dll}: {ex.Message}");
            }
        }

        return assemblies;
    }
}