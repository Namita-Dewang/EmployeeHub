export interface EmployeeModel {
  employeeId: number;
  name: string;
  contact: string;
  email: string;
  city: string;
  state: string;
  pincode: string;
  altContact: string;
  address: string;
  designationId: number;
  createdDate?: string;
  modifiedDate?: string;
  role: string;
}

export function createEmptyEmployee(): EmployeeModel {
  return {
    employeeId: 0,
    name: '',
    contact: '',
    email: '',
    city: '',
    state: '',
    pincode: '',
    altContact: '',
    address: '',
    designationId: 0,
    role: '',
  };
}
