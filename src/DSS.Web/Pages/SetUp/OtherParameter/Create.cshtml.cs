using DSS.OthersParameters.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace DSS.Web.Pages.SetUp.OtherParameter
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public TblOtherParameterCreateDto otherParametercreatedto { get; set; }
        private readonly IOtherParameterAppService _appService;
        private readonly ICurrentUser _currentUser;

        public CreateModel(IOtherParameterAppService appService, ICurrentUser currentUser)
        {
            _appService = appService;
            _currentUser = currentUser;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostCreate(TblOtherParameterCreateDto otherParametercreatedto)
        {
           
            var  User = _currentUser.UserName;
            otherParametercreatedto.CreatedBy = User;

            var createdParameter = await _appService.CreateParameterAsync(otherParametercreatedto);
            if (createdParameter != null)
            {
                return RedirectToPage("/SetUp/OtherParameter/List");
            }
            else
            {
                return Page(); 
            }
        }

    }
}
