using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnit;
using WebNUnit.Models;
using WebNUnit.TestingContext;

namespace WebNUnit.Controllers
{
    /// <summary>
    /// Implementation of home controller 
    /// </summary>
    public class HomeController : Controller
    {
        private TestContext testContext;
        private static string dir = Directory.GetCurrentDirectory();
        private string path = dir + "\\Testings";

        public HomeController()
        {
            testContext = new TestContext();
        }

        /// <summary>
        /// Start Page view
        /// </summary>
        public IActionResult Index()
            => View();

        /// <summary>
        /// Load Assembly Page view 
        /// </summary>
        public IActionResult LoadAssemblyPage()
        {
            return View("LoadAssemblyPage", GetAllDll());
        }

        private string[] GetAllDll()
        {
            var dlls = Directory
                .GetFiles(path, "*.dll")
                .Select(item => item[(path.Length + 1)..item.Length]);
            var enumerable = dlls as string[] ?? dlls.ToArray();
            return enumerable;
        }

        /// <summary>
        /// Load the assembly from specified path
        /// </summary>
        /// <param name="file">path to assembly</param>
        /// <returns>view of loaded assemblies</returns>
        public async Task<IActionResult> LoadAssembly(IFormFile file)
        {
            if (Directory.GetFiles(path).Contains(Path.Combine(path, file.FileName))) return LoadAssemblyPage();
            await using var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.CreateNew);
            await file.CopyToAsync(fileStream);
            return LoadAssemblyPage();
        }

        /// <summary>
        /// Run all tests
        /// </summary>
        /// <returns>view of loaded assemblies</returns>
        public async Task<IActionResult> Run()
        {
            var orderedNames = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

            var types = orderedNames
                .Select(Assembly.LoadFrom)
                .SelectMany(a => a.ExportedTypes);

            Runner.Run(path);

            var assembliesTests = Runner.TestInformation;

            var i = 0;

            foreach (var type in types)
            {
                var assembly = new LoadedAssembliesViewModel();
                var listOfMembersOfDll = type
                    .GetMembers()
                    .Select(member => assembliesTests!
                        .ToList()
                        .Find(item => item.Name == member.Name))
                    .Where(firstFound => firstFound != default(TestInformation))
                    .ToList();
                foreach (var test in listOfMembersOfDll.Select(item => new TestViewModel
                         {
                             Result = item!.Result,
                             Name = item.Name,
                             StartTime = DateTime.Now,
                             Time = item.Time,
                             IgnoreReason = item.IgnoreReason ?? "there is no reason for ignoring"
                         }))
                {
                    //Console.Write(test.Name, test.Result);
                    assembly.Tests.Add(test);
                }
                assembly.Name = orderedNames[i][(path.Length + 1)..orderedNames[i].Length];
                i++;
                testContext.AssembliesHistory.Add(assembly);
            }

            await testContext.SaveChangesAsync();
            return View("LoadAssemblyPage", GetAllDll());
        }

        /// <summary>
        /// Delete all loaded assemblies
        /// </summary>
        public IActionResult Delete()
        {
            foreach (var file in new DirectoryInfo(path).GetFiles())
            {
                try
                {
                    file.Delete();
                }
                // заглушка для падающей дллки
                catch (Exception exception)
                {
                    Console.Write(exception.Message);
                }

            }

            return View("LoadAssemblyPage", GetAllDll());
        }

        /// <summary>
        /// History about all testings
        /// </summary>
        /// <returns>history view</returns>
        public IActionResult History()
        {
            var assemblies = testContext.AssembliesHistory
                .Include(x => x.Tests)
                .ToList()
                .Reverse<LoadedAssembliesViewModel>();
            return View("History", assemblies);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}