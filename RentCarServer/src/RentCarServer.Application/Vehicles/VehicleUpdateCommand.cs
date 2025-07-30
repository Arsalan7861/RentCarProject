using FluentValidation;
using GenericFileService.Files;
using GenericRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentCarServer.Application.Behaviors;
using RentCarServer.Domain.Abstractions;
using RentCarServer.Domain.Shared;
using RentCarServer.Domain.Vehicles;
using RentCarServer.Domain.Vehicles.ValueObjects;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Vehicles;
[Permission("vehicle:edit")]
public sealed record VehicleUpdateCommand(
    Guid Id,
    string Brand,
    string Model,
    int ModelYear,
    string Color,
    string Plate,
    Guid CategoryId,
    Guid BranchId,
    string VinNumber,
    string EngineNumber,
    string Description,
    string FuelType,
    string Transmission,
    decimal EngineVolume,
    int EnginePower,
    string TractionType,
    decimal FuelConsumption,
    int SeatCount,
    int Kilometer,
    decimal DailyPrice,
    decimal WeeklyDiscountRate,
    decimal MonthlyDiscountRate,
    string InsuranceType,
    DateOnly LastMaintenanceDate,
    int LastMaintenanceKm,
    int NextMaintenanceKm,
    DateOnly InspectionDate,
    DateOnly InsuranceEndDate,
    DateOnly? CascoEndDate,
    string TireStatus,
    string GeneralStatus,
    List<string> Features,
    bool IsActive
) : IRequest<Result<string>>
{
    [FromForm]
    public IFormFile? File { get; init; }
}

internal sealed class VehicleUpdateCommandValidator : AbstractValidator<VehicleUpdateCommand>
{
    public VehicleUpdateCommandValidator()
    {
        RuleFor(x => x.Brand).NotEmpty().WithMessage("Lütfen araç markasını giriniz.");
        RuleFor(x => x.Model).NotEmpty().WithMessage("Lütfen araç modelini giriniz.");
        RuleFor(x => x.ModelYear).GreaterThan(1900).WithMessage("Lütfen geçerli bir model yılı giriniz. 1900'den büyük olmal?");
        RuleFor(x => x.Color).NotEmpty().WithMessage("Lütfen araç rengini giriniz.");
        RuleFor(x => x.Plate).NotEmpty().WithMessage("Lütfen araç plakasını giriniz.");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Lütfen araç kategorisini seçiniz.");
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("Lütfen araç şubesini seçiniz.");
        RuleFor(x => x.Features)
            .Must(features => features.Count > 0)
            .WithMessage("Lütfen en az bir özellik ekleyiniz.");
    }
}

internal sealed class VehicleUpdateCommandHandler(
    IVehicleRepository vehicleRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<VehicleUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VehicleUpdateCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (vehicle is null)
            return Result<string>.Failure("Araç bulunamad?.");

        // Plaka benzersizli?i kontrolü (güncellenen hariç)
        bool plateExists = await vehicleRepository.AnyAsync(
            x => x.Plate.Value == request.Plate && x.Id != request.Id, cancellationToken);
        if (plateExists)
            return Result<string>.Failure("Bu plakaya sahip başka bir araç zaten mevcut.");

        // Yeni resim gelirse kaydet, yoksa mevcut resmi koru
        ImageUrl imageUrl = vehicle.ImageUrl;
        if (request.File is not null)
        {
            // Yeni resmi kaydet
            string fileName = FileService.FileSaveToServer(request.File, "wwwroot/images/");
            imageUrl = new ImageUrl(fileName);
        }

        Brand brand = new(request.Brand);
        Model model = new(request.Model);
        ModelYear modelYear = new(request.ModelYear);
        Color color = new(request.Color);
        Plate plate = new(request.Plate);
        IdentityId categoryId = new(request.CategoryId);
        IdentityId branchId = new(request.BranchId);
        VinNumber vinNumber = new(request.VinNumber);
        EngineNumber engineNumber = new(request.EngineNumber);
        Description description = new(request.Description);
        FuelType fuelType = new(request.FuelType);
        Transmission transmission = new(request.Transmission);
        EngineVolume engineVolume = new(request.EngineVolume);
        EnginePower enginePower = new(request.EnginePower);
        TractionType tractionType = new(request.TractionType);
        FuelConsumption fuelConsumption = new(request.FuelConsumption);
        SeatCount seatCount = new(request.SeatCount);
        Kilometer kilometer = new(request.Kilometer);
        DailyPrice dailyPrice = new(request.DailyPrice);
        WeeklyDiscountRate weeklyDiscountRate = new(request.WeeklyDiscountRate);
        MonthlyDiscountRate monthlyDiscountRate = new(request.MonthlyDiscountRate);
        InsuranceType insuranceType = new(request.InsuranceType);
        LastMaintenanceDate lastMaintenanceDate = new(request.LastMaintenanceDate);
        LastMaintenanceKm lastMaintenanceKm = new(request.LastMaintenanceKm);
        NextMaintenanceKm nextMaintenanceKm = new(request.NextMaintenanceKm);
        InspectionDate inspectionDate = new(request.InspectionDate);
        InsuranceEndDate insuranceEndDate = new(request.InsuranceEndDate);
        CascoEndDate? cascoEndDate = request.CascoEndDate is not null ? new(request.CascoEndDate.Value) : null;
        TireStatus tireStatus = new(request.TireStatus);
        GeneralStatus generalStatus = new(request.GeneralStatus);
        var features = request.Features.Select(f => new Feature(f)).ToList();

        vehicle.SetBrand(brand);
        vehicle.SetModel(model);
        vehicle.SetModelYear(modelYear);
        vehicle.SetColor(color);
        vehicle.SetPlate(plate);
        vehicle.SetCategoryId(categoryId);
        vehicle.SetBranchId(branchId);
        vehicle.SetVinNumber(vinNumber);
        vehicle.SetEngineNumber(engineNumber);
        vehicle.SetDescription(description);
        vehicle.SetImageUrl(imageUrl);
        vehicle.SetFuelType(fuelType);
        vehicle.SetTransmission(transmission);
        vehicle.SetEngineVolume(engineVolume);
        vehicle.SetEnginePower(enginePower);
        vehicle.SetTractionType(tractionType);
        vehicle.SetFuelConsumption(fuelConsumption);
        vehicle.SetSeatCount(seatCount);
        vehicle.SetKilometer(kilometer);
        vehicle.SetDailyPrice(dailyPrice);
        vehicle.SetWeeklyDiscountRate(weeklyDiscountRate);
        vehicle.SetMonthlyDiscountRate(monthlyDiscountRate);
        vehicle.SetInsuranceType(insuranceType);
        vehicle.SetLastMaintenanceDate(lastMaintenanceDate);
        vehicle.SetLastMaintenanceKm(lastMaintenanceKm);
        vehicle.SetNextMaintenanceKm(nextMaintenanceKm);
        vehicle.SetInspectionDate(inspectionDate);
        vehicle.SetInsuranceEndDate(insuranceEndDate);
        if (cascoEndDate is not null)
            vehicle.SetCascoEndDate(cascoEndDate);
        vehicle.SetTireStatus(tireStatus);
        vehicle.SetGeneralStatus(generalStatus);
        vehicle.SetFeatures(features);
        vehicle.SetStatus(request.IsActive);

        vehicleRepository.Update(vehicle);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return "Araç başarıyla güncellendi.";
    }
}