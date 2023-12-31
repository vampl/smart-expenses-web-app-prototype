using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using smart_expenses_web_app.Enums;
using smart_expenses_web_app.Services;

namespace smart_expenses_web_app.Pages.Account;

[Authorize]
public class EditModel : PageModel
{
    private readonly UserService _userService;
    private readonly AccountService _accountService;

    public EditModel(UserService userService, AccountService accountService)
    {
        // Inject required services
        _userService = userService;
        _accountService = accountService;

        Account = new Models.Account();
    }

    public long? Id { get; set; }
    public string? UserId { get; set; }
    public SelectList? AccountTypesSelectList { get; set; }
    public SelectList? CurrencyCodesSelectList { get; set; }
    
    [BindProperty]
    public Models.Account Account { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        // Check if Id passed
        if (id == null)
        {
            return NotFound();
        }

        // Access current account Id
        Id = id;
        
        // Access current session user guid
        UserId = _userService.GetUserId();
        
        // Prepare form account types list
        var accountTypesSelectListItems = Enum.GetValues<AccountType>()
            .Select(accountType => new SelectListItem { Text = accountType.ToString(), Value = accountType.ToString() })
            .ToList();
        AccountTypesSelectList = new SelectList(accountTypesSelectListItems, "Value", "Text");
        
        // Prepare form currency codes list
        var currencyCodesSelectListItems = Enum.GetValues<CurrencyCode>()
            .Select(currencyCode => new SelectListItem { Text = currencyCode.ToString(), Value = currencyCode.ToString() })
            .ToList();
        CurrencyCodesSelectList = new SelectList(currencyCodesSelectListItems, "Value", "Text");
        
        // Check if account exist in database
        var account = await _accountService.GetAccountAsync(id);
        if (account == null)
        {
            return NotFound();
        }
        
        // Fill account form
        Account = account;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Check if can be pushed to database
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            
            return Page();
        }

        await _accountService.EditUserAccountAsync(Account);

        return RedirectToPage("./Index");
    }
}