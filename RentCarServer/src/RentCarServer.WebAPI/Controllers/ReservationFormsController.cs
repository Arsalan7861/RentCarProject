using Microsoft.AspNetCore.Mvc;
using RentCarServer.Application.Reservations.Froms;
using TS.MediatR;

namespace RentCarServer.WebAPI.Controllers;
[Route("reservation-form")]
[ApiController]
public class ReservationFormsController(
    ISender sender
    ) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> UpdateForm(
        [FromForm] FormUpdateCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return Ok(result);
    }
}
