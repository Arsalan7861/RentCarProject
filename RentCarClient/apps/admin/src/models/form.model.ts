/* eslint-disable @typescript-eslint/no-explicit-any */
export interface FormModel {
  reservationId: string;
  reservationNumber: string;
  reservationStatus: string;
  pickUpDateTime: string;
  deliveryDateTime: string;
  customerId: string;
  kilometer: number;
  customer: {
    fullName: string;
    identityNumber: string;
    phoneNumber: string;
    email: string;
    fullAddress: string;
  };
  vehicle: {
    id: string;
    brand: string;
    model: string;
    modelYear: number;
    color: string;
    categoryName: string;
    fuelConsumption: number;
    fuelType: string;
    seatCount: number;
    tractionType: string;
    kilometer: number;
    imageUrl: string;
    plate: string;
  };
  supplies: string[];
  imageUrls: string[];
  damages: {
    level: string;
    description: string;
  }[];
  note: string;
  files: any[];
}

export const initialFormModel: FormModel = {
  reservationId: '',
  reservationNumber: '',
  reservationStatus: '',
  pickUpDateTime: '',
  deliveryDateTime: '',
  customerId: '',
  kilometer: 0,
  customer: {
    fullName: '',
    identityNumber: '',
    phoneNumber: '',
    email: '',
    fullAddress: '',
  },
  vehicle: {
    id: '',
    brand: '',
    model: '',
    modelYear: 0,
    color: '',
    categoryName: '',
    fuelConsumption: 0,
    fuelType: '',
    seatCount: 0,
    tractionType: '',
    kilometer: 0,
    imageUrl: '',
    plate: '',
  },
  supplies: [],
  imageUrls: [],
  damages: [],
  note: '',
  files: [],
};
