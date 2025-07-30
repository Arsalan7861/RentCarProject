import { Pipe, PipeTransform } from '@angular/core';
import { VehicleModel } from '../models/vehicle.model';

@Pipe({
  name: 'vehicle',
})
export class VehiclePipe implements PipeTransform {
  transform(
    value: VehicleModel[],
    categoryName: string,
    fuelType: string,
    transmission: string
  ): VehicleModel[] {
    if (categoryName === '' && fuelType === '' && transmission === '') {
      return value; // No filters applied, return all vehicles
    }

    return value.filter(
      (vehicle) => 
        vehicle.categoryName.includes(categoryName) &&
        vehicle.fuelType.includes(fuelType) &&
        vehicle.transmission.includes(transmission)
    );
  }
}
