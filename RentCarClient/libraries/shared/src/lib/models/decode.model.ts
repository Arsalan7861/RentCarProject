export interface DecodeModel {
  id: string;
  fullName: string;
  fullNameWithEmail: string;
  email: string;
  role: string;
  permissions: string[];
  branch: string;
  branchId: string;
  tfaStatus: boolean;
}

export const initialDecode: DecodeModel = {
  id: '',
  fullName: '',
  fullNameWithEmail: '',
  email: '',
  role: '',
  permissions: [],
  branch: '',
  branchId: '',
  tfaStatus: false,
};
