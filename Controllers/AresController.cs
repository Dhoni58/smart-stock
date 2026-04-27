using Microsoft.AspNetCore.Mvc;
using WarehouseSystem.Services;

namespace WarehouseSystem.Controllers;

[ApiController]
[Route("api/ares")]
public class AresController : ControllerBase
{
    private readonly AresService _aresService;

    public AresController(AresService aresService)
    {
        _aresService = aresService;
    }

    [HttpGet("{ico}")]
    public async Task<IActionResult> GetByIco(string ico)
    {
        var result = await _aresService.GetByIcoAsync(ico);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}