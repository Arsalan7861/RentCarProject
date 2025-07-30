using GenericRepository;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Vehicles;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Vehicles;
[Permission("vehicle:delete")]
public sealed record VehicleDeleteCommand(Guid Id) : IRequest<Result<string>>;

internal sealed class VehicleDeleteCommandHandler(
    IVehicleRepository vehicleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<VehicleDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VehicleDeleteCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (vehicle is null)
            return Result<string>.Failure("Araç bulunamad?.");

        // Araç resmi varsa dosyadan sil (manuel)
        if (!string.IsNullOrWhiteSpace(vehicle.ImageUrl?.Value))
        {
            // wwwroot/images/ ile tam dosya yolunu olu?tur
            var filePath = Path.Combine("wwwroot/images", Path.GetFileName(vehicle.ImageUrl.Value));
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    // Dosya silinemese bile devam et
                }
            }
        }

        vehicle.Delete();
        vehicleRepository.Update(vehicle);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Araç ba?ar?yla silindi.";
    }
}