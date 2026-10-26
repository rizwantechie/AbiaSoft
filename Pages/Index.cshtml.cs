using AbiaSoft.Model;
using AbiaSoft.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbiaSoft.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IEmailService emailService, ILogger<IndexModel> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [BindProperty]
        public QuoteRequest Quote { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Quote request submission was rejected because model validation failed. Invalid fields: {InvalidFields}.",
                    string.Join(", ", ModelState
                        .Where(entry => entry.Value?.Errors.Count > 0)
                        .Select(entry => entry.Key)));
                return Page();
            }

            try
            {
                var body = $@"New Quote Request Received
                    Name: {Quote.Name}
                    Email: {Quote.Email}
                    Service: {Quote.Service}
                    Message:
                    {Quote.Message}
                    Submitted: {DateTime.Now:yyyy-MM-dd HH:mm:ss IST}
";
                await _emailService.SendAsync(
                    "New Quote Request from AbiaSoft Technology",
                    body,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Quote request email was sent successfully.");
                TempData["AlertType"] = "success";
                TempData["AlertMessage"] = "Your quote request has been sent successfully! We'll get back to you within 24 hours.";
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Quote request email sending was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send quote request email.");
                TempData["AlertType"] = "danger";
                TempData["AlertMessage"] = "There was an error sending your request. Please try again later.";
            }

            TempData["success"] = "Request submitted successfully!";
            return RedirectToPage();
        }
    }
}
