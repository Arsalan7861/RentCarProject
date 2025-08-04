import { EntityModel } from './entity.model';

export interface ExtraModel extends EntityModel {
  name: string;
  description: string;
  price: number;
}

export const initialExtra: ExtraModel = {
  id: '',
  name: '',
  description: '',
  price: 0,
  isActive: true,
  createdAt: '',
  createdBy: '',
  createdFullName: '',
};
