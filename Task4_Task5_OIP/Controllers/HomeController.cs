using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Task5_DEMO_OIP.Models;
using Task5_DEMO_OIP.Services;

namespace Task5_DEMO_OIP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SearchService _searchService;

        public HomeController(ILogger<HomeController> logger, SearchService searchService)
        {
            _logger = logger;
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(string query)
        {
            var model = new Task5_DEMO_OIP.Models.SearchViewModel
            {
                Query = query,
                IsSearchPerformed = !string.IsNullOrEmpty(query)
            };

            try
            {
                if (!_searchService.IsLoaded())
                {
                    await _searchService.LoadIndexAsync();
                    model.TotalDocuments = _searchService.GetDocumentCount();
                    model.UniqueTerms = _searchService.GetUniqueTermsCount();
                }
                else
                {
                    model.TotalDocuments = _searchService.GetDocumentCount();
                    model.UniqueTerms = _searchService.GetUniqueTermsCount();
                }

                if (!string.IsNullOrEmpty(query))
                {
                    var stopwatch = Stopwatch.StartNew();

                    model.Results = await _searchService.SearchAsync(query);
                    model.SearchTime = stopwatch.Elapsed.TotalMilliseconds;

                    if (model.Results.Count == 0)
                    {
                        model.ErrorMessage = "По вашему запросу ничего не найдено. Попробуйте изменить запрос.";
                    }
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Ошибка при поиске: {ex.Message}";
                _logger.LogError(ex, "Ошибка при выполнении поиска");
            }

            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}